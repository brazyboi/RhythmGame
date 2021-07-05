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
			long score = calculateTapNoteScore(note.tick, note.tick).Item2;
			totalScore += score;
			UnityEngine.Debug.Log("calculateTapNoteScore: " + score);
			score  = calculateHoldingNoteScore(note, isWindInstrument, note.tick + calculateNoteDuration(note), note.tick);
			totalScore += score;
			UnityEngine.Debug.Log("calculateHoldingNoteScore: " + score);
			score= calculateReleaseTimingScore(note, isWindInstrument, note.tick + calculateNoteDuration(note));
			totalScore += score;
			UnityEngine.Debug.Log("calculateReleaseTimingScore: " + score);

		}
		UnityEngine.Debug.Log("Total score: " + totalScore);
		return totalScore;
	}

	public static (string, int) calculateTapNoteScore(long tapTime, long noteTick)
    {
		float tapGapTime = Mathf.Abs((float)noteTick - tapTime);
		string scoreText = "";
		float score = 0;
		float tapPerfectScore = 30;
		if (tapGapTime < 30)
		{
			scoreText = "PERFECT";
			score = tapPerfectScore;
		}
		else if (tapGapTime < 60)
		{
			scoreText = "GREAT";
			score = tapPerfectScore * 0.93f;
		}
		else if (tapGapTime < 130)
        {
			scoreText = "GREAT";
			score = tapPerfectScore * 0.88f;
		} else if(tapGapTime < 200) 
		{
			scoreText = "JUST";
			score = tapPerfectScore * 0.82f;
		} else
        {
			scoreText = "NOT BAD";
			score = tapPerfectScore * 0.75f;
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
			if(holdingTime > calculateNoteDuration(note))
            {
				holdingTime = calculateNoteDuration(note);
			}
			if (playTime <= note.tick + calculateNoteDuration(note))
			{//only update if before reach to end.
				return holdingTime / 50;
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
			if (tapGapTime < 200)
			{
				score = 5;
			}

		}
		return score;
	}


}
