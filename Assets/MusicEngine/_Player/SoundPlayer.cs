using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MidiSheetMusic;

public abstract class PianoPlayerDelegate 
{

	public abstract void playNoteStart ();
	public abstract void playNoteEnd () ;
	public abstract void playNoteNotify (MusicNote midiEvent) ;
	public abstract void playComingNoteNotify (MusicNote midiEvent) ;
	public abstract void playEvent (int eventId, long info1, long info2) ;


};

public class SoundPlayer  {

	//public const int DISPLAY_DURATION_IPAD 2500
	public const int DISPLAY_DURATION = 4000;

	public const int TAP_PLAY = 1;
	public const int NON_STOP_TAP_PLAY = 2;
	public const int LEARN_PLAY       = 4;
	public const int HARMONY_PLAY    = 5;
	public const int DEMO_PLAY    = 6;
	public const int SOLO_IDLE = 7;
	public const int SOLO_RECORD_REPLAY = 8;
	public const int SOLO_RECORDING = 9;

	public const int SWIPE_PLAY_DURATION = 2000;

	public const int PLAY_EVENT_TOO_FAST = 1;
	public const int PLAY_EVENT_TOO_SLOW = 2;
	public const int PLAY_EVENT_START    = 3;
	public const int PLAY_EVENT_END      = 4;
	public const int PLAY_EVENT_PLAY_MODE_CHANGE    =  5;
	public const int PLAY_EVENT_PLAY_SCORE     = 6;
	public const int PLAY_EVENT_PERFECT_PLAY   = 7 ;
	public const int PLAY_EVENT_BONUS_SCORE   = 8;
	public const int PLAY_EVENT_FIRE_NOTE   = 9; 
	public const int PLAY_EVENT_MATCH_RATE = 10;
	public const int PLAY_EVENT_COMPLETE  = 11; //the parameter is the accurate percent
	public const int PLAY_EVENT_FIRE_DRUM_NOTE   = 12;
	public const int PLAY_EVENT_FIRE_SOLOPLAY_NOTE   = 13;
	public const int PLAY_EVENT_PROGRESS   = 14;
	public const int PLAY_EVENT_CURRENTTIME = 15;
	public const int PLAY_EVENT_KEY_ERROR = 17;
	public const int PLAY_EVENT_STOP_SOLOPLAY_NOTE   = 18;
	public const int DRUM_HIT_HEAVY = 3;
	public const int DRUM_HIT_MEDIUM = 2;
	public const int DRUM_HIT_LIGHT = 1;


	public const int TIME_TO_COMPLETE_AFTER_PLAY_LAST_NOTE = 2000;

	public const int ACURACY_BASE = 350; //500 ms
	public const int PLAY_NEXT_NOTE_JUMP = 1000; //if next melody note is >1000 far, can't jump to it.

	public const int INTERVAL_SEND_PROGRESS = 1000; //ms 

	public const int DRUM_NOTE_BASS2 = 35; //35 Bass Drum 2
	public const int DRUM_NOTE_BASS1 = 36; //36 Bass Drum 1
	public const int DRUM_NOTE_SNARE_DRUM1 = 38; //38 Snare Drum 1
	public const int DRUM_NOTE_SNARE_DRUM2 = 40; //40 Snare Drum 2

	public const int DRUM_NOTE_RUATION = 360; //ms

	public const int MAX_MELODY_COUNT = 10;


	//sub instrument fmidiEventList
	public int subInstument; // just for play drum. can set the melody instrument.
	//properties
	public PianoPlayerDelegate playerDelegate;
	public long playTime;
	public long currentNoteTime;
	//@property (nonatomic, assign) int instrument;
	public bool isPlayFinished;

	public int minMelodyNote;
	public int maxMelodyNote;


	//private:

	private bool hitNext;
	private double beatTime;
	private long default_clock_per_beat;


	//melody channel
	private int melodyChannel1;
	private int melodyChannel2;


	//for recording. if user don't play more than 2s. then don't elapse time
	private long lastTimeRecordNote;


	//control play
	private int         currentNoteIndex;

	private long lastPlayTime;

	private long lastSendPlayProgressTime;


	private long prepareNextNoteTimeBase;
	public bool isPause; 


	//control coming notes
	private long lastComingEventTime;
	private long lastComingEventStartTime;
	private int lastComingEventIndex;
	private long comingEventDuration;

	//auto play
	private int autoPlayMusic;

	//play speed
	private double playSpeed; 

	//actual play time
	private long actualPlayTime;

	//play scrore 
	private int melodyNumber; //
	private int melodyAcuracySum;

	//play last note;
	private bool isPlayLastNote;

	private List<MusicNote> currentPlayingMidiEventList;


	//for recording midi
	private long lastRecordNoteTick;

	private MidiEngine midiEngine;

	private const int PERFECT_PLAY_STEP  = 10;

	private bool melodyMute = false;

	private int baseNote; //base note

	private bool isRecordingAvaiableForSave_;

	private AppContext appContext;

	public MidiEventMan midiEventMan;

	private long lastNoteTick;
	private long firstMelodyTick;


	private static SoundPlayer soundPlayer;

	public void setMelodyMute(bool muteMelody_) {
		melodyMute = muteMelody_;
	}
	public bool isMelodyMute() {
		return melodyMute;
	}


	int playRateByHitPercent( float hitPercent) {
		//for calc play rate
		if(hitPercent > 0.98) {
			return 5;
		} else if(hitPercent > 0.93) {
			return 4;
		} else if (hitPercent > 0.87) {
			return 3;
		} else if(hitPercent > 0.82) {
			return 2;
		} else {
			return 1;
		} 

	}

