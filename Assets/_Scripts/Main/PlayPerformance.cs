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
    private float changeTextTime = 2.0f;
    bool isChangeAccuracyEnd;

    EndAccuracyCallback endAccuracyCallback;

    long accuracy;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {
        isChangeAccuracyEnd = false;
        accuracy = 0;
        songTitlePanel.GetComponent<Text>().text = AppContext.instance().songItem.title;

        restartButton.GetComponent<Button>().onClick.AddListener(restartSong);
        songsButton.GetComponent<Button>().onClick.AddListener(redirectSongList);

        endAccuracyCallback = new EndAccuracyCallback(this);
    }

    void restartSong()
    {
        SceneManager.LoadScene("PlayScene2D", LoadSceneMode.Single);
        AppContext.instance().totalScore = 0;
    }

    void redirectSongList()
    {
        SceneManager.LoadScene("SongList", LoadSceneMode.Single);
        AppContext.instance().totalScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            changeText();
        }
        if (!isChangeAccuracyEnd)
        {
            updateEndAccuracy();
        }
        //checkTouch();

    }

    void checkTouch()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == restartButton)
                {
                    //restart level
                    Debug.Log("restarted");
                }

                if (hit.collider.gameObject == songsButton)
                {
                    //go to songs list
                    Debug.Log("songs list");
                }

            }
        }
    }

    void updateEndAccuracy()
    {
        if (endAccuracyText == null)
        {
            endAccuracyText = this.gameObject.AddComponent<Timer>() as Timer;
        }
        endAccuracyText.startTimer(changeTextTime, endAccuracyCallback);
    }

    void changeText()
    {
        long perfectPlayScore = ScoreUtils.calculateTotalScore(AppContext.instance().isWindInstrument(), SoundPlayer.singleton().midiEventMan.midiEventListMelody);
        //scorePanelText.text = "" + AppContext.instance().totalScore + "/" + perfectPlayScore;

        accuracy = 100 * AppContext.instance().totalScore / perfectPlayScore;
        scorePanel.GetComponent<Text>().text = "Accuracy: " + accuracy + "%";

        if (AppContext.instance().failed)
        {
            passfailPanel.GetComponent<Text>().text = "Fail";
            passfailPanel.GetComponent<Text>().color = Color.red;
        }
        else
        {
            passfailPanel.GetComponent<Text>().text = "Pass";
            passfailPanel.GetComponent<Text>().color = Color.green;
        }

        
    }

    class EndAccuracyCallback : TimerCallback
    {
        PlayPerformance playPerformance;
        long accuracyTextAmt;

        public EndAccuracyCallback(PlayPerformance playPerformance)
        {
            this.playPerformance = playPerformance;
        }

        public override void onTick(Timer timer, float tick, float timeOut)
        {
            if (accuracyTextAmt < playPerformance.accuracy)
            {
                accuracyTextAmt++;
                playPerformance.scorePanel.GetComponent<Text>().text = "Accuracy: " + accuracyTextAmt + "%";
            } else 
            {
                onEnd(timer);
            }
        }

        public override void onEnd(Timer timer)
        {
            playPerformance.isChangeAccuracyEnd = true;
        }

    }

}
