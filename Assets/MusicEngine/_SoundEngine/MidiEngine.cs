using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LRUCache;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using MidiSheetMusic;
using FMOD;
using System;
using csmidi;

public class SoundHandle {
	public FMOD.Sound sound;
	public long startTick;
	public long length;
	public FMOD.Channel channel;
	public byte[] soundBuffer;
	public bool isMelodyNote;
	public int note;
	public int instrument;
};

public class MidiEngine {
	public const int NOTE_DEFAULT_FADE_OUT_MS = 500;

	private const int MAXIMAL_NOTE_SIZE = 32;
	private const float VOLUME_TIME_UNIT = 0.01f;
	private FMOD.System mFmodSystem;
	private FMOD.CREATESOUNDEXINFO soundExInfo;
	private FMOD.CREATESOUNDEXINFO soundExInfoMp3;
	private FMOD.CREATESOUNDEXINFO soundExInfoPCM;
	private bool isMelodyNote;
	private string dlsFileName;
	private int instrumentId;
	private SoundFont soundFont;
	private SoundFont soundFontOld;
	private const float MS_PER_TICK = 1.0f;//2.0642f;
	//private SoundNote[] fluteNote;

	private List<SoundHandle> soundHandles;
	private List<SoundNote> fluteNotes;
	private SoundHandle soundHandleReady;
	//private List<MidiEvent> midiEvents;
	private List<MidiEvent>[] midiAllEvents;
	private static MidiEngine engine;
	private int noteNumber;
	private int index;
	private int listIndex;

	private MidiFileSystem midiFileSystem;

	private MidiEngine() {
		GCHandle gch = GCHandle.Alloc(this);
		mFmodSystem = new FMOD.System ();
		mFmodSystem.handle = GCHandle.ToIntPtr(gch);
		dlsFileName  = "";
		instrumentId = -1;

		soundFont = null;
		soundFontOld = null;

		soundHandles = new List<SoundHandle>();
		fluteNotes = new List<SoundNote> ();
		//midiEvents = new List<MidiEvent> ();
		midiAllEvents = new List<MidiEvent>[32];
		for (int i = 0; i < midiAllEvents.Length; i++) {
			midiAllEvents [i] = new List<MidiEvent>();
		}
		index = 0;
	}


	~MidiEngine() {

		//releaseEngine();
	//	if(midiExtension)
	//		delete midiExtension;

		if(soundFont != null) {
		//	delete soundFont;
			soundFont = null;
		}
		if(soundFontOld != null) {
		//	delete soundFontold;
			soundFontOld = null;
		}

	}

	public static MidiEngine instance() {
		if (engine == null) {
			engine = new MidiEngine ();
			engine.initEngine ();
		}
		return engine;
	}

	private void initEngine()  {
		
		soundExInfo = new FMOD.CREATESOUNDEXINFO ();

#if UNITY_ANDROID
		//Debug.Log("Android");
		string dlsname =  "/mnt/sdcard/xiaimg.tad";
		soundExInfo.dlsname = Marshal.StringToHGlobalAuto(dlsname);
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
		 
#else
		//string dlsname =  Application.streamingAssetsPath + "/xiaimg.tad";
		//UnityEngine.Debug.Log("midi dls name: " + dlsname);
		// soundExInfo.dlsname = Marshal.StringToHGlobalAuto(dlsname);
#endif
		dlsFileName = "xiaimg";
		UnityEngine.Debug.Log("midi dls name: " + dlsFileName);
		soundExInfo.dlsname = Marshal.StringToHGlobalAuto(dlsFileName);
		UnityEngine.Debug.Log("midi dls name: -2 " + dlsFileName);
		soundExInfo.cbsize =  Marshal.SizeOf(soundExInfo);
		soundExInfo.suggestedsoundtype = FMOD.SOUND_TYPE.MIDI;

		// set up DLS file
		soundExInfoMp3.cbsize =  Marshal.SizeOf(soundExInfoMp3);

		soundExInfoPCM.cbsize =  Marshal.SizeOf(soundExInfoPCM);
		soundExInfoPCM.defaultfrequency = 44100;
		soundExInfoPCM.numsubsounds = 2;
		soundExInfoPCM.numchannels = 1;
		soundExInfoPCM.format = FMOD.SOUND_FORMAT.PCM16;

		/*
     Create a System object and initialize
     */    

		FMOD.RESULT res = FMOD.Factory.System_Create (out mFmodSystem);
		UnityEngine.Debug.Log("FMOD.Factory.System_Create" + res);
		if (res != FMOD.RESULT.OK) {
			UnityEngine.Debug.Log("FMOD.Factory.System_Create Failed");
			return;
		}
		MidiFileSystem.setupMidiFileSystem(mFmodSystem);
		res = mFmodSystem.init(32, FMOD.INITFLAGS.NORMAL, (System.IntPtr) 0);
		if(res != FMOD.RESULT.OK) {
			if (mFmodSystem.hasHandle())
			{
				mFmodSystem.release();
				mFmodSystem.clearHandle ();
			}   
		}

		
	}


