using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUtils
{
	public static long calculateTotalScore(bool isWindInstrument, List<MusicNote> midiEventListMelody)
	{
		long totalScore = 0;
		foreach( MusicNote note in midiEventListMelody)
        {
			totalScore += calculateTapNoteScore(note.tick, note.tick).Item2;
			totalScore += calculateHoldingNoteScore(note, isWindInstrument, note.tick + calculateNoteDuration(note), note.tick);
			totalScore += calculateReleaseTimingScore(note, isWindInstrument, note.tick + calculateNoteDuration(note));

		}
		return totalScore;
	}

	public static (string, int) calculateTapNoteScore(long tapTime, long noteTick)
    {
		float tapGapTime = Mathf.Abs((float)noteTick - tapTime);
		string scoreText = "";
		float score = 0;
		if (tapGapTime < 100)
		{
			scoreText = "GREAT";
			score = 20 - tapGapTime * 5 / 100;
		}
		else if (tapGapTime < 300)
		{
			scoreText = "GOOD";
			score = 15 - (tapGapTime - 100) * 5 / 200;
		}
		else if (tapGapTime < 500)
		{
			scoreText = "JUST";
			score = 10 - (tapGapTime - 300) * 5 / 200;
		}
		else
		{
			scoreText = "JUST";
			score = 5;
		}
		if(score < 0)
        {
			score = 0;
        }
		return (scoreText, (int) score);
	}

	public static long calculateNoteDuration(MusicNote note)
    {
		long noteDuration = 0;
		if (note.elapseTime < 500)
		{
			noteDuration = note.elapseTime;
		}
		else
		{
			noteDuration = note.tickGapNext - 100;
		}
		return noteDuration;
	}

	public static bool isHoldingNote(MusicNote note, bool isWindInstrument)
	{
		if (isWindInstrument)
		{
			if (calculateNoteDuration(note) > 300)
			{
				return true;
			}
			else
			{
				return false;
			}

		}
		else
		{
			return false;
		}

	}

	
	public static long calculateHoldingNoteScore(MusicNote note, bool isWindInstrument, long playTime, long tapTime)
    {
		if (isHoldingNote(note, isWindInstrument))
		{
			long holdingTime = playTime - tapTime;
			if (playTime < note.tick + calculateNoteDuration(note))
			{//only update if before reach to end.
				return holdingTime / 10;
			}
		}
		return 0;
	}


	public static long calculateReleaseTimingScore(MusicNote note, bool isWindInstrument, long releaseTime)
	{
		long score = 0;
		if (isHoldingNote(note, isWindInstrument))
		{
			float tapGapTime = Mathf.Abs((float)note.tick + calculateNoteDuration(note) - releaseTime);
			if (tapGapTime < 100)
			{
				score = 50;
			}

		}
		return score;
	}


}
