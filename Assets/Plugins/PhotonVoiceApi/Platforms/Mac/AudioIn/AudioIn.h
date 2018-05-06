/*
 
 Copyright (C) 2016 Apple Inc. All Rights Reserved.
 See LICENSE.txt for this sample’s licensing information
 
 Abstract:
 This class demonstrates the audio APIs used to capture audio data from the microphone and play it out to the speaker. It also demonstrates how to play system sounds
 
 */

#import <AudioToolbox/AudioToolbox.h>
#import <AVFoundation/AVFoundation.h>

extern "C" {
    typedef void (*Photon_IOSAudio_PushCallback)(int, float*, int);
}


struct CallbackData {
    AudioUnit rioUnit;
    BOOL audioChainIsBeingReconstructed;
    AudioBufferList bufferList;
    float* ringBuffer;
    int ringWritePos;
    int ringReadPos;
    
    int pushHostID;
    Photon_IOSAudio_PushCallback pushCallback;
    
    CallbackData(): rioUnit(NULL), audioChainIsBeingReconstructed(false),
    ringBuffer(NULL), ringWritePos(0), ringReadPos(0), pushHostID(0), pushCallback(NULL) {
        bufferList.mBuffers[0].mDataByteSize = 0;
        bufferList.mBuffers[0].mData = nullptr;
    }
    
    ~CallbackData() {
        if (ringBuffer)
            NSLog(@"[PV] [AudioIn] Freeing ring buffer");
        if (bufferList.mBuffers[0].mData)
            NSLog(@"[PV] [AudioIn] Freeing bufferList");
        free(ringBuffer);
        free(bufferList.mBuffers[0].mData);
    }
};

@interface Photon_Audio_In : NSObject {
    @public CallbackData cd;
    @public int deviceID;
}

@property (nonatomic, assign, readonly) BOOL audioChainIsBeingReconstructed;
- (int)setupIOUnit;
- (void)setupAudioChain;
- (OSStatus)    startIOUnit;
- (OSStatus)    stopIOUnit;
- (id)initWithDeviceID:(int)deviceID;
//- (double)      sessionSampleRate;

@end

@interface Photon_Audio_In_Enumerator : NSObject {
@public NSArray* devices;
}
@end

@interface DeviceInfo : NSObject {
@public
    AudioDeviceID id;
    CFStringRef uid;
    CFStringRef name;
    CFStringRef manufacturer;
};
@end

extern "C" {
    Photon_Audio_In* Photon_Audio_In_CreateReader(int deviceID);
    bool Photon_Audio_In_Read(Photon_Audio_In* handle, float* buf, int len);
    
    Photon_Audio_In* Photon_Audio_In_CreatePusher(int hostID, int deviceID, Photon_IOSAudio_PushCallback pushCallback);
    
    void Photon_Audio_In_Destroy(Photon_Audio_In* handle);
    
    Photon_Audio_In_Enumerator* Photon_Audio_In_CreateMicEnumerator();
    int Photon_Audio_In_MicEnumerator_Count(Photon_Audio_In_Enumerator* handle);
    const char* Photon_Audio_In_MicEnumerator_NameAtIndex(Photon_Audio_In_Enumerator* handle, int idx);
    int Photon_Audio_In_MicEnumerator_IDAtIndex(Photon_Audio_In_Enumerator* handle, int idx);
    void Photon_Audio_In_DestroyMicEnumerator(Photon_Audio_In_Enumerator* handle);
}