	int getDrumNoteFactor(int drumNote) {
		int noteFactor = 0;
		switch (drumNote) {
		case 35: //35 Bass Drum 2
		case 36: //36 Bass Drum 1
			noteFactor = 100;
			break;
		case 37:
			noteFactor = 10;
			break;
		case 38: //38 Snare Drum 1
		case 40: //40 Snare Drum 2
			noteFactor = 50;
			break;
		case 39: //hand clap
			noteFactor = 1;
			break;
		case 41:
		case 43:
		case 45:
		case 47:
		case 48:
		case 50:
			noteFactor = 25;
			break;
		default:
			break;
		}
		return noteFactor;
	}

	public static SoundPlayer singleton() {
		if(soundPlayer == null) {
			soundPlayer = new SoundPlayer();
		}
		return soundPlayer;
	}

	private SoundPlayer() {
		
		isPause = true;
		currentPlayingMidiEventList = null;
		default_clock_per_beat = 300;

		melodyMute = false;
		baseNote = 0;
		isRecordingAvaiableForSave_ = false;
		appContext = AppContext.instance ();
		midiEngine = MidiEngine.instance ();
		midiEventMan = new MidiEventMan();
		resetPlayStatus();
	}


	int getBaseNoteByInstrument(int instrumentId) {
		int baseNote_ = 0;
		if(instrumentId == MusicInstrument.HARMONICA_INSTRUMENT ||
			instrumentId== MusicInstrument.GUITAR_INSTRUMENT ||
			instrumentId == MusicInstrument.ELECTRIC_GUITAR_INSTRMENT ||
			instrumentId == MusicInstrument.SAX_INSTRUMENT) {
			baseNote_ = -12;
		} else if (instrumentId == MusicInstrument.TRUMPET_INSTRUMENT ) {
			baseNote_ = -12-6;
		} else if(instrumentId == MusicInstrument.VIOLIN_INSTRUMENT) {
			//pianoPlayer->baseNote = - 6;
			baseNote_ = 0;
		}else {
			baseNote_ = 0;
		}
		return baseNote_;
	}

	public void adjustBaseNoteByInstrument() {
		int instrumentId = appContext.getInstrument();
		baseNote = getBaseNoteByInstrument(instrumentId);
	}

	public long getTotalDuration() {
		return midiEventMan.duration();
	}

	public long getFirstMelodyTime() {
		return midiEventMan.firstMelodyTick;
	}

	public void loadMusic (string musicFile, bool isUserPlay ,  int melodyChannel_ ) {
		int instrument = appContext.getInstrument() ;
		melodyChannel1 = melodyChannel_ % 100;
		if(melodyChannel_ > 100) {
			melodyChannel2 = (melodyChannel_ % 10000) /100 % 100;
			if(melodyChannel2 == 0) {
				melodyChannel2 = -1;
			}
		} else {
			melodyChannel2 = -1;
		}
		int pitchValue = 0;
		if (melodyChannel_ >= 10000)
		{
			pitchValue = 1;
		}
		else
		{
			pitchValue = 0;
		}

		isPause = true;

		if(musicFile == null && isUserPlay) {
			minMelodyNote = midiEventMan.minMelodyNote;
			maxMelodyNote = midiEventMan.maxMelodyNote;
			lastNoteTick = midiEventMan.lastNoteTick;
			firstMelodyTick = midiEventMan.firstMelodyTick;

		} else {
			midiEventMan.removeAllEvent();
			byte[] filedata = FileReaderUtils.readMidiZipFile (musicFile);
			if (filedata == null) {
				return;
			}
			MidiFile midiFile = new MidiFile (filedata, "");
			List<MidiTrack> tracks = midiFile.Tracks;
			float ms_per_tick = midiFile.Time.Tempo / (1000f*midiFile.Time.Quarter); //microseconds per tick = microseconds per quarter note / ticks per quarter note
			UnityEngine.Debug.Log("ms_per_tick: " + ms_per_tick + " Tempo:" + midiFile.Time.Tempo + " querter:" + midiFile.Time.Quarter);
			for (int i =0; i< tracks.Count; i++) {
				MidiTrack t = tracks [i];
				List<MidiNote> notes = t.Notes;
				for(int j =0; j< notes.Count; j++) {
					MidiNote n = notes [j];
					if (n.Channel == 9) {
						int yyy = 0;
					}

					midiEventMan.insertMidiNote(i, n, melodyChannel1, melodyChannel2, instrument== MusicInstrument.DRUM_INSTRUMENT, pitchValue,  ms_per_tick);
				}
			}
			midiEventMan.sortAllMidiNote ();
			midiEventMan.trimNoSoundAtBeginning();
			//if(appContext.isCurrentFluteStyle()) { always adjust it
			midiEventMan.adjustMelodyNoteLengthByFlute();
			//}
			minMelodyNote = midiEventMan.minMelodyNote;
			maxMelodyNote = midiEventMan.maxMelodyNote;
			lastNoteTick = midiEventMan.lastNoteTick;
			firstMelodyTick = midiEventMan.firstMelodyTick;
		}

		midiEngine.releaseReadySound();
	}


	//for reset outside
	public void seektoCurrent() {
		seekToTime(playTime);
	}

	public void startPlay(bool isResume)
	{

		if(!isResume) {
			resetPlayStatus();
		} else if(isPlayFinished) {
			return;
		}
		isPause = false;
		if(currentNoteIndex == 0) {
			playerDelegate.playEvent(PLAY_EVENT_START, midiEventMan.lastNoteTick, 0);
			isPlayFinished = false;
			isPlayLastNote = false;
		}
	}

	public void pausePlay() {
		/* [timer invalidate];
	timer = nil;
	[UIApplication sharedApplication].idleTimerDisabled = NO;*/
		isPause = true;
		midiEngine.stopAllNoteFlute(0, 0);
	}

