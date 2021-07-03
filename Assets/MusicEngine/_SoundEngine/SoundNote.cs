using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using System;
using System.Runtime.InteropServices;


public class SoundNote {
	public const int NOTE_DEFAULT_FADE_OUT_MS = 500;

	public int currentNote;
	public int baseNote;
	public int startNote;
	public int endNote;
	public int loopStart;
	public int loopEnd;



	private int currentReadPos;
	private int direction;
	private WaveFormat wavFormat;
	private byte[] dataNote;



	private long durationDataLen;

	private long  noteLastTimeLen;
	private int absolutePos;
	private uint currentPosMs;
	private long durationMS;

	private int copyDataLen;


	private float channelWaveVolume;


	private int fadeOutTimeMS;

	private bool stopped;
	private bool isFadeOut;
	private float baseVolume;

	private long startLowVolumeMs;
	private float lowVolume;



	//sound engine
	FMOD.System mFmodSystem;

	FMOD.Channel channelWave;
	FMOD.Sound soundWave;
	FMOD.DSP dspPitch;
	FMOD.CREATESOUNDEXINFO soundExInfoWave;



	public float getBaseVolume() {
		return baseVolume;
	}


	public int getChannels() { return wavFormat.channels; }



	public SoundNote() {
		currentReadPos = 0;
		direction = 1;
		absolutePos = 0;
		soundWave.clearHandle ();
		channelWave.clearHandle ();
		//    waveFile = NULL;
		dspPitch.clearHandle ();
		isFadeOut = false;
		baseVolume = 1.0f;
		soundExInfoWave = new FMOD.CREATESOUNDEXINFO ();

	}

	~SoundNote() {
		//don't need to free dataNote in the future. it pass into from outside
		/*  if(dataNote) {
        delete dataNote;
    } */

		//    if(waveFile) {
		//        delete waveFile;
		//    }
		stopWave(0, 0, 0);
		if(dspPitch.hasHandle()) {
			dspPitch.release();
			dspPitch.clearHandle();
		}
		//Marshal.DestroyStructure (soundExInfoWave.userdata, typeof(UserData));
	}

	public bool init(byte[] dataNote_, WaveFormat wavFormat_) {
		if(dataNote_ == null || wavFormat_ == null) {
			return false;
		}
		baseVolume = 1.0f;
		dataNote = dataNote_;
		wavFormat = wavFormat_;
		return true;
	}

	public bool setNote(int note, long durationMs_) {
		currentNote = note;
		/* if(note >= startNote && note <= endNote) {
        return true;
    } else {
        return false;
    } */
		durationDataLen = (durationMs_ * (long)  wavFormat.samplerate / 1000) ;
		noteLastTimeLen = durationDataLen;
		durationMS = durationMs_;
		baseVolume = 0.4f;//1.0;
		return true;
	}

	public void setBaseVolume(float baseVolume_) {
		if(baseVolume_ > 1.0) {
			baseVolume = 1.0f;
		} else if(baseVolume_ < 0) {
			baseVolume = 0;
		} else {
			baseVolume = baseVolume_;
		}
		if(channelWave.hasHandle()) {
			channelWave.setVolume(channelWaveVolume*baseVolume);
		}
	}

	public bool isFinishPlaying() {
		//first filter the status of not playing
		if(channelWave.hasHandle()) {
			/*    bool isPlaying = false;
        channelWave->isPlaying(&isPlaying);
        if(!isPlaying) {
            return true;
        } */
			return false;
		} else {
			return true;
		}


	}

	public bool isPlayExceedDuraton() {
		//if playing, check the position.
		/*  if(absolutePos > durationDataLen) {
        return true;
    } else {
        return false;
    } */

		return true;
	}

	public void setPosition(long position) {
		currentReadPos = (int) position;
		//absolutePos = position;
	}

