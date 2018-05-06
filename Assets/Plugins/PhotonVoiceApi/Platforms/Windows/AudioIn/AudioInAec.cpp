#include <windows.h>
#include <dmo.h>
#include <Mmsystem.h>
#include <objbase.h>
#include <mediaobj.h>
#include <uuids.h>
#include <propidl.h>
#include <wmcodecdsp.h>

#include <atlbase.h>
#include <ATLComCli.h>
#include <audioclient.h>
#include <MMDeviceApi.h>
#include <AudioEngineEndPoint.h>
#include <DeviceTopology.h>
#include <propkey.h>
#include <strsafe.h>

#include "mediabuf.h"

#include "AecKsBinder.h"

#define SAFE_ARRAYDELETE(p) {if (p) delete[] (p); (p) = NULL;}
#define SAFE_RELEASE(p) {if (NULL != p) {(p)->Release(); (p) = NULL;}}

#define VBFALSE (VARIANT_BOOL)0
#define VBTRUE  (VARIANT_BOOL)-1

#define STREAM_BUFFER_LENGTH 0.1f  //streaming buffer is 0.1 second long.

#define CHECK_RET(hr, message) if (FAILED(hr)) { puts(message); goto exit;}
#define CHECKHR(x) hr = x; if (FAILED(hr)) {printf("%d: %08X\n", __LINE__, hr); goto exit;}
#define CHECK_ALLOC(pb, message) if (NULL == pb) { puts(message); goto exit;}

void SetThreadName(DWORD dwThreadID, const char* threadName);

typedef void(__stdcall *PushCallback)(BYTE*, ULONG);

class CStaticMediaBuffer : public CBaseMediaBuffer {
public:
	STDMETHODIMP_(ULONG) AddRef() { return 2; }
	STDMETHODIMP_(ULONG) Release() { return 1; }
	void Init(BYTE *pData, ULONG ulSize, ULONG ulData) {
		m_pData = pData;
		m_ulSize = ulSize;
		m_ulData = ulData;
	}
};

static DWORD WINAPI workerFuncDispatch(LPVOID lpParam);

class AudioInAec {
private:
	PushCallback callback;
	// Parameters to config DMO
	int  iSystemMode = MODE_NOT_SET;    // AEC-MicArray DMO system mode
	int  iMicDevIdx = -2;               // microphone device index
	int  iSpkDevIdx = -2;               // speaker device index
	BOOL bFeatrModeOn = 0;              // turn feature mode on/off
	BOOL bNoiseSup = 1;                 // turn noise suppression on/off
	BOOL bAGC = 0;                      // turn digital auto gain control on/off
	BOOL bCntrClip = 0;                 // turn center clippng on/off

	DWORD cOutputBufLen = 0;
	BYTE *pbOutputBuffer = NULL;

	IMediaObject* pDMO = NULL;
	IPropertyStore* pPS = NULL;

	CStaticMediaBuffer outputBuffer;
	DMO_OUTPUT_DATA_BUFFER OutputBufferStruct = { 0 };

	bool exit = false;
	HANDLE worker = 0;
	DWORD workerID = 0;

public:
	// iSystemMode: AEC-MicArray system mode. The valid modes are
	//   SINGLE_CHANNEL_AEC = 0
	//   OPTIBEAM_ARRAY_ONLY = 2
	//   OPTIBEAM_ARRAY_AND_AEC = 4
	//   SINGLE_CHANNEL_NSAGC = 5
	// Mode 1 and 3 are reserved for future features.
	AudioInAec(int iSystemMode, int iMicDevIdx, int iSpkDevIdx, PushCallback callback, BOOL bFeatrModeOn, BOOL bNoiseSup, BOOL bAGC, BOOL bCntrClip)
		: iSystemMode(iSystemMode), 
		iMicDevIdx(iMicDevIdx), 
		iSpkDevIdx(iSpkDevIdx), 
		callback(callback),  
		bFeatrModeOn(bFeatrModeOn),
		bNoiseSup(bNoiseSup),
		bAGC(bAGC),
		bCntrClip(bCntrClip)
	{
	}

	~AudioInAec() {		
	}

	void Close() {
		puts("Closing...");
		exit = true;
		WaitForSingleObject(worker, INFINITE);
		CloseHandle(worker);
		puts("... closed");
	}

	bool Init() {
		worker = CreateThread(
			NULL,                   // default security attributes
			0,                      // use default stack size  
			&workerFuncDispatch,       // thread function name
			this,          // argument to thread function 
			0,                      // use default creation flags 
			&workerID);   // returns the thread identifier 
		SetThreadName(workerID, "AudioInAec");
		return true;
	}

