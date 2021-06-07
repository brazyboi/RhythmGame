using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class MusicNote  {
	 

	public const int GENERAL_MELODY_NOTE = 1;
	public const int PIANO_MELODY_NOTE = 2;
	public const int PIANO_HARMONY_NOTE = 3;
	public const int OTHER_HARMONY_NOTE = 0;
	public const int SUB_MELODY_NOTE = 4; // this note will be in harmony list. but can be played with current instrument

	public long    tick;
	public int     value;
	public int     velocity;
	public int     channel;
	public int     melodyEventEx;
	public int     instrument;
	public int     volume;
	public int     tickGapNext;
	public int 		elapseTime;
	//modify in run time, for passing info
	public int drumFactorTotal;
	public bool isHit;


	private const int BEATABLE_DRUM_NUM = 15;

	private const int BASS_DRUM_2 = 35;
	private const int BASS_DRUM_1 =  36;
	private const int STICK_DRUM =  37;
	private const int SNARE_DRUM_1=  38;
	private const int SNARE_DRUM_2 = 40;
	private const int LOW_TOM_1   = 43;
	private const int MID_TOM_1   = 47;
	private const int HIGHT_TOM_1  =  50;
	private const int LOW_TOM_2  =  41;
	private const int MID_TOM_2  =  45;
	private const int HIGHT_TOM_2  =  48;
	private const int OPEN_HI_HAT =  46;
	private const int CLOSED_HI_HAT =  42;
	private const int CRASH_CYMBAL_1 = 49;
	private const int CRASH_CYMBAL_2 = 57;
	private const int CHINESE_CYMBAL = 52; 

	private const int LONGEST_NOTE_DRUATION_TO_ADJUST = 2000;
	   
	private static readonly int[] drumBeatNote = {BASS_DRUM_2, BASS_DRUM_1,STICK_DRUM,SNARE_DRUM_1,SNARE_DRUM_2,LOW_TOM_1,MID_TOM_1,
		HIGHT_TOM_1,LOW_TOM_2,MID_TOM_2,HIGHT_TOM_2,OPEN_HI_HAT,CRASH_CYMBAL_1, CRASH_CYMBAL_2, CHINESE_CYMBAL};

	public MusicNote() {
		
	}

	public MusicNote(MusicNote n) {
		this.tick = n.tick;
		this.value = n.value;
		this.velocity = n.velocity;
		this.channel = n.channel;
		this.melodyEventEx = n.melodyEventEx;
		this.instrument = n.instrument;
		this.volume = n.volume;
		this.tickGapNext = n.tickGapNext;
		this.elapseTime = n.elapseTime;

	}

	public int getValue()
    {
		return value;
    }

	public int getType() {
		return melodyEventEx;// =  OTHER_HARMONY_NOTE;
	}

	public void checkIfBeatble()  {
		//change the note which can't beatable to harmony note, use for drum
		if(channel == 9) {
			for (int i=0; i< BEATABLE_DRUM_NUM; i++) {
				if(drumBeatNote[i] == value) {
					return;
				}
			}
			melodyEventEx = PIANO_HARMONY_NOTE;
		} 
	}
	public bool isMelodyEvent() {
		if(melodyEventEx == GENERAL_MELODY_NOTE || melodyEventEx == PIANO_HARMONY_NOTE) {
			return true;
		} else {
			return false;
		}
	}

	public int getDuration(bool adjustByFluteInstrument) {
		int duration = elapseTime;
		if(adjustByFluteInstrument && channel != 9) {
			if(duration < tickGapNext) {
				//if the duration less than the tick gap bettween two notes:
				if(duration > LONGEST_NOTE_DRUATION_TO_ADJUST) {
					//if it is large than 2 second, use default, did nothing. otherwise

				} else {
					//set to gap, because the flute is last long
					duration = tickGapNext;
					if(duration > LONGEST_NOTE_DRUATION_TO_ADJUST) { //if the gap is too long , adjust to 2 second
						duration = LONGEST_NOTE_DRUATION_TO_ADJUST;
					}
				}
			}
			else if(duration > tickGapNext) {
				//if the large than tick gap next.
				duration = tickGapNext;
			}
			//make sure the duration is large than 500 second
			if(duration < 500) {
				duration = 500;
			}
		} else {
			if(duration <=0) {
				duration = 360;
			}
		}

		return duration;
	}



};



/*
class MidiEventList {

	public:


	protected:
	MidiEventSet* midiEventSetHead;
	MidiEventSet* lastMidiEventSet;
	MidiEventSet* lastMidiEventSetNoteOn;
	public:
	MidiEventList();
	~MidiEventList();

	int length();

	bool insertEvent(MidiEvent* midiEvent);       //NoteOn event must be insert before NoteOff 
	bool insertEventSet(MidiEventSet* midiEventSet);
	bool deleteEventSet(MidiEventSet* midiEventSet);
	MidiEventSet* getEventSet(int index);
	void removeAllEvent();

	int findFirstIndexAfterTick(long tick);
	bool isIncludeMelody();

	void removeAllEventBeforeTick(long tick);
	void removeAllEventBeforeTickAndAdjustBaseTime(long tick);

	long getStartEventTick();
	long getEndEventTick();
	int getEventNumberBeforeTick(long tick); 

	private: 




}

*/