	public bool seek(int progress) {
		if(progress > 100 || progress < 0 || midiEventMan.lastNoteTick <=0) {
			return false;
		}
		long newPlayTime = progress * midiEventMan.lastNoteTick/100;
		return seekToTime(newPlayTime);

	}

	public bool seekToTime(long tick) {
		if(midiEventMan.lastNoteTick <=0) {
			return false;
		}
		long newPlayTime = tick;

		midiEngine.releaseReadySound();
		currentPlayingMidiEventList = null;
		playTime = newPlayTime;// - appContext.musicNoteDisplayDuration;
		beatTime = playTime;
		prepareNextNoteTimeBase = newPlayTime - 10;
		lastComingEventStartTime = newPlayTime;
		currentNoteTime = newPlayTime; 
		lastSendPlayProgressTime = 0; //send progress immediately
		lastComingEventTime = 0; //reset to shoot coming note immeidately
		playerDelegate.playEvent(PLAY_EVENT_PROGRESS, (int) (playTime*100/midiEventMan.lastNoteTick), 0);
		return true;
	}

	public int getPlayMode() {
		return autoPlayMusic;
	}

	public void setPlayMode (int autoPlay) {
		autoPlayMusic = autoPlay;
		if(playerDelegate != null) {
			playerDelegate.playEvent(PLAY_EVENT_PLAY_MODE_CHANGE,0,0);
		}
	}
		
	public void timerUpdate(double dt) 
	{
		midiEngine.noteUpdateFlute((float) dt);
		if(isPause) 
			return;
		double deltaTime = dt*1000 ;
		addPlayDeltaTime(deltaTime);
	}

	void resetPlayStatus() {
		playTime = 0;
		currentNoteIndex = 0;
		lastComingEventTime = 0;
		lastComingEventStartTime = 0;
		lastPlayTime = 0;
		playSpeed = 1.0;
		lastComingEventIndex = 0;
		comingEventDuration =  appContext.musicNoteDisplayDuration;
		currentNoteTime = comingEventDuration;

		prepareNextNoteTimeBase = 0;
		actualPlayTime = 0;
		isPlayFinished = false;
		melodyAcuracySum = 0;
		melodyNumber = 0;
		beatTime = 0;
		isPlayLastNote = false;
		midiEngine.releaseReadySound();
		currentPlayingMidiEventList = null;
		lastSendPlayProgressTime = 0;
		lastTimeRecordNote = 0;
		isRecordingAvaiableForSave_ = false;

	}
		
	public void skipPrelude(bool isDelayToPrepare) {
		midiEngine.releaseReadySound();
		if(isDelayToPrepare) {
			if(playTime < midiEventMan.firstMelodyTick - appContext.musicNoteDisplayDuration/2) {
				playTime = midiEventMan.firstMelodyTick - appContext.musicNoteDisplayDuration /2;
				prepareNextNoteTimeBase = midiEventMan.firstMelodyTick - 10;
				currentNoteTime = midiEventMan.firstMelodyTick;
			}
		} else {
			if(playTime < midiEventMan.firstMelodyTick ) {
				playTime = midiEventMan.firstMelodyTick ;
				prepareNextNoteTimeBase = midiEventMan.firstMelodyTick - 10;
				currentNoteTime = midiEventMan.firstMelodyTick;
			}
		}


	}

	private void addPlayDeltaTime (double deltaTime) {
        deltaTime = deltaTime * 1.2;
		double newPlayTime = playTime + deltaTime;

		if(SOLO_IDLE == autoPlayMusic) {
			playTime = (long) newPlayTime;
		} else if(SOLO_RECORDING == autoPlayMusic) {
			if(newPlayTime > lastTimeRecordNote + 2000) {
				playTime = lastTimeRecordNote + 2000;
			} else {
				playTime = (long) newPlayTime;
			}
			midiEventMan.lastNoteTick = playTime;
		}else if(TAP_PLAY == autoPlayMusic) {
			//if not solo play , need to adjust time by currentNote Time
			//if(newPlayTime > currentNoteTime) {
			//	playTime = currentNoteTime;
			//} else {
			//	playTime = (long)newPlayTime;
			//} 
			if (newPlayTime > currentNoteTime)
			{
				playTime = currentNoteTime;
			}
			else
			{
				playTime = (long)newPlayTime;
			}
		}  else
		{
			playTime = (long)newPlayTime;
		} 
			
		actualPlayTime = (long) (actualPlayTime + deltaTime);
		if(actualPlayTime > playTime+2500) {
			actualPlayTime = playTime + 2500;
		}
		//CCLog("play time: %d" , playTime);
		//if(isPlayLastNote)
		if (!isPlayFinished && actualPlayTime > midiEventMan.getLastMelodyEndTick() + 500)
		{
			playComplete();
			return;
		} else if(isPlayLastNote)
        {
			return;
        }

		if (lastComingEventTime == 0) {
			shootComingNotes();
			lastComingEventTime = 200;

		}else if(playTime - lastComingEventTime > 200) {
			//add notification to get coming event;
			shootComingNotes();
			lastComingEventTime = playTime;
		}

		if(autoPlayMusic == SOLO_RECORDING || autoPlayMusic == SOLO_RECORD_REPLAY) {
			playerDelegate.playEvent(PLAY_EVENT_CURRENTTIME, playTime, 0);

		} else if (  playTime - lastSendPlayProgressTime > INTERVAL_SEND_PROGRESS) {
			lastSendPlayProgressTime = playTime;
			if(midiEventMan.lastNoteTick > 0) {
				playerDelegate.playEvent(PLAY_EVENT_PROGRESS, playTime*100/midiEventMan.lastNoteTick, 0);
			} 
		}

		// double runTimeSt = AppContext::getCurrentTimeFloat();
		// long playTimeTmp = playTime;
		bool isHit = false;
		prepareNextNote(1, false);

		if(autoPlayMusic==LEARN_PLAY ||  autoPlayMusic == NON_STOP_TAP_PLAY || autoPlayMusic==HARMONY_PLAY || autoPlayMusic == DEMO_PLAY || autoPlayMusic == SOLO_RECORD_REPLAY) {
			if(currentPlayingMidiEventList == null || currentPlayingMidiEventList.Count == 0) {
				hit(0, true);
				isHit = true;
			} else if(currentPlayingMidiEventList != null && midiEventMan.getStartEventTick(currentPlayingMidiEventList) <=playTime) {
				hit(0, true);
				isHit = true;
			} else {
				isHit = false;
			}
			beatIt();

		} else if(autoPlayMusic == TAP_PLAY ) {
			if(currentPlayingMidiEventList.Count > 0) {
				if(midiEventMan.getStartEventTick(currentPlayingMidiEventList) <=playTime) {
					if(checkNextisAutoPlay()) {
						hit(0, true);
						isHit = true;
					}
				}
			} 
			beatIt();
		} else if(SOLO_IDLE == autoPlayMusic) {
			beatIt();
		}

		//double runTimeEnd = AppContext::getCurrentTimeFloat();

		if(isHit) {
			//    playTime = playTime + (runTimeEnd - runTimeSt) * 1000;
		}

	}