	public void update() 
	{
		if(mFmodSystem.hasHandle()) {
			mFmodSystem.update();
		}
	}


	private SoundHandle findAvailableSoundHandle() {
		SoundHandle soundHandle;
		if (soundHandles.Count >= MAXIMAL_NOTE_SIZE) {
			soundHandle = soundHandles [0];
			if (soundHandle.sound.hasHandle()) {
				soundHandle.sound.release ();
				soundHandle.sound.clearHandle ();
			}
			//free(mSoundHandle[soundIndex]->soundBuffer);
			soundHandles.RemoveAt (0);
			soundHandle.sound.clearHandle ();
			soundHandle.channel.clearHandle ();
		} else {
			soundHandle = new SoundHandle ();
		}
		if (soundHandleReady == soundHandle) {
			soundHandleReady = null;
		}
		return soundHandle;
	}

	/*
	bool playSoundFile(string soundFile) {
		if(mFmodSystem == null)
			return false;

		int index = 0;
		index = findAvaibleSoundPool();

		int result = 0;
		soundExInfo.length = 0;
		result = mFmodSystem->createSound(soundFile , FMOD_SOFTWARE , &soundExInfoMp3, &mSoundHandle[index]->sound);

		if(result != FMOD_OK) {
			return false;
		}

		result = mSoundHandle[index]->sound->setMode(FMOD_LOOP_OFF);


		result = mFmodSystem->playSound(FMOD_CHANNEL_FREE, mSoundHandle[index]->sound, false, &(mSoundHandle[index]->channel));

		return true;
	}

	public void playMidiNoteWithFile(int note ,int velocity, int deltaTime, string file) {
		if(file) {
			playSoundMidi(file);
		}

	} 

bool playSoundMidi(string midiFile)
{
    if(!mFmodSystem) 
        return false;
       
    int index = 0;
    index = findAvaibleSoundPool();
    
    int result = 0;
    soundExInfo.length = 0;
    result = mFmodSystem->createSound(midiFile , FMOD_SOFTWARE , &soundExInfoMp3, &mSoundHandle[index]->sound);
    
    if(result != FMOD_OK) {
        return false;
    }
    
    result = mSoundHandle[index]->sound->setMode(FMOD_LOOP_OFF);
    

    result = mFmodSystem->playSound(FMOD_CHANNEL_FREE, mSoundHandle[index]->sound, false, &(mSoundHandle[index]->channel));
    
    return true;
}

	bool playSoundStream(byte[] midiStream, int streamSize)
{
    if(!mFmodSystem || !midiStream || streamSize==0) 
        return false;
    int index = 0;
    index = findAvaibleSoundPool();
    
    int result = 0;
    
    soundExInfoMp3.length = streamSize;
    
    
    result = mFmodSystem->createSound(midiStream , FMOD_SOFTWARE | FMOD_OPENMEMORY, &soundExInfoMp3, &mSoundHandle[index]->sound);
    
    if(result != FMOD_OK) {
        return false;
    }
  
    result = mSoundHandle[index]->sound->setMode(FMOD_LOOP_OFF);
    

    result = mFmodSystem->playSound(FMOD_CHANNEL_FREE, mSoundHandle[index]->sound, false, &(mSoundHandle[index]->channel));

   
    //fprintf ( stdout, "playsound: %d ", result);
    //mFmodSystem->update();
    return true;
}
*/
	private SoundHandle prepareSoundMidiStream(byte[] midiStream, int streamSize) {
		if(!mFmodSystem.hasHandle() || midiStream == null || streamSize==0) 
			return null;
		UnityEngine.Debug.Log("prepareSoundMidiStream: streamSize " + streamSize);
		SoundHandle sh = findAvailableSoundHandle();

		FMOD.RESULT result = 0;

		soundExInfo.length = (uint) streamSize;
		UnityEngine.Debug.Log("prepareSoundMidiStream: create sound step 1");
		result = mFmodSystem.createSound(midiStream , FMOD.MODE.OPENMEMORY, ref soundExInfo, out sh.sound);
		UnityEngine.Debug.Log("prepareSoundMidiStream: create sound step 2");
		if (result != FMOD.RESULT.OK) {
            UnityEngine.Debug.Log("prepareSoundMidiStream " + result);
			return null;
		}
		UnityEngine.Debug.Log("prepareSoundMidiStream: create sound successfully");
		soundHandles.Add (sh);
		return sh;
	}
		

