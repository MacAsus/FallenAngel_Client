// --------------------------------------------------------------------------
// MFWMAAEC (Aec-MicArray DMO) sample code.
//
// Note: 
//  1. DirectX SDK must be installed before compiling. 
//  2. DirectX SDK include path should be added after the VC include
//     path, because strsafe.h in DirectX SDK may be older.
//  3. platform SDK lib path should be added before the VC lib
//     path, because uuid.lib in VC lib path may be older
//  4. To run the demo properly for AEC enabled modes (mode 0 and 4), 
//     users must play some audio signals through the SAME speaker  
//     device specified for the DMO (i.e. the device specified by 
//     "-spkdev" option). These audio signals simulate far-end voices 
//     in a two-way chatting scenario. Users may use any player to play 
//     any audio signals. If there is no active render stream on the
//     selected speaker device, the AEC DMO will fail to process.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) 2004-2006, Microsoft Corporation. All rights reserved.
//---------------------------------------------------------------------------

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
#include <conio.h>

#include "AecKsBinder.h"
#include "AudioInAec.h"

#define SAFE_ARRAYDELETE(p) {if (p) delete[] (p); (p) = NULL;}
#define SAFE_RELEASE(p) {if (NULL != p) {(p)->Release(); (p) = NULL;}}

#define VBFALSE (VARIANT_BOOL)0
#define VBTRUE  (VARIANT_BOOL)-1

#define STREAM_BUFFER_LENGTH 0.1f  //streaming buffer is 0.1 second long.

#define CHECK_RET(hr, message) if (FAILED(hr)) { puts(message); goto exit;}
#define CHECKHR(x) hr = x; if (FAILED(hr)) {printf("%d: %08X\n", __LINE__, hr); goto exit;}
#define CHECK_ALLOC(pb, message) if (NULL == pb) { puts(message); goto exit;}

void OutputUsage();

static FILE * pfMicOutPCM;  // dump output signal using PCM format

void push(BYTE* buf, ULONG size) {
	if (fwrite(buf, 1, size, pfMicOutPCM) != size)
	{
		puts("write error");
	}
}

