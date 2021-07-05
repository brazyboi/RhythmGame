using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiSheetMusic;

public class MidiEventMan  {


	public int minMelodyNote;
	public int maxMelodyNote;
	public long lastNoteTick;
	public long firstMelodyTick;

	public List<MusicNote> midiEventListMelody;
	public List<MusicNote> midiEventListHarmony;

	private bool isOverFirstMelody;

	private const int MIN_MIDI_THESHOLD  = 100;

	//for debug
	private int lastMelodyI;
	private int lastHarmonyI;

	//
	private long firstNoteTick = long.MaxValue;

	public static bool isIncludeMelody(List<MusicNote> list) {
		for (int i = 0; i < list.Count; i++) {
			MusicNote n = list [i];
			if (n.isMelodyEvent ()) {
				return true;
			}
		}
		return false;
	}

	public MidiEventMan() {
		midiEventListMelody = new List<MusicNote> ();
		midiEventListHarmony = new List<MusicNote> ();
	}

	public void removeAllEvent() {
		midiEventListMelody.Clear();
		midiEventListHarmony.Clear();
		minMelodyNote = 200;
		maxMelodyNote = 0;
		firstMelodyTick = 0;
		isOverFirstMelody = false;
		lastNoteTick = 0;
	}

	public int length() {
		return midiEventListHarmony.Count + midiEventListMelody.Count;
	}

	public long duration() {
		long d1 = 0;
		long d2 = 0;
		if(midiEventListMelody.Count > 0) {
			d1 = midiEventListMelody [midiEventListMelody.Count - 1].tick + midiEventListMelody [midiEventListMelody.Count - 1].tickGapNext;
		}
		if (midiEventListHarmony.Count > 0) {
			d2 = midiEventListHarmony [midiEventListHarmony.Count - 1].tick + midiEventListHarmony [midiEventListHarmony.Count - 1].elapseTime;

		}
		return d1 > d2 ? d1 : d2;
	}

	public void insertMidiNote(int trackId, MidiNote note, int melodyChannel, int melodyChannel2, bool isDrumMelody, int pitchValue,  float ms_per_tick) {
		


		lastNoteTick = (long) (note.StartTime * ms_per_tick);
		//create music note
		MusicNote n = new MusicNote();
		n.channel = note.Channel;
		//n.instrument = 
		n.value = note.Number;
		n.tick = (long) (note.StartTime * ms_per_tick);
		n.elapseTime = (int) (note.Duration * ms_per_tick);
		//n.velocity = 
		n.isHit = false;
		n.velocity = note.velocity;


		if (n.channel == melodyChannel || (melodyChannel2>0 && n.channel == melodyChannel2)
			|| (isDrumMelody && n.channel==9)) {
			if(isDrumMelody &&  n.channel!=9) {
				n.melodyEventEx = MusicNote.SUB_MELODY_NOTE; //if play drum , melody note should be marked
			} else {
				n.melodyEventEx = MusicNote.GENERAL_MELODY_NOTE;
			}
			n.instrument = 0;
			if(pitchValue>0 && n.channel!=9) {
				n.value += 12;
			}
			n.checkIfBeatble();
		} else {
			if(melodyChannel==99) {
				if(trackId == 0) {
					if(pitchValue>0) {
						n.value += 12;
					}
					n.melodyEventEx = MusicNote.PIANO_MELODY_NOTE;
					n.instrument = 0;
				} else {
					n.melodyEventEx = MusicNote.PIANO_HARMONY_NOTE;
					//    note.instrument = 24;
				}   
			} else {
				n.melodyEventEx = MusicNote.OTHER_HARMONY_NOTE;
			}

		} 

		bool addEvent = true;
		if(n.channel==9 && (n.value == 42 || n.value == 44 || (n.value > 52 && n.value != 57))) {
			//skip closed hi-ha (42) open hi-hat (46) pedal hi-hat (44)
			addEvent = false;
			return;
		}

		bool isMelody = (n.melodyEventEx == MusicNote.GENERAL_MELODY_NOTE) ||  (n.melodyEventEx == MusicNote.PIANO_MELODY_NOTE);
		if(isMelody) {
			if(midiEventListMelody.Count > 0)
            {
				MusicNote lastNote = midiEventListMelody[midiEventListMelody.Count - 1];
				if(Mathf.Abs(lastNote.tick - n.tick) < 20)
                {
					if(n.value > lastNote.value)
                    {
						midiEventListMelody[midiEventListMelody.Count - 1] = n;
						return;
					} else
                    {
						return;
                    }
                }
            }
			updateMaxMinMelodyNote(note.Number);
			if(!isOverFirstMelody) {
				isOverFirstMelody = true;
				firstMelodyTick = lastNoteTick;
			}
			midiEventListMelody.Add (n);
		} else {
			midiEventListHarmony.Add (n);
		}

		if(firstNoteTick > n.tick)
        {
			firstNoteTick = n.tick;
        }

	}
		