	private bool playSound(SoundHandle sh) {
		if (sh != null) {
			FMOD.RESULT result = 0;
			result = sh.sound.setMode (FMOD.MODE.LOOP_OFF);
			FMOD.ChannelGroup group = new FMOD.ChannelGroup();
			result = mFmodSystem.playSound (sh.sound, group, false, out sh.channel);
			UnityEngine.Debug.Log ("playSound " + result);
			if (result != FMOD.RESULT.OK) {
				UnityEngine.Debug.Log ("playSound failed");
				return false;
			}
			return true;
		} else {
			return false;
		}
	}

	private byte[] makeMidiSound(List<MidiEvent>[] events) {
		//string path ="/Users/jzhong/Documents" + "/output.mid";
		//FileStream stream = new FileStream (path, FileMode.Create);
		MemoryStream stream = new MemoryStream();
		MidiFile.output (stream, events, 105);//105);
		return stream.ToArray();

	}

	private byte[] makeMidiSoundEx(List<MidiEvent>[] events) {

		int clks_per_beat = 105;
		int tempo = 1000000/5;
		int return_code = -1;
		int chan, // internal midi channel number 0...15 (named 1...16)
		val;
		chan = 0;
		//MIDIClockTime dt = 50; // time interval (1 second)

		List<MidiTrackEx> tracks = new List<MidiTrackEx>();
		//tracks.SetClksPerBeat( clks_per_beat );
		for (int n = 0; n < events.Length; n++) {
			MidiTrackEx track = new MidiTrackEx();
			tracks.Add (track);
			List<MidiEvent> listEvent = events [n];
			listEvent.Sort (delegate (MidiEvent e1, MidiEvent e2){
				return e1.DeltaTime - e2.DeltaTime;
			});
			if(n>=9) {
				chan = n+1;
			} else {
				chan = n;
			}
			//sete temp and time signature info.
			byte numerator = 4;
			byte denominator_power = 4;
			byte midi_clocks_per_metronome = 24;
			byte num_32nd_per_midi_quarter_note = 8;
			byte denominator = (byte) (1 << denominator_power);
			// forward to msg denominator instead denominator power
			// set numerator in msg byte2, denominator in byte3
			byte[] data = new byte[5];
			int k = 0;
			data[k++] = numerator;
			data[k++] = denominator;
			data[k++] = denominator_power; // also add original denominator power in byte4
			data[k++] = midi_clocks_per_metronome;
			data[k++] = num_32nd_per_midi_quarter_note;

			MetaMidiEvent timeSignature = new MetaMidiEvent (0L, 0x58, data); //MetaType.TimeSignature
			//byte[] inn = timeSignature.getEventData();
			track.midiEvents.Add (timeSignature);
			//set volume
			//m.SetController(C_MAIN_VOLUME);
			//m.SetControllerValue(volumes[i]);

			//set tempo
			data = new byte[4];
			data[3] = (byte) (tempo & 0xFF);
			data[2] = (byte) ((tempo >> 8) & 0xFF);
			data[1] = (byte) ((tempo >> 16) & 0xFF);
			data [0] = 03;
			MetaMidiEvent tempoEvent = new MetaMidiEvent (0L, 0x51, data); // MetaType.TempoSetting
			//inn = tempoEvent.getEventData();
			track.midiEvents.Add (tempoEvent);
			//
			if(listEvent[0].Channel == 9) {
				chan = 9;
				MessageMidiEvent msgMidi = new MessageMidiEvent (0, (byte) chan, NormalType.Controller, 0x20, 0);
				track.midiEvents.Add (msgMidi);
			} else {
				MessageMidiEvent msgMidi = new MessageMidiEvent (0, (byte) chan, NormalType.Program, listEvent[0].Instrument, 0);
				track.midiEvents.Add (msgMidi);
			//	inn = msgMidi.getEventData();
			}

			for (int i = 0; i < listEvent.Count; i++) {
				MidiEvent e = listEvent [i];
				MessageMidiEvent msgMidi = new MessageMidiEvent (e.DeltaTime, e.Channel, 
					                           e.EventFlag == MidiFile.EventNoteOn ? NormalType.NoteON : NormalType.NoteOFF, e.Notenumber, e.Velocity);
				track.midiEvents.Add (msgMidi);
			//	inn = msgMidi.getEventData();
			}
		}
		MidiFileEx midiFile = new MidiFileEx ();
		midiFile.midiTracks.AddRange(tracks);
		midiFile.timeDivision = (ushort) clks_per_beat;
		//midiFile.saveMidiToFile (Application.persistentDataPath + "/" + "test.mid");
		UnityEngine.Debug.Log("makeMidiSoundEx: create sound ");
		return midiFile.saveMidiToStream ();


	}