	DWORD WINAPI workerFunc() {
		HRESULT hr = S_OK;
		
		printf("CoInitializeEx %d", CoInitializeEx(NULL, COINIT_MULTITHREADED));

		OutputBufferStruct.pBuffer = &outputBuffer;
		DMO_MEDIA_TYPE mt = { 0 };

		WAVEFORMATEX wfxOut = { WAVE_FORMAT_PCM, 1, 16000, 32000, 2, 16, 0 };

		HANDLE currThread;
		HANDLE currProcess;
		BOOL iRet;
		currProcess = GetCurrentProcess();
		currThread = GetCurrentThread();

		iRet = SetPriorityClass(currProcess, HIGH_PRIORITY_CLASS);
		if (0 == iRet)
		{
			// call getLastError.
			puts("failed to set process priority\n");
			goto exit;
		}

		// DMO initialization
		CHECKHR(CoCreateInstance(CLSID_CWMAudioAEC, NULL, CLSCTX_INPROC_SERVER, IID_IMediaObject, (void**)&pDMO));
		CHECKHR(pDMO->QueryInterface(IID_IPropertyStore, (void**)&pPS));


		// Set AEC mode and other parameters
		// Not all user changeable options are given in this sample code.
		// Please refer to readme.txt for more options.

		// Set AEC-MicArray DMO system mode.
		// This must be set for the DMO to work properly
		puts("\nAEC settings:");
		PROPVARIANT pvSysMode;
		PropVariantInit(&pvSysMode);
		pvSysMode.vt = VT_I4;
		pvSysMode.lVal = (LONG)(iSystemMode);
		CHECKHR(pPS->SetValue(MFPKEY_WMAAECMA_SYSTEM_MODE, pvSysMode));
		CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_SYSTEM_MODE, &pvSysMode));
		printf("%20s %5d \n", "System Mode is", pvSysMode.lVal);
		PropVariantClear(&pvSysMode);

		// Tell DMO which capture and render device to use 
		// This is optional. If not specified, default devices will be used
		if (iMicDevIdx >= 0 || iSpkDevIdx >= 0)
		{
			PROPVARIANT pvDeviceId;
			PropVariantInit(&pvDeviceId);
			pvDeviceId.vt = VT_I4;
			pvDeviceId.lVal = (unsigned long)(iSpkDevIdx << 16) + (unsigned long)(0x0000ffff & iMicDevIdx);
			CHECKHR(pPS->SetValue(MFPKEY_WMAAECMA_DEVICE_INDEXES, pvDeviceId));
			CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_DEVICE_INDEXES, &pvDeviceId));
			PropVariantClear(&pvDeviceId);
		}

		if (bFeatrModeOn)
		{
			// Turn on feature modes
			PROPVARIANT pvFeatrModeOn;
			PropVariantInit(&pvFeatrModeOn);
			pvFeatrModeOn.vt = VT_BOOL;
			pvFeatrModeOn.boolVal = bFeatrModeOn ? VBTRUE : VBFALSE;
			CHECKHR(pPS->SetValue(MFPKEY_WMAAECMA_FEATURE_MODE, pvFeatrModeOn));
			CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_FEATURE_MODE, &pvFeatrModeOn));
			printf("%20s %5d \n", "Feature Mode is", pvFeatrModeOn.boolVal);
			PropVariantClear(&pvFeatrModeOn);

			// Turn on/off noise suppression
			PROPVARIANT pvNoiseSup;
			PropVariantInit(&pvNoiseSup);
			pvNoiseSup.vt = VT_I4;
			pvNoiseSup.lVal = (LONG)bNoiseSup;
			CHECKHR(pPS->SetValue(MFPKEY_WMAAECMA_FEATR_NS, pvNoiseSup));
			CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_FEATR_NS, &pvNoiseSup));
			printf("%20s %5d \n", "Noise suppresion is", pvNoiseSup.lVal);
			PropVariantClear(&pvNoiseSup);

			// Turn on/off AGC
			PROPVARIANT pvAGC;
			PropVariantInit(&pvAGC);
			pvAGC.vt = VT_BOOL;
			pvAGC.boolVal = bAGC ? VBTRUE : VBFALSE;
			CHECKHR(pPS->SetValue(MFPKEY_WMAAECMA_FEATR_AGC, pvAGC));
			CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_FEATR_AGC, &pvAGC));
			printf("%20s %5d \n", "AGC is", pvAGC.boolVal);
			PropVariantClear(&pvAGC);

			// Turn on/off center clip
			PROPVARIANT pvCntrClip;
			PropVariantInit(&pvCntrClip);
			pvCntrClip.vt = VT_BOOL;
			pvCntrClip.boolVal = bCntrClip ? VBTRUE : VBFALSE;
			CHECKHR(pPS->SetValue(MFPKEY_WMAAECMA_FEATR_CENTER_CLIP, pvCntrClip));
			CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_FEATR_CENTER_CLIP, &pvCntrClip));
			printf("%20s %5d \n", "Center clip is", (BOOL)pvCntrClip.boolVal);
			PropVariantClear(&pvCntrClip);
		}

		// Set DMO output format
		hr = MoInitMediaType(&mt, sizeof(WAVEFORMATEX));
		CHECK_RET(hr, "MoInitMediaType failed");

		mt.majortype = MEDIATYPE_Audio;
		mt.subtype = MEDIASUBTYPE_PCM;
		mt.lSampleSize = 0;
		mt.bFixedSizeSamples = TRUE;
		mt.bTemporalCompression = FALSE;
		mt.formattype = FORMAT_WaveFormatEx;
		memcpy(mt.pbFormat, &wfxOut, sizeof(WAVEFORMATEX));

		hr = pDMO->SetOutputType(0, &mt, 0);
		CHECK_RET(hr, "SetOutputType failed");
		MoFreeMediaType(&mt);

		// Allocate streaming resources. This step is optional. If it is not called here, it
		// will be called when first time ProcessInput() is called. However, if you want to 
		// get the actual frame size being used, it should be called explicitly here.
		hr = pDMO->AllocateStreamingResources();
		CHECK_RET(hr, "AllocateStreamingResources failed");

		// Get actually frame size being used in the DMO. (optional, do as you need)
		int iFrameSize;
		PROPVARIANT pvFrameSize;
		PropVariantInit(&pvFrameSize);
		CHECKHR(pPS->GetValue(MFPKEY_WMAAECMA_FEATR_FRAME_SIZE, &pvFrameSize));
		iFrameSize = pvFrameSize.lVal;
		PropVariantClear(&pvFrameSize);

		// allocate output buffer
		cOutputBufLen = wfxOut.nSamplesPerSec * wfxOut.nBlockAlign;
		pbOutputBuffer = new BYTE[cOutputBufLen];
		CHECK_ALLOC(pbOutputBuffer, "out of memory.\n");

		while (!exit) {
			do {
				Sleep(10); //sleep 10ms
				outputBuffer.Init((byte*)pbOutputBuffer, cOutputBufLen, 0);
				OutputBufferStruct.dwStatus = 0;
				ULONG cbProduced = 0;
				DWORD dwStatus;
				HRESULT hr = pDMO->ProcessOutput(0, 1, &OutputBufferStruct, &dwStatus);
				// 0 - ok
				// 1 - no output signal
				// -2016673782 - there has not been for few sec. (can't sync with output?)
				CHECK_RET(hr, "ProcessOutput failed");

				if (hr == S_FALSE) {
					cbProduced = 0;
				}
				else {
					hr = outputBuffer.GetBufferAndLength(NULL, &cbProduced);
					CHECK_RET(hr, "GetBufferAndLength failed");
					callback(pbOutputBuffer, cbProduced);
				}
			} while (OutputBufferStruct.dwStatus & DMO_OUTPUT_DATA_BUFFERF_INCOMPLETE);
		}

exit:
		SAFE_ARRAYDELETE(pbOutputBuffer);

		SAFE_RELEASE(pDMO);
		SAFE_RELEASE(pPS);

		CoUninitialize();

		return true;
	}
};

static DWORD WINAPI workerFuncDispatch(LPVOID lpParam) {
	AudioInAec* aec = (AudioInAec*)lpParam;
	return aec->workerFunc();	
}

extern "C" {
	__declspec(dllexport) void* Photon_Audio_In_Create(int iSystemMode, int iMicDevIdx, int iSpkDevIdx, PushCallback callback, BOOL bFeatrModeOn, BOOL bNoiseSup, BOOL bAGC, BOOL bCntrClip) {
		AudioInAec* aec = new AudioInAec(iSystemMode, iMicDevIdx, iSpkDevIdx, callback, bFeatrModeOn, bNoiseSup, bAGC, bCntrClip);
		if (aec->Init())
			return aec;
		else {
			aec->Close();
			return NULL;
		}
	}

	__declspec(dllexport) void Photon_Audio_In_Destroy(void* x) {
		AudioInAec* aec = (AudioInAec*)x;
		aec->Close();
		delete (aec);
	}
	
}