	void beatIt() {

		/*
		if(autoPlayMusic==HARMONY_PLAY || autoPlayMusic == DEMO_PLAY || autoPlayMusic == SOLO_RECORD_REPLAY || autoPlayMusic == SOLO_IDLE) {
			return;
		}

		//return;
		int  clock_per_beat = midiParserEx->clock_per_beat/2;
		if(clock_per_beat == 0) {
			clock_per_beat = default_clock_per_beat;
		}

		if(clock_per_beat < 350 ) {
			clock_per_beat = clock_per_beat * (1 + 400/clock_per_beat);

		} 
		int step2 = playTime / clock_per_beat;
		int step1 = beatTime / clock_per_beat;


		if(step2>step1) {
			//play drum
			beatTime = clock_per_beat*step2;
			if(abs(playTime - beatTime) < 50) {
				shootTempoEvent(beatTime + clock_per_beat * 8);
				if(!alignWithServerTime) {
					//    gameCenter->sendCommand(MP_CMD_ID_SERVER_TIME, playTime, 0);
				}
				//if(step2%2==0)
				//	midiEngine.playMidiNote(35 ,200 ,60 ,0, 9);
			} 

		}*/

	} 

	//in half play mode, check if next note should play auto
	bool checkNextisAutoPlay() {
		//MidiEventSet* eventSet = NULL;

		/*   if(!currentPlayingMidiEventList) {
		//if current playing midiEventList is exist.
		currentPlayingMidiEventList = midiEventMan.getPlayingMidiEventListByTime(playTime);
		} */

		if(currentPlayingMidiEventList.Count == 0) {
			return false;
		}

		return !MidiEventMan.isIncludeMelody (currentPlayingMidiEventList);
	}

	void playComplete() {
		isPlayFinished = true;
		int finalAccuateRate = 100;
		if(melodyNumber>0) {
			finalAccuateRate = melodyAcuracySum/melodyNumber;
		}
		playerDelegate.playEvent(PLAY_EVENT_COMPLETE, finalAccuateRate,melodyNumber);
		resetPlayStatus();
		if(autoPlayMusic == HARMONY_PLAY) {
			//looping for harmony play
			//    startPlay(false);
		}
	}

	/*
	int getLastPlayMelody(int lastPlayMelodyNotes_[]) {
		memcpy(lastPlayMelodyNotes_, lastPlayMelodyNotes, lastPlayMelodyCount* sizeof(int));
		return lastPlayMelodyCount;
	} */

