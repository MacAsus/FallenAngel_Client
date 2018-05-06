#pragma once
#include "minwindef.h"

extern "C" {
	__declspec(dllexport) void* Photon_Audio_In_Create(int systemMode, int micDevIdx, int spkDevIdx, void(*callback)(BYTE*, ULONG), BOOL bFeatrModeOn, BOOL bNoiseSup, BOOL bAGC, BOOL bCntrClip);
	__declspec(dllexport) void Photon_Audio_In_Destroy(void* aec);

	__declspec(dllexport) void* Photon_Audio_In_CreateMicEnumerator();
	__declspec(dllexport) void Photon_Audio_In_DestroyMicEnumerator(void* handle);
	__declspec(dllexport) int Photon_Audio_In_MicEnumerator_Count(void* handle);
	// 
	__declspec(dllexport) const char* Photon_Audio_In_MicEnumerator_NameAtIndex(void* handle, int idx);
	// DirectX uses an index in the audio device collection for device identification. Capture devices are in the separate collection.
	// So we simply return 'idx' parameter.
	__declspec(dllexport) int Photon_Audio_In_MicEnumerator_IDAtIndex(void* handle, int idx);
}