	private int readDataForRealTimePlaying(IntPtr data, int datalen) {
		int readSize = 0;

		//short[] stereo16bitbuffer = (short[])data;
		//short[] dataWav16bit = (short[]) dataNote;

		//Marshal.Copy(readbuffer, 0, buffer, (int)sizebytes);

		//  int loopStart = 26511;//44100/4;
		//  int loopEnd = 38021;

		//flute79： 36039 - loop：25429-36007
		// int loopStart = 25429;//44100/4;
		// int loopEnd = 36007;

		//flute61： 36860 - loop：22365-36828
		//int loopStart = 22365;//44100/4;
		//int loopEnd = 36828;

		/*   for (readSize = 0; readSize < (datalen /2 ); )        // >>2 = 16bit stereo (4 bytes per sample)
    {
        if(currentReadPos >= loopEnd) {
            //direction = -1;
            currentReadPos = loopStart;
            direction = 1;
            //printf("direction  -1");
        }  else if(currentReadPos <= loopStart && direction <= -1)  {
            currentReadPos = loopStart;
            direction = 1;
          //  printf("direction  1");
        } 
        dataWav16bit = (signed short*) dataNote;
        dataWav16bit += currentReadPos;
        *stereo16bitbuffer++ = *dataWav16bit;
        //*stereo16bitbuffer++ = *(dataWav16bit);
        currentReadPos += direction;
        absolutePos++;
        readSize += 1;
    } 
    return readSize;
   */

		copyDataLen = 0;
		byte[] dataOut = new byte[datalen];
		copyBuffers(dataOut, 0, dataNote, (int) datalen);
		Marshal.Copy(dataOut, 0, data, (int) datalen);
		absolutePos += copyDataLen;

		return copyDataLen; 

	}

	private int copyBuffers(byte[] dataOut, int dataOutOffset,  byte[] dataIn,  int datalen) {
		if(datalen==0) {
			return 0;
		}

		//short[] stereo16bitbuffer = (short[])dataOut;
		//short[] dataWav16bit = (short[]) dataIn;

		int copyLenTemp  = 0;


		if(loopEnd>0) {

			if( datalen + currentReadPos < loopEnd) {
				Buffer.BlockCopy(dataIn,currentReadPos, dataOut, dataOutOffset, sizeof(byte) * datalen);
				currentReadPos += datalen;
				copyLenTemp = datalen;
				copyDataLen += copyLenTemp;
			} else  {
				copyLenTemp = loopEnd - currentReadPos;
				if(copyLenTemp > 0) {
					Buffer.BlockCopy(dataIn,currentReadPos, dataOut, dataOutOffset, sizeof(byte) * copyLenTemp);
				}
				copyDataLen += copyLenTemp;
				int leftBytes = datalen - copyLenTemp;
				currentReadPos = loopStart;
				copyBuffers(dataOut, dataOutOffset + copyLenTemp, dataIn , leftBytes);

			}
		} else {

			if( datalen + currentReadPos <  (int) wavFormat.data_size) {
				Buffer.BlockCopy(dataIn,currentReadPos, dataOut, dataOutOffset, sizeof(byte) * datalen);
				currentReadPos += datalen;
				copyLenTemp = datalen;
				copyDataLen += copyLenTemp;
			} else  {
				copyLenTemp = ((int) wavFormat.data_size) - currentReadPos;
				if(copyLenTemp > 0) {
					Buffer.BlockCopy(dataIn,currentReadPos, dataOut, dataOutOffset, sizeof(byte) * copyLenTemp);
				}
				copyDataLen += copyLenTemp;
				if(copyDataLen< datalen) {
					//   printf("--out of wave length: %d", datalen - copyDataLen);
					//!!!!!! memset((void*) (stereo16bitbuffer + copyLenTemp ), 0, (datalen - copyDataLen)*2);
					copyDataLen = datalen;
				}
			}

		}

		return copyDataLen;

	}

	private int getSampleRate() {
		int sampleRate = (int) wavFormat.samplerate;


		float pitchShift = 1.0f;
		int gapNote = currentNote - baseNote;
		if(gapNote > 0 && gapNote < 32) {
			pitchShift = (float) (pitchShift * Math.Pow(1.05946, gapNote));// (1 + 0.05946 * gapNote);
		} else if(gapNote < 0 && gapNote > -32) {
			pitchShift = (float) (pitchShift * Math.Pow(0.9438, -gapNote));//(1 + 0.0562 * gapNote);
		}

		return (int) (sampleRate * pitchShift) ;
	}

	private float getPitchShiftDelta() {
		/*  float pitchShift = 1.0f;
    int gapNote = currentNote - baseNote;
    if(gapNote > 0 && gapNote < 12) {
        pitchShift = pitchShift * pow(1.05946, gapNote);// (1 + 0.05946 * gapNote);
    } else if(gapNote < 0 && gapNote > -12) {
        pitchShift = pitchShift * pow(0.9438, -gapNote);//(1 + 0.0562 * gapNote);
    }
    return pitchShift; */
		return 1.0f;
	}