	int playReadyNote() {
	//	lastPlayMelodyCount = 0;

		List<MusicNote> midiEventList = currentPlayingMidiEventList;
		if(midiEventList == null || midiEventList.Count == 0) {
			return 0;
		}

		bool isIncludeMelody =  MidiEventMan.isIncludeMelody(midiEventList);
		bool isIncludeSubMelody = false;
		//make the note explosed
		int index = 0;
		int drumFactorTotal = 0;
		int noteNumber = 0;
		int mainInsturment = appContext.getInstrument();
		playerDelegate.playNoteStart();
		//fire the meldoy event
		MusicNote musicNote;
		while(true) {
			if (index >= midiEventList.Count)
				break;
			musicNote = midiEventList[index++];
			int note = musicNote.value;
			bool isMelodyNote = musicNote.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE || musicNote.melodyEventEx == MusicNote.PIANO_MELODY_NOTE;
			int instrument = musicNote.instrument;

			if(isMelodyNote) {
			//	if(lastPlayMelodyCount< MAX_MELODY_COUNT) {
			//		lastPlayMelodyNotes[lastPlayMelodyCount++] = note;
			//	}
				note = note + baseNote;
				instrument = mainInsturment;
			}

			if(appContext.getInstrument() == MusicInstrument.DRUM_INSTRUMENT) {
				//if drum is main instrument. fire the melody as well
				if(musicNote.channel == melodyChannel1 ||
					(melodyChannel2 > 0 && musicNote.channel == melodyChannel2)) {
					isMelodyNote = true;
					isIncludeSubMelody = true;
				}
			}


			playerDelegate.playNoteNotify(musicNote);

			//playerDelegate.playNoteNotify(note,instrument,velocity, musicNote.tick, musicNote.melodyEventEx, 
			//	musicNote.tickGapNext,musicNote.channel );
			if(musicNote.channel==9) {
				drumFactorTotal += getDrumNoteFactor(musicNote.value);
			}
			if(autoPlayMusic==SOLO_RECORD_REPLAY) {
				playerDelegate.playEvent(PLAY_EVENT_FIRE_SOLOPLAY_NOTE, note,0);
			}
			noteNumber++;

		}

		//play the ready sound which was prepare last time.
		midiEngine.playReadySound();
		midiEngine.releaseReadySound();
		//UnityEngine.Debug.Log("playReadySound: ============================" + playTime);
		lastPlayTime = midiEventList [0].tick;//getStartEventTick();

		int melodyInstrument = appContext.getInstrumentMelody();
		if((
			(isIncludeMelody && appContext.isCurrentSoundBank() && !melodyMute)   // for non-drum
			|| 
			(isIncludeSubMelody && AppContext.isSoundBank(melodyInstrument) && mainInsturment == MusicInstrument.DRUM_INSTRUMENT)
		) 

		) {
			

			index = 0;
			int note = -1;
			int velocity = 80;
			int duration = 0;
			int channel;
			bool isMelodyNote = false;
			if(appContext.isWindInstrument() 
				|| (AppContext.isWindInstrument(melodyInstrument) && mainInsturment == MusicInstrument.DRUM_INSTRUMENT)) {
				//is Flute style, find the high picth melody note then play
				while(index < midiEventList.Count) {
					musicNote = midiEventList[index++];

					isMelodyNote = musicNote.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE || musicNote.melodyEventEx == MusicNote.PIANO_MELODY_NOTE;


					if(musicNote.channel == 9) {
						isMelodyNote = false;
					} else if(mainInsturment == MusicInstrument.DRUM_INSTRUMENT) {
						if(musicNote.channel == melodyChannel1 ||
							(melodyChannel2 > 0 && musicNote.channel == melodyChannel2)) {
							isMelodyNote = true;
						}
					}

					if(isMelodyNote && musicNote.value > note) {
						//find maximal note
						note = musicNote.value + baseNote;
						velocity = 255;//musicNote.velocity;
						duration =  musicNote.getDuration(true);
						channel = musicNote.channel;

					}

				}
				if(note > 0) {
					//play sound bank
					midiEngine.stopAllNoteFlute(0, 0);
					int instrumentNew = mainInsturment;
					int newNote = note;
					if(mainInsturment == MusicInstrument.DRUM_INSTRUMENT) {
						instrumentNew = melodyInstrument;
						newNote += getBaseNoteByInstrument(melodyInstrument);
					} 
					if(melodyMute && mainInsturment != MusicInstrument.DRUM_INSTRUMENT) {

					} else if(!isMelodyMute()){
						playNote(note, instrumentNew, velocity, duration, false);
					}

					/*	if(isChannelMute(channel)) {
			
						} else {
							
						} */

				}

			} else if(mainInsturment != MusicInstrument.DRUM_INSTRUMENT) {
				//is not flute style, play all note
				while(true) {
					musicNote = midiEventList[index++];
					isMelodyNote = musicNote.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE || musicNote.melodyEventEx == MusicNote.PIANO_MELODY_NOTE;

					if(isMelodyNote) {
						//find maximal note
						note = musicNote.value + baseNote;
						velocity = musicNote.velocity;
						duration =  musicNote.getDuration(false);
						if(melodyMute) {

						} else {
							playNote(note, mainInsturment, velocity, duration, false);
						}
					}

				}
			}
		} 


		int matchRate = 0;
		if(isIncludeMelody) {




			if(actualPlayTime > currentNoteTime) {
				//play too late
				if(Math.Abs((float) (actualPlayTime - currentNoteTime)) < ACURACY_BASE) {
					matchRate = (int) (100 - Math.Abs((float)actualPlayTime - currentNoteTime)*100/ACURACY_BASE);
				} else {
					matchRate = - 101;
				}
			} else {
				if(Math.Abs((float) playTime - currentNoteTime) < ACURACY_BASE) {
					matchRate = 100 - (int) Math.Abs((float) playTime - currentNoteTime)*100/ACURACY_BASE;
				} else {
					matchRate = 101;
				}
			}
			melodyNumber++;
			if(matchRate < -100) {
				//too slow
				playerDelegate.playEvent(PLAY_EVENT_TOO_SLOW, 0,0);
			} else if (matchRate >100){
				//too fast
				playerDelegate.playEvent(PLAY_EVENT_TOO_FAST, 0,0);
			} else {
				if(matchRate>95) {
					//if match rate > 90. we can say it is 100%.
					matchRate = 100;
				} else {
					matchRate = matchRate * 100/95;
				}
				playerDelegate.playEvent(PLAY_EVENT_MATCH_RATE, matchRate,0);

				melodyAcuracySum += matchRate;
			}
			playerDelegate.playNoteEnd();

			playerDelegate.playEvent(PLAY_EVENT_FIRE_NOTE, 1,0);
			playerDelegate.playEvent(PLAY_EVENT_FIRE_DRUM_NOTE, DRUM_HIT_LIGHT,0);


		} 

		if(isIncludeMelody && drumFactorTotal>=100 && noteNumber >4) {
			playerDelegate.playEvent(PLAY_EVENT_FIRE_DRUM_NOTE, DRUM_HIT_HEAVY,0);
		}
		else if( (drumFactorTotal>=50 && noteNumber >2) || (noteNumber >4 && playTime < midiEventMan.firstMelodyTick)) {
			playerDelegate.playEvent(PLAY_EVENT_FIRE_DRUM_NOTE, DRUM_HIT_MEDIUM,1);
		}
		else if(isIncludeMelody || drumFactorTotal>0 || noteNumber >4) {
			playerDelegate.playEvent(PLAY_EVENT_FIRE_DRUM_NOTE, DRUM_HIT_LIGHT,0);
		}

		///another play mode. don't stop . need to comment out this
		if(autoPlayMusic != NON_STOP_TAP_PLAY &&  playTime < currentNoteTime) {
			playTime = currentNoteTime;
		}
		///end of another play mode.
		actualPlayTime = playTime;
		return matchRate;

	}