int __cdecl _tmain(int argc, const TCHAR ** argv)
{
	HRESULT hr = S_OK;
	void* aec = NULL;
	CoInitialize(NULL);

	// Parameters to config DMO
	int  iSystemMode = MODE_NOT_SET;    // AEC-MicArray DMO system mode
	int  iOutFileIdx = -1;              // argument index for otuput file name
	int  iMicDevIdx = -2;               // microphone device index
	int  iSpkDevIdx = -2;               // speaker device index
	BOOL bFeatrModeOn = 0;              // turn feature mode on/off
	BOOL bNoiseSup = 1;                 // turn noise suppression on/off
	BOOL bAGC = 0;                      // turn digital auto gain control on/off
	BOOL bCntrClip = 0;                 // turn center clippng on/off

	// control how long the Demo runs
	int  iDuration = 60;   // seconds
	int  cTtlToGo = 0;

	AUDIO_DEVICE_INFO *pCaptureDeviceInfo = NULL, *pRenderDeviceInfo = NULL;

	int i;
	for (i = 1; i < argc - 1; i++)
	{
		if (argv[i][0] == '-')
		{
			if (!_tcscmp(_T("-micdev"), argv[i]))
			{   // microphone device index. The valid range is -1, 0~N-1
				// where N is the number of capture device. Use -1 if
				// you want to use the default device.
				iMicDevIdx = _ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-spkdev"), argv[i]))
			{   // speaker device index. The valid values are -1, 0~N-1
				// where N is the number of capture device. Use -1 if
				// you want to use the default device.
				iSpkDevIdx = _ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-out"), argv[i]))
			{   // output file name
				iOutFileIdx = i + 1; i++;
			}
			else if (!_tcscmp(_T("-mod"), argv[i]))
			{   // AEC-MicArray system mode. The valid modes are
				//   SINGLE_CHANNEL_AEC = 0
				//   OPTIBEAM_ARRAY_ONLY = 2
				//   OPTIBEAM_ARRAY_AND_AEC = 4
				//   SINGLE_CHANNEL_NSAGC = 5
				//
				// Mode 1 and 3 are reserved for future features.
				iSystemMode = _ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-feat"), argv[i]))
			{   // turn feature mode on/off. The valid values are 0 or 1
				// The feature mode must be turned on in order to config
				// noise suppression, AGC, centerclip, and other AEC features.
				bFeatrModeOn = _ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-ns"), argv[i]))
			{   // turn noise suppression on/off. The valid values are 0 or 1
				// Feature mode must be on in order to set noise suppression
				bNoiseSup = _ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-agc"), argv[i]))
			{   // turn digital AGC on/off. The valid values are 0 or 1
				// Feature mode must be on in order to set digital AGC
				bAGC = _ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-cntrclip"), argv[i]))
			{   // turn center clipping on/off. The valid values are 0 or 1
				// Center clipping is an post process to remove small echo residuals 
				// which are not completely cancelled. Comfort noise with a same level
				// of background noise will be filled after the removal.
				// Feature mode must be on in order to set center clipping
				bCntrClip = (BOOL)_ttoi(argv[i + 1]); i++;
			}
			else if (!_tcscmp(_T("-duration"), argv[i]))
			{   // control program running duration in seconds. The default 
				// value is 60 seconds.
				iDuration = _ttoi(argv[i + 1]); i++;
			}
			else
			{
				OutputUsage();
				goto exit;
			}
		}
	}

	// display usage info if required arguments are not specified
	if (iSystemMode == MODE_NOT_SET || iOutFileIdx == -1)
	{
		OutputUsage();
		goto exit;
	}

	// Select capture device
	UINT uCapDevCount = 0;
	UINT uRenDevCount = 0;
	char  pcScanBuf[256] = { 0 };

	hr = GetCaptureDeviceNum(uCapDevCount);
	CHECK_RET(hr, "GetCaptureDeviceNum failed");

	pCaptureDeviceInfo = new AUDIO_DEVICE_INFO[uCapDevCount];
	hr = EnumCaptureDevice(uCapDevCount, pCaptureDeviceInfo);
	CHECK_RET(hr, "EnumCaptureDevice failed");

	printf("\nSystem has totally %d capture devices\n", uCapDevCount);
	for (i = 0; i < (int)uCapDevCount; i++)
	{
		_tprintf(_T("Device %d is %s"), i, pCaptureDeviceInfo[i].szDeviceName);
		if (pCaptureDeviceInfo[i].bIsMicArrayDevice)
			_tprintf(_T(" -- Mic Array Device \n"));
		else
			_tprintf(_T("\n"));
	}

	if (iMicDevIdx < -1 || iMicDevIdx >= (int)uCapDevCount)
	{
		do {
			printf("Select device ");
			scanf_s("%255s", pcScanBuf, 255);
			iMicDevIdx = atoi(pcScanBuf);
			if (iMicDevIdx < -1 || iMicDevIdx >= (int)uCapDevCount)
				printf("Invalid Capture Device ID \n");
			else
				break;
		} while (1);
	}
	if (iMicDevIdx == -1)
		_tprintf(_T("\n Default device will be used for capturing \n"));
	else
		_tprintf(_T("\n %s is selected for capturing\n"), pCaptureDeviceInfo[iMicDevIdx].szDeviceName);
	SAFE_ARRAYDELETE(pCaptureDeviceInfo);


	// Select render device
	if (iSystemMode == SINGLE_CHANNEL_AEC ||
		iSystemMode == ADAPTIVE_ARRAY_AND_AEC ||
		iSystemMode == OPTIBEAM_ARRAY_AND_AEC)
	{
		hr = GetRenderDeviceNum(uRenDevCount);
		CHECK_RET(hr, "GetRenderDeviceNum failed");

		pRenderDeviceInfo = new AUDIO_DEVICE_INFO[uRenDevCount];
		hr = EnumRenderDevice(uRenDevCount, pRenderDeviceInfo);
		CHECK_RET(hr, "EnumRenderDevice failed");

		printf("\nSystem has totally %d render devices\n", uRenDevCount);
		for (i = 0; i < (int)uRenDevCount; i++)
		{
			_tprintf(_T("Device %d is %s \n"), i, pRenderDeviceInfo[i].szDeviceName);
		}

		if (iSpkDevIdx < -1 || iSpkDevIdx >= (int)uRenDevCount)
		{
			do {
				printf("Select device ");
				scanf_s("%255s", pcScanBuf, 255);
				iSpkDevIdx = atoi(pcScanBuf);
				if (iSpkDevIdx < -1 || iSpkDevIdx >= (int)uRenDevCount)
					printf("Invalid Render Device ID \n");
				else
					break;
			} while (1);
		}
		if (iSpkDevIdx == -1)
			_tprintf(_T("\n Default device will be used for rendering \n"));
		else
			_tprintf(_T("\n %s is selected for rendering \n"), pRenderDeviceInfo[iSpkDevIdx].szDeviceName);
	}
	else {
		iSpkDevIdx = -1;
	}

	SAFE_ARRAYDELETE(pRenderDeviceInfo);

	// --- PREPARE OUTPUT --- //
	if (NULL != _tfopen_s(&pfMicOutPCM, argv[iOutFileIdx], _T("wb")))
	{
		puts("cannot open file for output.\n");
		goto exit;
	}

	// number of frames to play
	cTtlToGo = iDuration * 100;

	aec = Photon_Audio_In_Create(iSystemMode, iMicDevIdx, iSpkDevIdx, push, bFeatrModeOn, bNoiseSup, bAGC, bCntrClip);
	if (!aec) {
		puts("Failed to create autio input.\n");
		return -1;
	}

	// main loop to get mic output from the DMO
	puts("\nAEC-MicArray is running ... Press \"s\" to stop");
	while (1)
	{
		Sleep(10); //sleep 10ms

		if (cTtlToGo-- <= 0)
			break;

		// check keyboard input to stop
		if (_kbhit())
		{
			int ch = _getch();
			if (ch == 's' || ch == 'S')
				break;
		}
	}

exit:
	if (aec) {
		Photon_Audio_In_Destroy(aec);
	}
}

void OutputUsage()
{
    printf("MFWMAAEC (Aec-MicArray DMO) Demo. \n");
    printf("Copyright (c) 2004-2006, Microsoft Corporation. All rights reserved. \n\n");
    printf("Usage: AecSDKDemo.exe -out mic_out.pcm -mod 0 [-feat 1] [-ns 1] [-agc 0] \n");
    printf("       [-cntrclip 0] [-micdev 0] [-spkdev 0] [-duration 60]\n");
    return;
}