	//public delegate RESULT SOUND_PCMREADCALLBACK    (IntPtr soundraw, IntPtr data, uint datalen);
	[AOT.MonoPInvokeCallback(typeof(FMOD.SOUND_PCMREAD_CALLBACK))]
	public static FMOD.RESULT PCMREADCALLBACK    (IntPtr soundraw, IntPtr data, uint datalen)
	{
		
		FMOD.Sound sound = new FMOD.Sound();
		sound.handle = soundraw;
		IntPtr userdata;
		sound.getUserData (out userdata);
		//UserData ud = (UserData) Marshal.PtrToStructure (userdata, typeof(UserData));
		GCHandle gch = GCHandle.FromIntPtr(userdata);
		SoundNote noteEngine = (SoundNote) gch.Target;

		noteEngine.readDataForRealTimePlaying(data, (int) datalen);
		// Populate readbuffer here with raw data
		//Marshal.Copy(readbuffer, 0, buffer, (int)sizebytes);
		return FMOD.RESULT.OK;
	}


	[AOT.MonoPInvokeCallback(typeof(FMOD.SOUND_PCMSETPOS_CALLBACK))]
	public static FMOD.RESULT PCMSETPOSCALLBACK  (IntPtr soundraw, int subsound, uint position, TIMEUNIT postype)
	{
		FMOD.Sound sound = new FMOD.Sound();
		sound.handle = soundraw;
		IntPtr userdata;
		sound.getUserData (out userdata);
		//UserData ud = (UserData) Marshal.PtrToStructure (userdata, typeof(UserData));
		GCHandle gch = GCHandle.FromIntPtr(userdata);
		SoundNote noteEngine = (SoundNote) gch.Target;

		noteEngine.setPosition (position);
		return FMOD.RESULT.OK;
	}




	public bool playWave(FMOD.System fmodSystem, int note, long duration, float volume) {
		FMOD.RESULT result = FMOD.RESULT.OK;
		mFmodSystem = fmodSystem;
		currentPosMs = 0;
		if(duration < 500) {
			duration = 500;
		}

		stopped = true;
		isFadeOut = false;
		absolutePos = 0;


		if (soundWave.hasHandle())
		{
			channelWave.setPaused(true);

			result = channelWave.stop();
			soundWave.release();
			soundWave.clearHandle ();
		}

		if(dspPitch.hasHandle()) {
			dspPitch.release();
			dspPitch.clearHandle();
		}

		
		setNote(note, duration);
		startLowVolumeMs = 800;
		baseVolume = volume;
		lowVolume = 1.0f;
		//memset(&soundExInfoWave, 0, sizeof(FMOD_CREATESOUNDEXINFO));
		soundExInfoWave.cbsize            = Marshal.SizeOf(soundExInfoWave); //sizeof(FMOD.CREATESOUNDEXINFO);             
		soundExInfoWave.decodebuffersize  = (uint) getSampleRate() *  sizeof(short);//33622;                                       // Chunk size of stream update in samples.  This will be the amount of data passed to the user callback. 
		if( loopEnd == 0) {
			durationMS = (long) (wavFormat.data_size * 1000 * 0.35 / getSampleRate());
			soundExInfoWave.length            =  (uint) (wavFormat.data_size * wavFormat.block_align * 0.8) ;//getSampleRate()* wavFormat.bytes_per_sample *  durationMS /1000;
			//soundExInfoWave.length            = wavFormat.data_size * wavFormat.block_align ;// getSampleRate() * sizeof(signed short) * ( duration /1000 + 4);        // Length of PCM data in bytes of whole song (for Sound::getLength) 
			//   loopStart = 0;
			//   loopEnd = wavFormat.data_size ;
			// durationMS = soundExInfoWave.length * 6  / getSampleRate();
		} else {
			soundExInfoWave.length            = (uint) (getSampleRate() * sizeof(short) * ( duration /1000 + 4));        // Length of PCM data in bytes of whole song (for Sound::getLength) 
		}

		soundExInfoWave.numchannels       = getChannels();                                    // Number of channels in the sound. 
		soundExInfoWave.defaultfrequency  = getSampleRate();//33622;                                       // Default playback rate of sound. 
		soundExInfoWave.format            = FMOD.SOUND_FORMAT.PCM16;                     // Data format of sound. 
		soundExInfoWave.pcmreadcallback   = PCMREADCALLBACK;                             // User callback for reading. 
		soundExInfoWave.pcmsetposcallback = PCMSETPOSCALLBACK;  
		// User callback for seeking. 
		//Marshal.StructureToPtr(userdata, soundExInfoWave.userdata,false);
		GCHandle gch = GCHandle.Alloc(this);
		soundExInfoWave.userdata = GCHandle.ToIntPtr (gch);

		var mode = FMOD.MODE._2D | FMOD.MODE.OPENUSER;
		string name = null;
		//result = mFmodSystem->createSound(NULL, FMOD_2D | FMOD_OPENUSER | FMOD_LOOP_NORMAL | FMOD_SOFTWARE, &soundExInfoWave, &soundWave);
		result = mFmodSystem.createStream (name, mode  , ref soundExInfoWave, out soundWave);
		if(result != FMOD.RESULT.OK) {
			return false;
		}
		ChannelGroup channelGroup = new ChannelGroup();
		result = mFmodSystem.playSound(soundWave, channelGroup, false, out channelWave);
		if(result != FMOD.RESULT.OK) {
			return false;
		}
		stopped = false;
		channelWaveVolume = 1f ;//0.5;
		channelWave.setVolume(channelWaveVolume * baseVolume);

		float dspShiftDelta = getPitchShiftDelta();
		if(dspShiftDelta != 1.0f) {
			if(!dspPitch.hasHandle()) {
				result = mFmodSystem.createDSPByType(FMOD.DSP_TYPE.PITCHSHIFT , out dspPitch);
			}
			dspPitch.setParameterFloat((int) FMOD.DSP_PITCHSHIFT.PITCH, dspShiftDelta );
			//dspPitch->setParameter(FMOD_DSP_PITCHSHIFT_FFTSIZE, 4096);
			result = channelWave.addDSP(0, dspPitch);
		}

		return true;
	}


