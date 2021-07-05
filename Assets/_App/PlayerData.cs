using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData 
{

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

    public static bool isSongUnlock(int songLevel)
    {
        int cur = PlayerPrefs.GetInt("songLevel", 0);
        if(songLevel <= cur)
        {
            return true;
        } else
        {
            return false;
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

}
