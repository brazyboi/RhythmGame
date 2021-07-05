using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public long score;
    public float speed = 30.0f; //unit per second
                                // Use this for initialization
    public static string prevScene;

    public static void gotoSongSelect()
    {
        SceneManager.LoadScene("SongSelect", LoadSceneMode.Single);
    }

    public static void gotoTitleScreen()
    {
        SceneManager.LoadScene("TitleScreen", LoadSceneMode.Single);
    }

    public static void gotoSongList()
    {
        SceneManager.LoadScene("SongList", LoadSceneMode.Single);
    }

    public static void gotoPlayScene()
    {
        SceneManager.LoadScene("PlayScene2D", LoadSceneMode.Single);
    }

    public static void gotoRewardScreen()
    {
        SceneManager.LoadScene("RewardScreen", LoadSceneMode.Single);
    }

    public static void backToPrevScene()
    {
        SceneManager.LoadScene(prevScene, LoadSceneMode.Single);
    }
}
