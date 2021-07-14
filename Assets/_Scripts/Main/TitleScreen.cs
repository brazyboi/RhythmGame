using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;
using AudienceNetwork;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameObject playButton;
    public GameObject rewardButton;
    public GameObject shareButton;
    private SoundHandle soundHandle;
    public Text text;

    // Start is called before the first frame update
    void Start()
    {
        playButton.GetComponent<Button>().onClick.AddListener(startGame);   
        rewardButton.GetComponent<Button>().onClick.AddListener(rewards);

        AudienceNetworkAds.Initialize();
    }

    public void startGame()
    {
        Analytics.CustomEvent("startGame");
        GameManager.gotoLevelScreen();
    }

    void rewards()
    {
        GameManager.prevScene = "TitleScreen";
        GameManager.gotoRewardScreen();
        //soundHandle = MidiEngine.instance().playMidiSoundFile("songs/zhouhui_fengling");
    }

    void share()
    {
        // show popup for share
    }

    // Update is called once per frame
    void Update()
    {
        SoundPlayer.singleton().timerUpdate(Time.deltaTime);
    }
}