	public void sortAllMidiNote() {
		midiEventListHarmony.Sort (delegate(MusicNote note1, MusicNote note2) {
			return note2.tick > note1.tick ? -1:1; 
		});
	}

	public void sortMidiNote(List<MusicNote> list) {
		list.Sort (delegate(MusicNote note1, MusicNote note2) {
			return note2.tick > note1.tick ? -1:1; 
		});
	}

	public void trimNoSoundAtBeginning()
    {
		foreach(MusicNote n in midiEventListHarmony)
        {
			n.tick -= firstNoteTick;
        }
		foreach(MusicNote n in midiEventListMelody)
        {
			n.tick -= firstNoteTick;
        }
		lastNoteTick -= firstNoteTick;
		firstMelodyTick -= firstNoteTick;
	}

	public long getLastMelodyEndTick()
    {
		if(midiEventListMelody.Count > 0)
        {
			MusicNote n = midiEventListMelody[midiEventListMelody.Count - 1];
			return n.tick + n.tickGapNext;
        }
		return 0;
    }

	public void skipPrelude() {
	//	removeAllEventBeforeTime(firstMelodyTick, true);
		lastNoteTick -= ((int) firstMelodyTick);
	}

	void updateMaxMinMelodyNote(int note) {
		if(note > maxMelodyNote) {
			maxMelodyNote = note;
		}
		if(note < minMelodyNote) {
			minMelodyNote = note;
		}
	}

	void setLastMelodyNoteDuration(long durationMs) {
		if(durationMs <=0 || midiEventListMelody.Count <= 0) {
			return;
		}
		MusicNote n = midiEventListMelody[midiEventListMelody.Count - 1];
		n.elapseTime = (int) durationMs;
		n.tickGapNext = (int) durationMs;

	}
		

	public List<MusicNote> getPlayingMidiNoteListByTime(long currentTick) {
		//UnityEngine.Debug.Log("Last lastMelodyI -"  + lastMelodyI + " lastHarmonyI - " + lastHarmonyI);
		long curPlayTime = 0;
		long curPlayTimeM = long.MaxValue;
		long curPlayTimeH = long.MaxValue;
		int firstMelody = -1;
		int firstHarmony = -1;
		List<MusicNote> midiEventListResult = new List<MusicNote>();

		for(int i=0; i< midiEventListMelody.Count; i++) {
			MusicNote n = midiEventListMelody [i];
			if(n.tick >= currentTick) {
				firstMelody = i;
				break;
			} 
		}

		for(int i=0; i< midiEventListHarmony.Count; i++) {
			MusicNote n = midiEventListHarmony [i];
			if(n.tick >= currentTick) {
				firstHarmony = i;
				break;
			} 
		}

		if (firstMelody >= 0) {
			curPlayTimeM = midiEventListMelody [firstMelody].tick; 
		}
		if (firstHarmony >= 0) {
			curPlayTimeH = midiEventListHarmony [firstHarmony].tick;
		}
		curPlayTime =(long) Mathf.Min (curPlayTimeH, curPlayTimeM);
		List<MusicNote> list = getMidiEventListByTime(curPlayTime, MIN_MIDI_THESHOLD);

		return list;
	}

	public List<MusicNote> getMidiEventListByTime(long currentTick, long tickDuration) {
		int hitNumber =  0;
		bool exitHit = false;
		long curPlayTime = 0;
		long firstPlay = -1;

		List<MusicNote> midiEventListResult = new List<MusicNote>();

		for(int i=0; i< midiEventListMelody.Count; i++) {
			MusicNote n = midiEventListMelody [i];
			if(n.tick < currentTick) {
				continue;
			} else if(n.tick >= currentTick && n.tick < currentTick + tickDuration) {
				midiEventListResult.Add (n);
				lastMelodyI = i;
			} else {
				break;
			}
		}

		for(int i=0; i< midiEventListHarmony.Count; i++) {
			MusicNote n = midiEventListHarmony [i];
			if(n.tick < currentTick) {
				continue;
			} else if(n.tick >= currentTick && n.tick < currentTick + tickDuration) {
				midiEventListResult.Add (n);	
				lastHarmonyI = i;
			} else {
				break;
			}
		}
		sortMidiNote (midiEventListResult);
		return midiEventListResult;
	}
		