	void onLastNoteIsPlayed() {
		isPlayLastNote = true;
	}



	public void playMidiEvents(List<MusicNote> midiEvents, int instrument) {
		MusicNote eventFinal = null;

		int i;
		//if not blow type instrument, play all notes at the same time.
		if(instrument < MusicInstrument.BLOW_TYPE_INSTRUMENT) {
			midiEngine.addNoteBegin();
			for(i = 0; i< midiEvents.Count; i++) {
				MusicNote note =  midiEvents[i];
				midiEngine.addNote(0, note.value, note.velocity, instrument, 480, 
					note.channel, 255, true);
			}
			midiEngine.prepareSoundAllNotes();
			midiEngine.playReadySound();



		} else {
			//look for high pitch note
			long duration = 500;
			for(i = 0; i< midiEvents.Count; i++) {
				MusicNote note = midiEvents[i];
				if(eventFinal==null) {
					eventFinal = note;
				} else if(note.value > eventFinal.value) {
					eventFinal = note;
				}
				if(eventFinal.tickGapNext > duration) {
					duration = eventFinal.tickGapNext;
				}

			}
			if(eventFinal!=null) {
				//if(!melodyMute)
				{
					playNote(eventFinal.value + baseNote, instrument,   eventFinal.velocity, duration, true);
				}
			}
		}
	}

	int prepareNextNote(int hitPressCount, bool autoHit) {

		if(midiEngine.isReadySoundAvailable()) {
			return 0;
		}

		currentPlayingMidiEventList = midiEventMan.getPlayingMidiNoteListByTime(prepareNextNoteTimeBase);

		//CCLog("prepareNextNoteTimeBase: %d", prepareNextNoteTimeBase);
		if(currentPlayingMidiEventList.Count > 0) {
			//CCLog("get playing event list");
			prepareNextNoteTimeBase = midiEventMan.getEndEventTick(currentPlayingMidiEventList)+1;
		} else {
			//CCLog("empty playing event list!!!!");
			if(currentNoteIndex >0) {
				onLastNoteIsPlayed();
			}
			return 0;
		}


		currentNoteTime = midiEventMan.getStartEventTick(currentPlayingMidiEventList);
		////////
		List<MusicNote> midiEventList = currentPlayingMidiEventList;
		midiEngine.addNoteBegin();
		int index = 0;
		MusicNote musicNote = null;
		while(index < midiEventList.Count) {

			musicNote = midiEventList [index++];
			int note = musicNote.value;
			int velocity = musicNote.velocity;
			bool isMelodyNote = musicNote.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE || musicNote.melodyEventEx == MusicNote.PIANO_MELODY_NOTE;
			long dt = (long) Math.Abs(musicNote.getDuration(appContext.isWindInstrument() && isMelodyNote));

			int instrument = appContext.getInstrument();


			int volume;



			if(isMelodyNote) {
				note = note + baseNote;
				if(instrument==-1) {
					instrument = musicNote.instrument;
				}
				//make melody sound louder.

				volume = 255;
				velocity = velocity * 2;
				if(autoPlayMusic == HARMONY_PLAY) {
					velocity = 0;
				}
				if(appContext.isCurrentSoundBank()) {
					volume = 0;
					velocity = 0;
				} else if(instrument == MusicInstrument.DRUM_INSTRUMENT && melodyMute) {
					continue; //don't play drum
				} else if(melodyMute) {
					volume = 0;
					velocity = 0;
				}


			} else if(musicNote.channel==9) {
				instrument = musicNote.instrument;
				volume = musicNote.volume;
				//velocity = velocity * 1.5;
				//game center. don't play drum
				//continue;

			}
			else {
				instrument = musicNote.instrument;
				volume = musicNote.volume;
			}
			if(isMelodyNote && melodyMute) {
				velocity = 0;
				volume = 0;
			}
				

			if( isMelodyNote && instrument == MusicInstrument.DRUM_INSTRUMENT) {
				//if play instrument is drum, then coonvert it to piano
				dt = 350;
				midiEngine.addNote((int) (musicNote.tick - currentNoteTime), note, velocity, 0, dt, musicNote.channel, volume, isMelodyNote);
			} else {

				midiEngine.addNote((int) (musicNote.tick - currentNoteTime), note, velocity, instrument, dt, musicNote.channel, volume, isMelodyNote);
			}
		}
		midiEngine.prepareSoundAllNotes();
		currentNoteIndex++;
		hitNext = false;
		return 0;

	}



	public int hit(int hitPressCount, bool autoHit) {

		return playReadyNote();


	}




int playNextNote (int hitPressCount, int jumpMaximal) {
	/*  
	//it introduce a bug: in pause mode, click screen will restart play
	if(timer == nil) {
		[self startPlay:YES];
	} */
	if( !checkNextisAutoPlay()) {
		if(jumpMaximal==0) {
			jumpMaximal = PLAY_NEXT_NOTE_JUMP;
		}
		long tick = currentNoteTime;
		if(tick>=0 && tick - playTime < jumpMaximal ) {
			hitNext = true;
			return hit(hitPressCount, false);
		} else {
			playerDelegate.playEvent(PLAY_EVENT_TOO_FAST, 0,0);
		}

	} else {

		//jump to next melody , because performance issue. only for iOS.

		long tick = midiEventMan.findNextMelodyEventTime( playTime+1);
		if(jumpMaximal==0) {
			jumpMaximal = PLAY_NEXT_NOTE_JUMP;
		}
		if(tick>=0 && tick - playTime < jumpMaximal ) {
			midiEngine.releaseReadySound();
			if(tick >0) {
				prepareNextNoteTimeBase = tick;
			}
			prepareNextNote(1, false);

			return hit(hitPressCount, false);
		} else {
			if(currentNoteIndex <= 1) {
				hitNext = true;
				return hit(hitPressCount, false);
			} else {
				playerDelegate.playEvent(PLAY_EVENT_TOO_FAST, 0,0);
			}
		}
	}



	return 0;

}

