using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongListUI : MonoBehaviour
{
    public GameObject backButton;
    public GameObject songsTextTop;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {
        songsTextTop.GetComponent<Text>().text = "SONGS: LEVEL " + (AppContext.instance().curSongListLevel + 1);
        backButton.GetComponent<Button>().onClick.AddListener(backToLevelScreen);

    }

    void backToLevelScreen()
    {
        GameManager.gotoLevelScreen();
    }

    // Update is called once per frame
    void Update()
    {
        SoundPlayer.singleton().timerUpdate(Time.deltaTime);
    }
}