	/*
	void removeAllEventBeforeTime(long tick, bool adjustBaseTime) {
		midiEventListMelody->removeAllEventBeforeTickAndAdjustBaseTime(tick);
		midiEventListHarmony->removeAllEventBeforeTickAndAdjustBaseTime(tick);
	} 

	int getMelodyNumBeforeTime(long tick) {
		return midiEventListMelody->getEventNumberBeforeTick(tick);
	}
*/

	public void adjustMelodyNoteLengthByFlute() {
		/*if(isDrum) {
			int melodyChannel1 = melodyChannel % 100;
			int melodyChannel2 = -1;
			if(melodyChannel > 100) {
				melodyChannel2 = (melodyChannel % 10000) /100 % 100;
				if(melodyChannel2 == 0) {
					melodyChannel2 = -1;
				}
			} else {
				melodyChannel2 = -1;
			}
			//the leading melody is in midiEventListHarmony when playing instrument is drum
			MidiEventSet* eventSetMelody = midiEventListHarmony->getEventSet(0);
			while (eventSetMelody!=NULL) {
				if(eventSetMelody->eventNoteOn->channel != melodyChannel1 && 
					eventSetMelody->eventNoteOn->channel != melodyChannel2) {
					eventSetMelody = eventSetMelody->nextSet;
					continue;
				} else {
					break;
				}
			}
			if(eventSetMelody==NULL) {
				//finish adjustment
				return;
			}
			MidiEventSet* nexteventSetMelody = eventSetMelody->nextSet;
			bool finishSetOne = false;
			int gap = 0;
			while (eventSetMelody!=NULL) {
				if(nexteventSetMelody == NULL) {
					break;
				}
				if(nexteventSetMelody->eventNoteOn->channel != melodyChannel1 && 
					nexteventSetMelody->eventNoteOn->channel != melodyChannel2) {
					nexteventSetMelody = nexteventSetMelody->nextSet;
					continue;
				}

				finishSetOne = false;
				gap = 0;
				while (nexteventSetMelody != NULL && !finishSetOne) {
					if(nexteventSetMelody->eventNoteOn->channel != melodyChannel1 && 
						nexteventSetMelody->eventNoteOn->channel != melodyChannel2) {
						nexteventSetMelody = nexteventSetMelody->nextSet;
						continue;
					}
					gap = (nexteventSetMelody->eventNoteOn->tick - eventSetMelody->eventNoteOn->tick);
					if(gap < 25) {
						nexteventSetMelody = nexteventSetMelody->nextSet;
						gap = 0;
					} else {

						finishSetOne = true;
					}
				}
				if(gap <=0 ) {
					eventSetMelody->eventNoteOn->tickGapNext = 3000;
				} else {
					eventSetMelody->eventNoteOn->tickGapNext = gap;
				}
				eventSetMelody = nexteventSetMelody;

			}


		} else */
		{
			if (midiEventListMelody.Count == 0) {
				return;
			}
			//adjust flute type
			long gap = 0;
			for (int i = 0; i < midiEventListMelody.Count; i++)
			{
				gap = 0;
				MusicNote currentNote = midiEventListMelody[i];
				MusicNote nextNote = null;
				for (int j = i + 1; j < i + 5; j++)
				{
					if (j >= midiEventListMelody.Count)
					{
						break;
					}
					MusicNote n = midiEventListMelody[j];
					if (n.tick - currentNote.tick > 0)
					{
						nextNote = n;
						break;
					}
				}
				if (nextNote != null)
				{
					gap = nextNote.tick - currentNote.tick;
				}
				if (gap <= 0)
				{
					currentNote.tickGapNext = 3000;
				}
				else
				{
					currentNote.tickGapNext = (int)gap;
				}
				currentNote.tickGapNext = currentNote.getDuration(true);
			}
		}
	}


	public  long findNextMelodyEventTime( long currentTick) {
		for (int i = 0; i < midiEventListMelody.Count; i++) {
			MusicNote note = midiEventListMelody [i];
			if (note.tick >= currentTick) {
				return note.tick;
			}
		}
		return 0;

	}


	public long getStartEventTick(List<MusicNote> list) {
		if (list.Count <= 0) {
			return long.MaxValue;
		} else {
			return list [0].tick;
		}
	}

	public long getEndEventTick(List<MusicNote> list) {
		return list [list.Count - 1].tick;
	}
}