	void setBaseVolume(float baseVolume) {
		midiEngine.setBaseVolume(baseVolume);
	}

	void changeVolumeBySpeed(float speed) { // speed (- 100.0 ~ 100.0)
		midiEngine.changeVolumeBySpeed(speed);

	}
		
	public void playNote(int note, int instrument, int velocity, long duration, bool stopAllPrevious) {

		/*	if(instrument < BLOW_TYPE_INSTRUMENT && instrument != DRUM_INSTRUMENT) {
		instrument = PIANO_INSTRUMENT;
	}*/
		note = baseNote + note;

		MusicNote n = new MusicNote();

		n.instrument = instrument;
		n.value = note;
		n.velocity = velocity;
		n.tickGapNext =(int) duration;
		playerDelegate.playNoteNotify(n);
		//playerDelegate.playNoteNotify(note, instrument, velocity, 0, false, duration, 0);
		playerDelegate.playEvent(PLAY_EVENT_FIRE_SOLOPLAY_NOTE, note,0);


		//gu zheng
		if(AppContext.isSoundBank(instrument)) { //appContext.isCurrentSoundBank() 
			/*    char noteFile[256*2];
		note = note - 62 + 12;
		sprintf(noteFile, "%.2d.mp3", note);
		char* dataOut = NULL;
		int dataSize = AppContext::readDataFromAsset(noteFile, &dataOut);
		if(dataOut) {
		midiEngine.playSoundStream(dataOut, dataSize);
		delete dataOut;
		}
		//std::string strPath = CCFileUtils::sharedFileUtils()->fullPathFromRelativePath(noteFile);
		//midiEngine.playMidiNoteWithFile(note, velocity, duration, noteFile);//(char*) strPath.c_str());
		*/
			string strPath = "/soundfont";
			if(!AppContext.isWindInstrument(instrument)){// !appContext.isCurrentFluteStyle()) {//midiEngine.stopWave()) {

				// int instrument = appContext.getInstrument();
				if(instrument == 108)  {// gu zheng
					instrument = MusicInstrument.ZITHER_INSTRUMENT;
				}
				instrument = MusicInstrument.ZITHER_INSTRUMENT;

				midiEngine.playNoteFlute(instrument, note, strPath, duration, velocity * 1.0f / 255);// (1, (char*) strPath.c_str(), (char*) strPathEx.c_str());

			} else {
				if(stopAllPrevious) {

					midiEngine.stopAllNoteFlute(0, 0);
				}
				/* if(autoPlayMusic == HALF_AUTO_PLAY  ) {
			duration = 15000;//duration * 5;
			if(duration > 15000) {
			duration = 15000;
			}
			} else if(autoPlayMusic == SOLO_IDLE || autoPlayMusic == SOLO_RECORDING) {
			duration = 15000;
			}*/
				if(autoPlayMusic == TAP_PLAY || autoPlayMusic == NON_STOP_TAP_PLAY) {
					duration = duration * 2 + 2000;
				} else if(autoPlayMusic == DEMO_PLAY || autoPlayMusic == LEARN_PLAY){
					if(duration <= 1000) { //吹的长度如果太小，就不用换气，所以+100，以连接下一个音符，否则要换气，就不加，这样会真实得多。
						duration = duration + 100;
					}

				}
				midiEngine.playNoteFlute(instrument, note, strPath, duration, velocity * 1.0f / 255);// (1, (char*) strPath.c_str(), (char*) strPathEx.c_str());
			}

		}
		else {

			midiEngine.playMidiNote(note ,velocity ,(int) duration ,instrument, instrument == 128?9:0);

		}


	}

	public void stopNote (int note, int atLeastMS_, int noteLastTimeMs_) {
		note = baseNote + note;
		if(appContext.isWindInstrument() && appContext.isCurrentSoundBank()) {
			playerDelegate.playEvent(PLAY_EVENT_STOP_SOLOPLAY_NOTE, note,noteLastTimeMs_);
			midiEngine.stopNoteFlute(note, atLeastMS_, noteLastTimeMs_);
			if( autoPlayMusic == SOLO_RECORDING) {
				//if it is recording, then adjust teh flute note duration
				long durationMs = playTime - lastRecordNoteTick;
				//midiEventMan.setLastMelodyNoteDuration(durationMs);

			}
		}
	}

	public void stopAllNote (int atLeastMS_, int noteLastTimeMs_) {
		//if(appContext.isCurrentFluteStyle() && appContext.isCurrentSoundBank()) {
		playerDelegate.playEvent(PLAY_EVENT_STOP_SOLOPLAY_NOTE, 0,0);
		midiEngine.stopAllNoteFlute(atLeastMS_, noteLastTimeMs_);
		//}
	}

