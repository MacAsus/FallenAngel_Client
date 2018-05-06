//
//  AudioInEnum.m
//  AudioIn
//
//  Created by Vadim Lobanov on 19/10/2017.
//  Copyright © 2017 Exit Games GMBH. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "AudioIn.h"

@implementation DeviceInfo : NSObject {
@public

};
@end
// https://stackoverflow.com/questions/4575408/audioobjectgetpropertydata-to-get-a-list-of-input-devices
NSArray* CreateInputDeviceArray()
{
    AudioObjectPropertyAddress propertyAddress = {
        kAudioHardwarePropertyDevices,
        kAudioObjectPropertyScopeGlobal,
        kAudioObjectPropertyElementMaster
    };
    
    UInt32 dataSize = 0;
    OSStatus status = AudioObjectGetPropertyDataSize(kAudioObjectSystemObject, &propertyAddress, 0, NULL, &dataSize);
    if(kAudioHardwareNoError != status) {
        NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyDataSize (kAudioHardwarePropertyDevices) failed: %i\n", status);
        return NULL;
    }
    
    UInt32 deviceCount = static_cast<UInt32>(dataSize / sizeof(AudioDeviceID));
    
    AudioDeviceID *audioDevices = static_cast<AudioDeviceID *>(malloc(dataSize));
    if(NULL == audioDevices) {
        NSLog(@"[PV] [AudioIn] [ERROR] Unable to allocate memory");
        return NULL;
    }
    
    status = AudioObjectGetPropertyData(kAudioObjectSystemObject, &propertyAddress, 0, NULL, &dataSize, audioDevices);
    if(kAudioHardwareNoError != status) {
        NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyData (kAudioHardwarePropertyDevices) failed: %i\n", status);
        free(audioDevices), audioDevices = NULL;
        return NULL;
    }
    
    NSMutableArray* inputDeviceArray = [NSMutableArray arrayWithCapacity:deviceCount];
    if(NULL == inputDeviceArray) {
        NSLog(@"[PV] [AudioIn] [ERROR] CFArrayCreateMutable failed");
        free(audioDevices), audioDevices = NULL;
        return NULL;
    }
    
    // Iterate through all the devices and determine which are input-capable
    propertyAddress.mScope = kAudioDevicePropertyScopeInput;
    for(UInt32 i = 0; i < deviceCount; ++i) {
        // Query device UID
        CFStringRef deviceUID = NULL;
        dataSize = sizeof(deviceUID);
        propertyAddress.mSelector = kAudioDevicePropertyDeviceUID;
        status = AudioObjectGetPropertyData(audioDevices[i], &propertyAddress, 0, NULL, &dataSize, &deviceUID);
        if(kAudioHardwareNoError != status) {
            NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyData (kAudioDevicePropertyDeviceUID) failed: %i\n", status);
            continue;
        }
        
        // Query device name
        CFStringRef deviceName = NULL;
        dataSize = sizeof(deviceName);
        propertyAddress.mSelector = kAudioDevicePropertyDeviceNameCFString;
        status = AudioObjectGetPropertyData(audioDevices[i], &propertyAddress, 0, NULL, &dataSize, &deviceName);
        if(kAudioHardwareNoError != status) {
            NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyData (kAudioDevicePropertyDeviceNameCFString) failed: %i\n", status);
            continue;
        }
        
        // Query device manufacturer
        CFStringRef deviceManufacturer = NULL;
        dataSize = sizeof(deviceManufacturer);
        propertyAddress.mSelector = kAudioDevicePropertyDeviceManufacturerCFString;
        status = AudioObjectGetPropertyData(audioDevices[i], &propertyAddress, 0, NULL, &dataSize, &deviceManufacturer);
        if(kAudioHardwareNoError != status) {
            NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyData (kAudioDevicePropertyDeviceManufacturerCFString) failed: %i\n", status);
            continue;
        }
        
        // Determine if the device is an input device (it is an input device if it has input channels)
        dataSize = 0;
        propertyAddress.mSelector = kAudioDevicePropertyStreamConfiguration;
        status = AudioObjectGetPropertyDataSize(audioDevices[i], &propertyAddress, 0, NULL, &dataSize);
        if(kAudioHardwareNoError != status) {
            NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyDataSize (kAudioDevicePropertyStreamConfiguration) failed: %i\n", status);
            continue;
        }
        
        AudioBufferList *bufferList = static_cast<AudioBufferList *>(malloc(dataSize));
        if(NULL == bufferList) {
            NSLog(@"[PV] [AudioIn] [ERROR] Unable to allocate memory");
            break;
        }
        
        status = AudioObjectGetPropertyData(audioDevices[i], &propertyAddress, 0, NULL, &dataSize, bufferList);
        if(kAudioHardwareNoError != status || 0 == bufferList->mNumberBuffers) {
            if(kAudioHardwareNoError != status)
                NSLog(@"[PV] [AudioIn] [ERROR] AudioObjectGetPropertyData (kAudioDevicePropertyStreamConfiguration) failed: %i\n", status);
            free(bufferList), bufferList = NULL;
            continue;
        }
        
        NSLog(@"[PV] [AudioIn] Device %d \tdeviceUID: %s \tdeviceName: %s deviceManufacturer: %s", \
              audioDevices[i], \
              CFStringGetCStringPtr(deviceUID, kCFStringEncodingMacRoman),\
              CFStringGetCStringPtr(deviceName, kCFStringEncodingMacRoman), \
              CFStringGetCStringPtr(deviceManufacturer, kCFStringEncodingMacRoman)
              );
        
        free(bufferList), bufferList = NULL;
        DeviceInfo* d = [[DeviceInfo alloc] init];
        d->id = audioDevices[i];
        d->uid = deviceUID;
        d->name = deviceName;
        d->manufacturer = deviceManufacturer;
        [inputDeviceArray addObject:d];
    }
    
    free(audioDevices), audioDevices = NULL;
    
    NSArray *array = [inputDeviceArray copy];
    return array;
}


extern NSMutableSet* handles;

Photon_Audio_In_Enumerator* Photon_Audio_In_CreateMicEnumerator() {
    Photon_Audio_In_Enumerator* handle = [[Photon_Audio_In_Enumerator alloc] init];
    [handles addObject:handle];
    return handle;
}

int Photon_Audio_In_MicEnumerator_Count(Photon_Audio_In_Enumerator* handle) {
    if (handle->devices)
        return (int)[handle->devices count];
    else
        return 0;
}

const char* Photon_Audio_In_MicEnumerator_NameAtIndex(Photon_Audio_In_Enumerator* handle, int idx) {
    if (handle->devices && idx >=0 && idx < [handle->devices count])
        return CFStringGetCStringPtr(((DeviceInfo*)handle->devices[idx])->name, kCFStringEncodingMacRoman);
    else
        return NULL;
}

int Photon_Audio_In_MicEnumerator_IDAtIndex(Photon_Audio_In_Enumerator* handle, int idx) {
    if (handle->devices && idx >=0 && idx < [handle->devices count])
        return ((DeviceInfo*)handle->devices[idx])->id;
    else
        return -1;
}

@implementation Photon_Audio_In_Enumerator
- (id)init
{
    if (self = [super init]) {
        self->devices = CreateInputDeviceArray();
    }
    return self;
}

void Photon_Audio_In_DestroyMicEnumerator(Photon_Audio_In_Enumerator* handle) {
    [handles removeObject:handle];
}
@end