	public bool stopWave(int fadeOutTimeMS_, int atLeastMs, int noteLastTimeMs_) {
		bool ret = false;
			bool startFadeOut = false;
		if(fadeOutTimeMS_ == 0) {
			releaseSound();
			return true;
		} else if(atLeastMs > 0 || noteLastTimeMs_ > 0) {
			if (!channelWave.hasHandle()) {
				startFadeOut = true;
			} else {
				fadeOutTimeMS = fadeOutTimeMS_;
				uint position = 0;
				channelWave.getPosition (out position, TIMEUNIT.MS);
				int position2 = (int)position;
				if (noteLastTimeMs_ > 0) {
					if (noteLastTimeMs_ < (atLeastMs - position2)) {
						noteLastTimeMs_ = atLeastMs - position2;
					}
					durationMS = currentPosMs + noteLastTimeMs_;
				} else {
					if (position > atLeastMs) {
						startFadeOut = true;
					} else {
						durationMS = atLeastMs - position2;
					}
				}
			}

		} else {
			startFadeOut = true;
		}
		if(startFadeOut) {
			if(channelWave.hasHandle() && !isFadeOut){
				fadeOutTimeMS = fadeOutTimeMS_;
				stopped = false;
				isFadeOut = true;
				channelWaveVolume = 0.25f;
				channelWave.setVolume(channelWaveVolume * baseVolume);
				ret = true;
			}
		}
		return ret;
	}

	public bool releaseSound() {
		fadeOutTimeMS = 0;
		stopped = true;
		isFadeOut = false;
		if(channelWave.hasHandle()) {
			channelWave.stop();
			channelWave.clearHandle();
		}
		if (soundWave.hasHandle())
		{
			soundWave.release();
			soundWave.clearHandle();
		}
		return true;
	}

	public void update(float delta) {
		if (channelWave.hasHandle()) {
			channelWave.getPosition (out currentPosMs, TIMEUNIT.MS);
			//  printf("\n--currentPosMs: %d", currentPosMs);
		}


		if (isFadeOut && channelWave.hasHandle()) {
			if (channelWaveVolume >= 0) {
				channelWave.setVolume (channelWaveVolume * baseVolume);
				channelWaveVolume -= (delta * 1000 * 0.25f / fadeOutTimeMS);
				//channelWave->setVolume(channelWaveVolume);
			} else {
				releaseSound ();
			}
		} else if (channelWave.hasHandle()) {
			if (currentPosMs > durationMS) {
				stopWave (NOTE_DEFAULT_FADE_OUT_MS, 0, 0);
			} else if (currentPosMs > startLowVolumeMs) {
				lowVolume -= delta * 1000 / 2000;
				if (lowVolume < 0.35f) {
					lowVolume = 0.35f;
				}
				channelWave.setVolume (lowVolume * channelWaveVolume * baseVolume);
			}

		} 
	}


}