	public void playMidiNote(int note ,int velocity, int deltaTime, int instrument, int channel) {


		List<MidiEvent>[] events = new List<MidiEvent>[1];
		List<MidiEvent> eventList = new List<MidiEvent> ();
		events [0] = eventList;
		int tick = 0;
		MidiEvent midiEvent = new MidiEvent ();
		midiEvent.EventFlag = MidiFile.EventNoteOn;
		midiEvent.Instrument = (byte) instrument;
		midiEvent.Channel = (byte) channel;
		midiEvent.Notenumber = (byte) note; 
		midiEvent.Velocity = (byte) velocity;
		//midiEvent.StartTime = tick;
		midiEvent.DeltaTime = (int) (tick/MS_PER_TICK);
		eventList.Add (midiEvent);

		midiEvent = new MidiEvent ();
		midiEvent.EventFlag = MidiFile.EventNoteOff;
		midiEvent.Instrument = (byte) instrument;
		midiEvent.Channel = (byte) channel;
		midiEvent.Notenumber = (byte) note;
		midiEvent.Velocity = (byte) velocity;
		midiEvent.DeltaTime =(int) (deltaTime/MS_PER_TICK);
		eventList.Add (midiEvent);

		byte[] soundData = makeMidiSoundEx (events);
		if (soundData == null) {
			return;
		}
		SoundHandle sh = prepareSoundMidiStream (soundData, soundData.Length);
		if (sh != null)
		{
			UnityEngine.Debug.Log("playMidiNote ---- ");
			playSound(sh);
		} else
        {
			UnityEngine.Debug.Log("prepareSoundMidiStream: return null ");
		}
	}


