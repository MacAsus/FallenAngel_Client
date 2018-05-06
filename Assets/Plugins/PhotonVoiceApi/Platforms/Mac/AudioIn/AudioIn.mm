// Photon_Audio_In works in 2 modes.
// 1. Fills buffer provided in ..._Read call. Takes minimum cpu cycles in render callback. But requires ring buffer adding some latency
// 2. Pushes audio data via callback as soon as data is available. Minimal latency and ability to use "push" Photon Voice interface which is more efficient.
// Push mode enabled if CallbackData.pushCallback property set.
// Sample rate = 44100

#import <Foundation/Foundation.h>
#import "AudioIn.h"

const int BUFFER_SIZE = 4096000;
NSMutableSet* handles = [[NSMutableSet alloc] init];

Photon_Audio_In* Photon_Audio_In_CreateReader(int deviceID) {
    Photon_Audio_In* handle = [[Photon_Audio_In alloc] initWithDeviceID:deviceID];
    NSLog(@"[PV] [AudioIn] Allocating ring buffer %d X %d", (int)sizeof(float), BUFFER_SIZE);
    handle->cd.ringBuffer = (float*)malloc(sizeof(float)*BUFFER_SIZE);
    [handles addObject:handle];
    [handle startIOUnit];
    return handle;
}

bool Photon_Audio_In_Read(Photon_Audio_In* handle, float* buf, int len) {
    CallbackData& cd = handle->cd;
    if (cd.ringReadPos + len > cd.ringWritePos) {
        return false;
    }
    if (cd.ringReadPos + BUFFER_SIZE < cd.ringWritePos) {
        cd.ringReadPos = cd.ringWritePos - BUFFER_SIZE;
    }
    
    int pos = cd.ringReadPos % BUFFER_SIZE;
    if (pos + len > BUFFER_SIZE) {
        int remains = BUFFER_SIZE - pos;
        memcpy(buf, cd.ringBuffer + pos, remains * sizeof(float));
        memcpy(buf + remains, cd.ringBuffer, (len - remains) * sizeof(float));
    } else {
        memcpy(buf, cd.ringBuffer + pos, len * sizeof(float));
    }
    // tone test
    //for(int i = 0;i < len;i++) buf[i] = sin((cd.ringReadPos + i) / 16.0f)/4;
    cd.ringReadPos += len;
    return true;
}

Photon_Audio_In* Photon_Audio_In_CreatePusher(int hostID, int deviceID, Photon_IOSAudio_PushCallback callback) {
    Photon_Audio_In* handle = [[Photon_Audio_In alloc] initWithDeviceID:deviceID];
    handle->cd.pushCallback = callback;
    handle->cd.pushHostID = hostID;
    [handles addObject:handle];
    [handle startIOUnit];
    return handle;
}

void Photon_Audio_In_Destroy(Photon_Audio_In* handle) {
    [handle stopIOUnit];
    [handles removeObject:handle];
}

// Render callback function
static OSStatus	performRender (void                         *inRefCon,
                               AudioUnitRenderActionFlags 	*ioActionFlags,
                               const AudioTimeStamp 		*inTimeStamp,
                               UInt32 						inBusNumber,
                               UInt32 						inNumberFrames,
                               AudioBufferList              *ioData)
{
    //NSLog(@"===== render ====== %p %d %d %d %d %d", inRefCon, ioData->mNumberBuffers, ioData->mBuffers[0].mDataByteSize, ioData->mBuffers[0].mNumberChannels, ioData->mBuffers[0].mDataByteSize, ioData->mBuffers[0].mNumberChannels);
    OSStatus err = noErr;
    CallbackData& cd = *((CallbackData*)inRefCon);
    if (cd.audioChainIsBeingReconstructed == NO)
    {
        AudioBufferList* bl = &cd.bufferList;
        
        if (bl->mBuffers[0].mDataByteSize != inNumberFrames * sizeof(float)) {
            if (bl->mBuffers[0].mData)
                NSLog(@"[PV] [AudioIn] Freeing bufferList");
            free(bl->mBuffers[0].mData);
            
            bl->mNumberBuffers = 1;
            bl->mBuffers[0].mNumberChannels = 1;
            bl->mBuffers[0].mDataByteSize = inNumberFrames * sizeof(float);
            NSLog(@"[PV] [AudioIn] Allocating bufferList %d X %d", (int)sizeof(float), inNumberFrames);
            bl->mBuffers[0].mData = calloc(inNumberFrames, sizeof(float));
        }

        err = AudioUnitRender(cd.rioUnit, ioActionFlags, inTimeStamp, 1, inNumberFrames, bl);

        if (!err) {
            if (cd.pushCallback) {
                cd.pushCallback(cd.pushHostID, (float*)bl->mBuffers[0].mData, inNumberFrames);
            } else {
                int pos = cd.ringWritePos % BUFFER_SIZE;
                if (pos + inNumberFrames > BUFFER_SIZE) {
                    int remains = BUFFER_SIZE - pos;
                    memcpy(cd.ringBuffer + pos, (float*)bl->mBuffers[0].mData, remains * sizeof(float));
                    memcpy(cd.ringBuffer, (float*)bl->mBuffers[0].mData + remains, (inNumberFrames - remains) * sizeof(float));
                } else {
                    memcpy(cd.ringBuffer + pos, (float*)bl->mBuffers[0].mData, inNumberFrames * sizeof(float));
                }
                // tone test
                //for(int i = 0;i < inNumberFrames;i++) cd.ringBuffer[(cd.ringWritePos + i) % BUFFER_SIZE] = sin((cd.ringWritePos + i) / 16.0f)/4;
                cd.ringWritePos += inNumberFrames;
            }
        }
    }
    
    return err;
}

