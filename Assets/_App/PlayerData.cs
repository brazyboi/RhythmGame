using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerData 
{
    public enum SongStatus{
        SONG_PASSED,
        SONG_LOCKED,
        SONG_IN_PROGRESS
    }
    public static int saveSongScore(string songId, int accuracy)
    {
        int oldAccuracy = PlayerPrefs.GetInt(songId + "-accuracy", 0);
        if (oldAccuracy < accuracy)
        {
            PlayerPrefs.SetInt(songId + "-accuracy", accuracy);
        } else
        {
            accuracy = oldAccuracy;
        }
        return accuracy;
    }

    public static int getSongScore(string songId)
    {
        return PlayerPrefs.GetInt(songId + "-accuracy", 0);
    }

    public static int getPlayerLevel()
    {
        return PlayerPrefs.GetInt("songLevel", 0) / 10; 
    }

    public static bool unlockSong(int level)
    {
        int oldLevel = PlayerPrefs.GetInt("songLevel", 0);

        Debug.Log("oldLevel = " + oldLevel + " level = " + level);
        if (oldLevel < level)
        {
            PlayerPrefs.SetInt("songLevel", level);
        }
        else
        {
            level = oldLevel;
        }
        return true;
    }

    public static SongStatus getSongStatus(int songLevel)
    {
        int cur = PlayerPrefs.GetInt("songLevel", 0);
        if(songLevel < cur)
        {
            return SongStatus.SONG_PASSED;
        } else if(songLevel == cur)
        {
            return SongStatus.SONG_IN_PROGRESS;
        } else
        {
            return SongStatus.SONG_LOCKED;
        }
    }

    public static bool isInstrumentUnlock(int instrumentLevel)
    {
        return getPlayerLevel() >= instrumentLevel;
    }

    public static void savePlayerName(string name)
    {
        PlayerPrefs.SetString("playerName", name);
    }

    public static string getPlayerName()
    {
        return PlayerPrefs.GetString("playerName", "Anynomous"); ;
    }

    public static void resetPlayer()
    {
        PlayerPrefs.SetInt("songLevel", 0);

    }

    public static void unlockEverything()
    {
        PlayerPrefs.SetInt("songLevel", 100);
    }

}