	public void addNoteBegin() {
		isMelodyNote = false;
		//midiEvents.Clear ();
		noteNumber = 0;
		listIndex = 0;
		for (int i = 0; i < midiAllEvents.Length; i++) {
			List<MidiEvent> list = midiAllEvents [i];
			list.Clear ();
		}
	}

	public void addNote(int tick, int note, int velocity, int instrument, long duration, int channel, int volume, bool isMelodyNote_) {
		if (channel >= 32) {
			return;
		}
		if(isMelodyNote_) {
			isMelodyNote = isMelodyNote_;
		}
			
		List<MidiEvent> midiEvents = midiAllEvents [channel];

		MidiEvent midiEvent = new MidiEvent ();
		midiEvent.EventFlag = MidiFile.EventNoteOn;
		if (channel == 9) {
			midiEvent.Instrument = 0;
		} else {
			midiEvent.Instrument = (byte)instrument;
		}
		midiEvent.Channel = (byte) channel;
		midiEvent.Notenumber = (byte) note; 
		midiEvent.Velocity = (byte) velocity;
		//midiEvent.StartTime = tick;
		midiEvent.DeltaTime = (int) (tick/MS_PER_TICK);
		midiEvents.Add (midiEvent);

		midiEvent = new MidiEvent ();
		midiEvent.EventFlag = MidiFile.EventNoteOff;
		if (channel == 9) {
			midiEvent.Instrument = 0;
		} else {
			midiEvent.Instrument = (byte)instrument;
		}
		midiEvent.Channel = (byte) channel;
		midiEvent.Notenumber = (byte) note;
		midiEvent.Velocity = (byte) velocity;
		midiEvent.DeltaTime =(int) ((tick + duration)/MS_PER_TICK);
		midiEvents.Add (midiEvent);
		noteNumber++;

	}

	public void prepareSoundAllNotes() {
		int size = 0;
		for (int i = 0; i < midiAllEvents.Length; i++) {
			List<MidiEvent> list = midiAllEvents [i];
			if (list.Count > 0)
				size++;
		}
		List<MidiEvent>[] events = new List<MidiEvent>[size];
		size = 0;
		for (int i = 0; i < midiAllEvents.Length; i++) {
			List<MidiEvent> list = midiAllEvents [i];
			if (list.Count > 0)
				events[size++] = list;
		}

		byte[] soundData = makeMidiSoundEx (events);
		if (soundData == null) {
			return;
		}
		SoundHandle sh = prepareSoundMidiStream (soundData, soundData.Length);
		soundHandleReady = sh;
		//UnityEngine.Debug.Log ("prepareSoundAllNotes-------------" + (index++) + " num - "+ noteNumber);

	}

	public void playReadySound() {
		
		playSound (soundHandleReady);
		soundHandleReady = null;
	}

	public bool isReadySoundAvailable() {
		return soundHandleReady!=null;
	}

	public void releaseReadySound() {
		soundHandleReady = null;
	}


	public void playAllNotes() {
		prepareSoundAllNotes ();
		playSound(soundHandleReady);
	}
	/*
	public void stopMelodyNote() {
		int i= 0;
		for(;i<SOUNDS_BUFFER_SIZE;i++) {

			if(mSoundHandle[soundIndex]->sound && mSoundHandle[soundIndex]->isMelodyNote) {
				mSoundHandle[soundIndex]->sound->release();
				if(mSoundHandle[soundIndex]->soundBuffer) {
					free(mSoundHandle[soundIndex]->soundBuffer);
				}
			}
		}
	}
*/

