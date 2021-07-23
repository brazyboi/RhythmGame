using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayPerformance : MonoBehaviour
{
    public GameObject songTitlePanel;
    public GameObject scorePanel;
    public GameObject passfailPanel;
    public GameObject restartButton;
    public GameObject songsButton;

    Timer endAccuracyText;
    private float changeTextTime;
    long accuracy;
    // Start is called before the first frame update
    void Start()
    {
        changeTextTime = 1.5f;
        init();
    }

    void init()
    {
        songTitlePanel.GetComponent<Text>().text = AppContext.instance().songItem.title;
        restartButton.GetComponent<Button>().onClick.AddListener(restartSong);
        songsButton.GetComponent<Button>().onClick.AddListener(redirectSongList);
        
        scorePanel.GetComponent<Text>().text = "accuracy: 0%";
        calculateAccuracy();
        animateUpdateAccuracy();

        if (AppContext.instance().failed)
        {
            passfailPanel.GetComponent<Text>().text = "FAIL";
            passfailPanel.GetComponent<Text>().color = Color.red;
        }
        else
        {
            passfailPanel.GetComponent<Text>().text = "PASS";
            passfailPanel.GetComponent<Text>().color = Color.green;
            PlayerData.unlockSong(AppContext.instance().songItem.level + 1);
            PlayerData.saveSongScore(AppContext.instance().songItem.path, (int)(accuracy));

            Debug.Log("new song unlcoked: " + (AppContext.instance().songItem.level + 1));
            //Debug.Log("SONG LEVEL: " + AppContext.instance().songItem.level
        }
    }

    void restartSong()
    {
        
        AppContext.instance().totalScore = 0;
        GameManager.gotoPlayScene();
    }

    void redirectSongList()
    { 
        AppContext.instance().totalScore = 0;
        GameManager.gotoSongList();
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void animateUpdateAccuracy()
    {
        Timer.createTimer(gameObject).startTimer(changeTextTime, new EndAccuracyCallback(this));
    }

    void calculateAccuracy()
    {
        long perfectPlayScore = ScoreUtils.calculateTotalScore(AppContext.instance().isWindInstrument(), SoundPlayer.singleton().midiEventMan.midiEventListMelody);
        UnityEngine.Debug.Log("Perfect score: " + perfectPlayScore + "  / play score: " + AppContext.instance().totalScore);
        //scorePanelText.text = "" + AppContext.instance().totalScore + "/" + perfectPlayScore;
        if(perfectPlayScore > 0)
        {
            accuracy = 100 * AppContext.instance().totalScore / perfectPlayScore;
        } else
        {
            accuracy = 0;
        }

    }

    class EndAccuracyCallback : TimerCallback
    {
        PlayPerformance playPerformance;

        public EndAccuracyCallback(PlayPerformance playPerformance)
        {
            this.playPerformance = playPerformance;
        }

        public override void onTick(Timer timer, float tick, float timeOut)
        {
            long stepAcc = (long) (tick * playPerformance.accuracy / timeOut);
            playPerformance.scorePanel.GetComponent<Text>().text = "Accuracy: " + stepAcc + "%";
            
        }

        public override void onEnd(Timer timer)
        {
            playPerformance.scorePanel.GetComponent<Text>().text = "Accuracy: " + playPerformance.accuracy + "%";
        }

    }

}
