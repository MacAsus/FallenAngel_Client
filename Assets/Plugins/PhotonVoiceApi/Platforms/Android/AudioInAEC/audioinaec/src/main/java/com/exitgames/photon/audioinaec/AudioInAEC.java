package com.exitgames.photon.audioinaec;

import android.media.AudioManager;
import android.content.Context;
import android.media.AudioFormat;
import android.media.AudioRecord;
import android.media.MediaRecorder;
import android.media.audiofx.AcousticEchoCanceler;
import android.media.audiofx.AutomaticGainControl;
import android.media.audiofx.NoiseSuppressor;
import android.provider.Settings;
import android.util.Log;
import android.os.Process;
import android.os.Build;


/**
 * Created by vadim on 7/28/2017.
 */

public class AudioInAEC {
	interface DataCallback {
		void OnData();
		void OnStop();
	}

// when passing short[] in callback directly, "NoSuchMethodError: no method with name='getLength' signature='(L[S;)I' in class Ljava/lang/reflect/Array" is thrown
	// ENCODING_PCM_FLOAT added in API level 21
	public static final int format = AudioFormat.ENCODING_PCM_16BIT;
    private AudioRecord audioRecord;
    volatile private boolean exit;
	private AcousticEchoCanceler aec;
	private AutomaticGainControl agc;
	private NoiseSuppressor ns;
	short[] buf;

    public boolean SetBuffer(short[] buf) {
		Log.i("[PV] AudioInAEC", "AudioRecord buffer set, size: " + buf.length);
		this.buf = buf;
		return true;
	}

    public boolean Start(final Context ctx, final DataCallback pushCallback, final int samplingRate, final int channels, final int recordBufSizeBytes, final boolean enableAEC) {
//		AudioManager audioManager = (AudioManager)ctx.getSystemService(Context.AUDIO_SERVICE);
//		Log.i("[PV] AudioInAEC", "AudioManager mode: " + audioManager.getMode());
//		audioManager.setMode(AudioManager.MODE_IN_COMMUNICATION);
//		audioManager.setSpeakerphoneOn(true);
//		Log.i("[PV] AudioInAEC", "AudioManager mode: " + audioManager.getMode());
		
		final int channelConfig = channels == 1 ? AudioFormat.CHANNEL_IN_MONO : AudioFormat.CHANNEL_IN_STEREO;
		if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
			audioRecord = new AudioRecord.Builder()
				.setAudioSource(MediaRecorder.AudioSource.VOICE_COMMUNICATION)
				.setAudioFormat(new AudioFormat.Builder()
				.setEncoding(format)
				.setSampleRate(samplingRate)
				.setChannelMask(channelConfig)
				.build())
				.setBufferSizeInBytes(recordBufSizeBytes)
				.build();
		} else {
			audioRecord = new AudioRecord(MediaRecorder.AudioSource.VOICE_COMMUNICATION, samplingRate, channelConfig, format, recordBufSizeBytes);
		}
		Log.i("[PV] AudioInAEC", "AudioRecord created: " + audioRecord + ": samplingRate: " + samplingRate + ", channels: " + channels + ", recordBufSizeBytes: " + recordBufSizeBytes + ", enableAEC: " + enableAEC);
		
		// Depending on device, effects may be enabled by default for AudioSource.VOICE_COMMUNICATION (and seems like can't be disabled)
		// From the doc: On some devices, an AEC can be inserted by default in the capture path by the platform according to the MediaRecorder.AudioSource used. The application should call AcousticEchoCanceler.getEnable() after creating the AEC to check the default AEC activation state on a particular AudioRecord session.
		if(AcousticEchoCanceler.isAvailable() && enableAEC) {
			aec = AcousticEchoCanceler.create(audioRecord.getAudioSessionId());
			boolean enabled = aec.getEnabled();
			int res = aec.setEnabled(true);
			Log.i("[PV] AudioInAEC", "AcousticEchoCanceler created: " + aec + ", setEnabled res: " + res + ": " + enabled + " -> " + aec.getEnabled());
		}

		if(AutomaticGainControl.isAvailable() && enableAEC) {			
			agc = AutomaticGainControl.create(audioRecord.getAudioSessionId());
			boolean enabled = agc.getEnabled();
			int res = agc.setEnabled(true);
			Log.i("[PV] AudioInAEC", "AutomaticGainControl created: " + agc + ", setEnabled res: " + res + ": " + enabled + " -> " + agc.getEnabled());
		}

		if(NoiseSuppressor.isAvailable() && enableAEC) {
			ns = NoiseSuppressor.create(audioRecord.getAudioSessionId());
			boolean enabled = ns.getEnabled();
			int res = ns.setEnabled(true);
			Log.i("[PV] AudioInAEC", "NoiseSuppressor created: " + ns + ", setEnabled res: " + res + ": " + enabled + " -> " + ns.getEnabled());
		}
		audioRecord.startRecording();
		
        new Thread(new Runnable() {
            @Override
            public void run() {
				Process.setThreadPriority(Process.THREAD_PRIORITY_URGENT_AUDIO);				
                Log.i("[PV] AudioInAEC", "thread start");
				int cntFrame = 0;
				int cntShort = 0;
				
                while (!exit) {
//                    Log.i("++++ AudioInAEC", "read..." + buf.length);
                    int len = audioRecord.read(buf, 0, buf.length);
					cntFrame++;
					cntShort += len;
//                    Log.i("++++ AudioInAEC", "read: " + len + " " + cntFrame + " " + cntShort + " " + buf[0] + "," + buf[1] + "," + buf[2] + "," + buf[3] + "," + buf[4] + "," + buf[5]);
//					Log.i("++++ AudioInAEC", "read: " + len + " " + cntFrame + " " + cntShort);
					pushCallback.OnData();
                }
                audioRecord.stop();
                
				if (aec != null) {
				  aec.release();
				  aec = null;
				}
				if (agc != null) {
				  agc.release();
				  agc = null;
				}
				if (ns != null) {
				  ns.release();
				  ns = null;
				}
				audioRecord.release();
                audioRecord = null;
				
				pushCallback.OnStop();
				Log.i("[PV] AudioInAEC", "thread stop");
            }
        }).start();
        return true;
    }

    public boolean Stop() {
        exit = true;
        return true;
    }

    public boolean AECIsAvailable() {
        return AcousticEchoCanceler.isAvailable();
    }
	
	public boolean AGCIsAvailable() {
        return AutomaticGainControl.isAvailable();
    }
	
	public boolean NSIsAvailable() {
        return NoiseSuppressor.isAvailable();
    }

    public int GetMinBufferSize(int samplingRate, int channels) {
		final int channelConfig = channels == 1 ? AudioFormat.CHANNEL_IN_MONO : AudioFormat.CHANNEL_IN_STEREO;
        return AudioRecord.getMinBufferSize(samplingRate, channelConfig, format);
    }
}