	public void releaseEngine()
	{
		int i= 0;
		for(;i<soundHandles.Count ;i++) {
			SoundHandle sh = soundHandles [i];
			if(sh.sound.hasHandle()) {
				sh.sound.release();
				sh.sound.clearHandle ();
			}
		}
		for(;i<fluteNotes.Count ;i++) {
			SoundNote sn = fluteNotes [i];
			sn.stopWave (0, 0, 0);
		}
		soundHandles.Clear ();
		fluteNotes.Clear ();
		if (mFmodSystem.hasHandle())
		{
			mFmodSystem.release();
			mFmodSystem.clearHandle ();
		}   

	}


	private SoundNote findAvaiableFluteNote()
	{
		//clear unused note
		for(int i = fluteNotes.Count - 1; i>=0; i--) {
			SoundNote sn = fluteNotes [i];
			if(sn.isFinishPlaying()) {
				sn.releaseSound ();
				fluteNotes.RemoveAt (i);
			}
		}

		SoundNote soundNote = new SoundNote();
		return soundNote;

	}

	public void setBaseVolume(float baseVolume) {

		for(int i=0; i<fluteNotes.Count; i++) {
			SoundNote sn = fluteNotes [i];
			sn.setBaseVolume(baseVolume);
		}
	}


	public void changeVolumeBySpeed(float speed)  // speed (- 100.0 ~ 100.0);
	{
		float oldVolume;
		float newVolume;
		for(int i=0; i<fluteNotes.Count; i++) {
			SoundNote sn = fluteNotes [i];
			oldVolume = sn.getBaseVolume();
			newVolume = oldVolume + speed * VOLUME_TIME_UNIT;
			sn.setBaseVolume(newVolume);
		}

	}

	int fluteIndex = 0;

	public bool playNoteFlute(int instrumentId_, int note, string folderInstrument, long duration, float volume) {
		//stopAllNoteFlute();
		//UnityEngine.Debug.Log ("playNoteFlute-------------" + (fluteIndex++) + " duration - "+ duration);
		if(instrumentId != instrumentId_ || soundFont == null) {
			instrumentId = instrumentId_;
			soundFontOld = null;
			if (soundFont != null) {
				soundFontOld = soundFont;
				soundFont = null;
			} else {
				soundFontOld = null;
			}
			soundFont = new SoundFont();
			soundFont.loadSoundFont(instrumentId, folderInstrument);
		}

		SoundNote sn = findAvaiableFluteNote();

		if(soundFont.prepareNoteWithInstrument(sn, note)) {
			sn.setNote(note, duration);
			sn.playWave(mFmodSystem, note, duration, volume);
		}
		fluteNotes.Add (sn);

		return true;

	}

	public bool noteUpdateFlute(float dt) {
		mFmodSystem.update();

		for(int i=0; i<fluteNotes.Count; i++) {
			SoundNote sn = fluteNotes [i];
			sn.update(dt);
		}
		return true;
	}


	public bool stopNoteFlute(int note, int atLeastMS_,  int noteLastTimeMs_) {
		//find current avaiable note
		for(int i=0; i<fluteNotes.Count; i++) {
			SoundNote sn = fluteNotes [i];
			if (sn.currentNote == note) {
				sn.stopWave (NOTE_DEFAULT_FADE_OUT_MS, atLeastMS_, noteLastTimeMs_);
			}
		}
		soundFontOld = null;
		return true;
	}

	int stopFluteIndex = 0;
	public bool stopAllNoteFlute(int atLeastMS_, int noteLastTimeMs_) {
		//UnityEngine.Debug.Log ("stopAllNoteFlute-------------" + (stopFluteIndex++) + " atLeastMS_ - "+ atLeastMS_);
		for(int i=0; i<fluteNotes.Count; i++) {
			SoundNote sn = fluteNotes [i];
			sn.stopWave (NOTE_DEFAULT_FADE_OUT_MS, atLeastMS_, noteLastTimeMs_);
		}
		soundFontOld = null;
		return true;
	}


	


}
