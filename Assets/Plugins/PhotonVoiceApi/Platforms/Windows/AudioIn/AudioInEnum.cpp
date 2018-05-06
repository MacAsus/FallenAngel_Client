#include "AecKsBinder.h"

#define CHECK_RET(hr, message) if (FAILED(hr)) { puts(message); goto exit;}
#define SAFE_ARRAYDELETE(p) {if (p) delete[] (p); (p) = NULL;}

struct AudioDeviceInfos {
	AUDIO_DEVICE_INFO* devices;
	UINT count;
};
extern "C" {
	__declspec(dllexport) void* Photon_Audio_In_CreateMicEnumerator() {
		HRESULT hr = S_OK;
		AUDIO_DEVICE_INFO *pCaptureDeviceInfo = NULL, *pRenderDeviceInfo = NULL;
		UINT uCapDevCount = 0;
		UINT uRenDevCount = 0;
		char  pcScanBuf[256] = { 0 };

		hr = GetCaptureDeviceNum(uCapDevCount);
		CHECK_RET(hr, "GetCaptureDeviceNum failed");

		pCaptureDeviceInfo = new AUDIO_DEVICE_INFO[uCapDevCount];
		hr = EnumCaptureDevice(uCapDevCount, pCaptureDeviceInfo);
		CHECK_RET(hr, "EnumCaptureDevice failed");

		printf("\nSystem has totally %d capture devices\n", uCapDevCount);
		for (int i = 0; i < (int)uCapDevCount; i++)
		{
			_tprintf(_T("Device %d is %s"), i, pCaptureDeviceInfo[i].szDeviceName);
			if (pCaptureDeviceInfo[i].bIsMicArrayDevice)
				_tprintf(_T(" -- Mic Array Device \n"));
			else
				_tprintf(_T("\n"));
		}

		return new AudioDeviceInfos{ pCaptureDeviceInfo , uCapDevCount };
	exit:
		SAFE_ARRAYDELETE(pCaptureDeviceInfo);
		return NULL;
	}
	__declspec(dllexport) void Photon_Audio_In_DestroyMicEnumerator(void* handle) {
		AudioDeviceInfos* infos = (AudioDeviceInfos*)handle;
		SAFE_ARRAYDELETE(infos->devices);
		delete infos;
	}
	__declspec(dllexport) int Photon_Audio_In_MicEnumerator_Count(void* handle) {
		AudioDeviceInfos* infos = (AudioDeviceInfos*)handle;
		return infos->count;
	}
	__declspec(dllexport) wchar_t* Photon_Audio_In_MicEnumerator_NameAtIndex(void* handle, int idx) {
		AudioDeviceInfos* infos = (AudioDeviceInfos*)handle;
		if (idx >= 0 && (UINT)idx < infos->count) {
			return infos->devices[idx].szDeviceName;
		}
		else {
			return NULL;
		}
	}
	__declspec(dllexport) int Photon_Audio_In_MicEnumerator_IDAtIndex(void* handle, int idx) {
		AudioDeviceInfos* infos = (AudioDeviceInfos*)handle;
		if (idx >= 0 && (UINT)idx < infos->count) {
			return idx;
		}
		else {
			return -1;
		}
	}
}