@implementation Photon_Audio_In
CallbackData cd;
- (id)initWithDeviceID:(int)deviceID
{
    if (self = [super init]) {
        self->deviceID = deviceID;
        [self setupAudioChain];
    }
    return self;
}

- (void)handleMediaServerReset:(NSNotification *)notification
{
    NSLog(@"[PV] [AudioIn] Media server has reset");
    cd.audioChainIsBeingReconstructed = YES;
    
    usleep(25000); //wait here for some time to ensure that we don't delete these objects while they are being accessed elsewhere
    
    [self setupAudioChain];
    [self startIOUnit];
    
    cd.audioChainIsBeingReconstructed = NO;
}

- (int)setupIOUnit
{
    AudioObjectPropertyAddress propertyAddress = { kAudioHardwarePropertyDevices,
        kAudioObjectPropertyScopeGlobal,
        kAudioObjectPropertyElementMaster };
    
    UInt32 dataSize = 0;
    AudioObjectGetPropertyDataSize(kAudioObjectSystemObject, &propertyAddress, 0, NULL, &dataSize);
    AudioDeviceID devList[dataSize / sizeof(AudioDeviceID)];
    
    AudioObjectGetPropertyData(kAudioObjectSystemObject,
                               &propertyAddress,
                               0,
                               NULL,
                               &dataSize,
                               &devList);
//    for (int i = 0; i < dataSize / sizeof(AudioDeviceID); i++) {
//        NSLog(@"[PV] [AudioIn] =============== %d", devList[i]);
//    }
   
    NSLog(@"[PV] [AudioIn] Setting up input device %d", deviceID);
    AudioComponent                   component;
    AudioComponentDescription        description;
    OSStatus    err = noErr;
    UInt32  param;
    AURenderCallbackStruct  callback;
    
    description.componentType = kAudioUnitType_Output;
    description.componentSubType = kAudioUnitSubType_VoiceProcessingIO; // kAudioUnitSubType_HALOutput
    description.componentManufacturer = kAudioUnitManufacturer_Apple;
    description.componentFlags = 0;
    description.componentFlagsMask = 0;
    component = AudioComponentFindNext(nullptr, &description);
    if (!component)
    {
        NSLog(@"[PV] [AudioIn] could not find component");
        err = -1;
        return err;
    }
    
    err = AudioComponentInstanceNew(component, &cd.rioUnit);
    if(err != noErr)
    {
        NSLog(@"[PV] [AudioIn] could not craate new component instance");
        cd.rioUnit = NULL;
        return err;
    }
    
    callback.inputProc = performRender;
    callback.inputProcRefCon = &cd;
    err = AudioUnitSetProperty(cd.rioUnit, kAudioOutputUnitProperty_SetInputCallback, kAudioUnitScope_Output, 1, &callback, sizeof(callback));
    if(err != noErr)
    {
        NSLog(@"[PV] [AudioIn] [ERROR] couldn't set render callback on AURemoteIO");
        return err;
    }
    
    AudioDeviceID device;
    UInt32 deviceSizeof = sizeof(AudioDeviceID);
    if (deviceID == -1) {
        err = AudioHardwareGetProperty(kAudioHardwarePropertyDefaultInputDevice, &deviceSizeof, &device);
        if(err != noErr)
        {
            NSLog(@"[PV] [AudioIn] [ERROR] couldn't get default input device");
            return err;
        }
    } else {
        device = deviceID;

        err = AudioUnitSetProperty(cd.rioUnit, kAudioOutputUnitProperty_CurrentDevice, kAudioUnitScope_Global, 1, &device, sizeof(AudioDeviceID));
        if(err != noErr)
        {
            NSLog(@"[PV] [AudioIn] [ERROR] could not set current device %d / %d", deviceID, device);
            return err;
        }
    }
    UInt32 fAudioSamples;
    param = sizeof(UInt32);
    err = AudioUnitGetProperty(cd.rioUnit, kAudioDevicePropertyBufferFrameSize, kAudioUnitScope_Global, 0, &fAudioSamples, &param);
    if(err != noErr)
    {
        NSLog(@"[PV] [AudioIn] [ERROR] failed to get audio sample size\n");
        return err;
    }
    err = AudioUnitInitialize(cd.rioUnit);
    if(err != noErr)
    {
        NSLog(@"[PV] [AudioIn] [ERROR] failed to initialize AU\n");
        return err;
    }
    return 0;
}

- (void)setupAudioChain
{
    NSLog(@"[PV] [AudioIn] start AURemoteIO: %d", [self setupIOUnit]);
}

- (OSStatus)startIOUnit
{
    OSStatus err = AudioOutputUnitStart(cd.rioUnit);
    if (err) NSLog(@"[PV] [AudioIn] couldn't start AURemoteIO: %d", (int)err);
    return err;
}

- (OSStatus)stopIOUnit
{
    OSStatus err = AudioOutputUnitStop(cd.rioUnit);
    if (err) NSLog(@"[PV] [AudioIn] couldn't stop AURemoteIO: %d", (int)err);
    NSLog(@"[PV] [AudioIn] stop AURemoteIO: %d", (int)err);
    return err;
}
/*
- (double)sessionSampleRate
{
    return [[AVAudioSession sharedInstance] sampleRate];
}
*/
- (BOOL)audioChainIsBeingReconstructed
{
    return cd.audioChainIsBeingReconstructed;
}

@end