	void recordNoteStart() {
		autoPlayMusic = SOLO_RECORDING;
		midiEventMan.removeAllEvent();
		resetPlayStatus();
		playTime = 0;
		actualPlayTime = 0;

	}
	/*
	void recordNote (int note, int instrument, int velocity, int duration) {
		if(autoPlayMusic != SOLO_RECORDING) {
			return;
		}
		MusicNote mn = new MusicNote();
		mn.instrument = instrument;
		mn.value = note - baseNote;
		mn.melodyEventEx = PIANO_MELODY_NOTE;// OTHER_HARMONY_NOTE;
		mn.volume = 127;
		mn.channel = 0;
		mn.velocity = velocity;
		mn.tick = playTime;
		lastRecordNoteTick = playTime;
		midiEventMan.insertEvent(midiEvent);

		MidiEvent* midiEventOff = mn.clone();
		midiEventOff->mEventType = MIDI_EVENT_NOTE_OFF;
		midiEventOff->tick += duration;

		midiEventMan.insertEvent(midiEventOff);
		lastTimeRecordNote = playTime;
		isRecordingAvaiableForSave_ = true;
	}

	void recordNoteEnd() {
		autoPlayMusic = SOLO_IDLE;

	}*/

	void playDrumNote(int note, int velocity, int duration) {
		MusicNote mn = new MusicNote();

		mn.instrument = 0;
		mn.value = note;
		mn.velocity = velocity;
		mn.tickGapNext = duration;
		mn.channel = 9;
		if(playerDelegate!=null) {
			playerDelegate.playNoteNotify(mn);
		}

		//playerDelegate.playNoteNotify(note, 0, velocity, 0, false, duration, 9);
		int drumFactor = getDrumNoteFactor(note);
		if(drumFactor >=100) {
			playerDelegate.playEvent(PLAY_EVENT_FIRE_DRUM_NOTE, 1,0);
		} else {
			playerDelegate.playEvent(PLAY_EVENT_FIRE_DRUM_NOTE, 0,0);
		}

		midiEngine.playMidiNote(note ,velocity ,duration ,0, 9);
	}

	string[] labelNote = {
		"C",
		"Db",
		"D",
		"Eb",
		"E",
		"F",
		"Gb",
		"G",
		"Ab",
		"A",
		"Bb",
		"B",
	};

	void shootComingNotes() {
		 
		List<MusicNote> midiEventList = midiEventMan.getMidiEventListByTime(lastComingEventStartTime, playTime+comingEventDuration - lastComingEventStartTime);



		lastComingEventStartTime = playTime+comingEventDuration;
		if(midiEventList==null || midiEventList.Count == 0) {
			//    std::cout << "midiEventList == NULL";
			return;
		}
		//int instrument = 0;//[AppContext sharedContext].instrument;
		int index = 0;
		int drumFactorTotal = 0;
		MusicNote mn;
		while(index < midiEventList.Count) {

			mn = midiEventList[index++];
			if(lastComingEventIndex == 0 && autoPlayMusic != LEARN_PLAY && autoPlayMusic != HARMONY_PLAY && autoPlayMusic != DEMO_PLAY && autoPlayMusic!= SOLO_RECORD_REPLAY) {
				currentNoteTime = mn.tick;
			}
			int note = mn.value;
			int velocity = mn.velocity;
			drumFactorTotal = 0;


			if(mn.tick < playTime + comingEventDuration) {
				if(mn.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE ||  mn.melodyEventEx == MusicNote.PIANO_MELODY_NOTE) { //musicNote.melodyEventEx == PIANO_HARMONY_NOTE ||
					//don't display drum event
					if(mn.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE) {
						drumFactorTotal = getDrumFactorByTime(mn.tick);
					}
					note = note;// + baseNote;
					MusicNote musicNote = new MusicNote(mn);
					musicNote.value = note;
					musicNote.drumFactorTotal = drumFactorTotal;
					//musicNote.tickGapNext =  musicNote.getDuration(appContext.isCurrentSoundBank());

					playerDelegate.playComingNoteNotify(musicNote);
					//playerDelegate.playComingNoteNotify(note, musicNote.instrument, word ,
					//	musicNote.tick, musicNote.melodyEventEx ,
					//	drumFactorTotal,eventSet->getDuration(appContext.isCurrentSoundBank()));

				} 
				lastComingEventIndex++;
			} else {
				break;
			}


		} 

	}

	int getDrumFactorByTime(long tick) {
		int drumFactorTotal = 0;
		List<MusicNote>  midiEventList = midiEventMan.getPlayingMidiNoteListByTime(tick);
		if(midiEventList == null) 
			return drumFactorTotal;
		int index = 0;
		MusicNote n;
		while(index < midiEventList.Count) {

			n = midiEventList[index++];
			int note = n.value;
			if(n.channel==9) {
				drumFactorTotal += getDrumNoteFactor(n.value);
			}
		}
		return drumFactorTotal;

	}
		
	/*
	bool saveRecordNotesToMidi(char* midiFile) {
		int bufferSize = 10* 1000;
		char* bufferMidi = (char*) malloc(bufferSize); //10 K is enough
		int midiSize = 0;
		bool ret = false;
		if(appContext.getInstrument() == DRUM_INSTRUMENT) {
			midiSize = midiParserEx->saveCurrentEventListToMidi(bufferMidi, bufferSize, true);
		} else {
			midiSize = midiParserEx->saveCurrentEventListToMidi(bufferMidi, bufferSize, false);
		}
		if(midiSize > 0 ) {
			ret = AppContext::packAndSaveDataToFile(bufferMidi, midiSize, midiFile);
		}
		free(bufferMidi);
		return ret;
	}
	*/
	void shootTempoEvent(long tick) {
	/*	MidiEvent midiEvent(MIDI_EVENT_TEMP);

		midiEvent.lyricsWord = NULL;
		midiEvent.value = 1;
		midiEvent.drumFactorTotal = 0;
		midiEvent.tickGapNext =0;
		midiEvent.tick = tick;
		if(delegate) 
			playerDelegate.playComingNoteNotify(&midiEvent);*/
	}



}
