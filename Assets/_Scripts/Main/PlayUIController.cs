using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayUIController : GameBaseEx
{
    private float fadeTime = 0.5f;

    Timer timerScoreText;
    Timer timerAlertText;

    public Transform scoreNotifyText;
    public Transform alertText;
    public Text totalScoreText;

    public GameObject skipButton;
    public GameObject skipButtonText;
    public GameObject backButton;
    public GameObject countdownText;
    public GameObject resumeObject;
    public GameObject resumeButton;



    private Vector3 scoreNotifyTextPos;
    private Vector3 alertTextPos;

    ScoreTextCallback scoreTextCallback;

    bool isInPrelude;

    // Start is called before the first frame update
    void Start()
    {

        isInPrelude = true;

        scoreTextCallback = new ScoreTextCallback(this);
        scoreNotifyText.GetComponent<Text>().text = "";
        totalScoreText.text = "";
        alertText.GetComponent<Text>().text = "";
        scoreNotifyTextPos = scoreNotifyText.localPosition;
        alertTextPos = alertText.localPosition;

        Button skip = skipButton.GetComponent<Button>();
        skip.onClick.AddListener(skipIntro);

        Button back = backButton.GetComponent<Button>();
        back.onClick.AddListener(backToSongList);

        Button resume = resumeButton.GetComponent<Button>();
        resume.onClick.AddListener(resumePlay);

    }

    // Update is called once per frame
    void Update()
    {

        if (isInPrelude)
        {
            disableSkipButton();
            countdown();
        }
    }



    void countdown()
    {
        long countdownTime = ((soundPlayer.getFirstMelodyTime() - AppContext.instance().musicNoteDisplayDuration / 2) - soundPlayer.playTime)/1000;

        countdownText.GetComponent<Text>().text = "" + countdownTime;
    }

    void disableSkipButton()
    {
        if (soundPlayer.getFirstMelodyTime() - AppContext.instance().musicNoteDisplayDuration / 2 - soundPlayer.playTime <= 0)
        {
            skipButton.SetActive(false);
            skipButtonText.SetActive(false);
            isInPrelude = false;
            countdownText.SetActive(false);
        }
    }

    public void pausePlay()
    {
        UnityEngine.Debug.Log("PausePlay");
        if (!soundPlayer.isPlayFinished && !soundPlayer.isPause)
        {
            UnityEngine.Debug.Log("PausePlay!!!!!");
            resumeObject.SetActive(true);
            soundPlayer.pausePlay();
        }
    }

    void resumePlay()
    {
        UnityEngine.Debug.Log("Resume play!!!!!");
        soundPlayer.startPlay(true);
        resumeObject.SetActive(false);
    }

    void skipIntro()
    {
        soundPlayer.skipPrelude(true);
    }

    void backToSongList()
    {
        SoundPlayer.singleton().pausePlay();
        GameManager.gotoSongList();
    }


    public void updateTotalScoreTexts(string text)
    {
        totalScoreText.text = text;
    }

    public void updateScoreTexts(string text)
    {
        if(timerScoreText == null)
        {
            timerScoreText = this.gameObject.AddComponent<Timer>() as Timer;
        }
        timerScoreText.startTimer(fadeTime, scoreTextCallback);
        scoreNotifyText.GetComponent<Text>().text = text;
    }

    public void updateAlertText(string text)
    {
        if (timerAlertText == null)
        {
            timerAlertText = this.gameObject.AddComponent<Timer>() as Timer;
        }
        timerAlertText.startTimer(fadeTime, scoreTextCallback);
        alertText.GetComponent<Text>().text = text;
    }


    class ScoreTextCallback : TimerCallback
    {
        PlayUIController controller;
        public ScoreTextCallback(PlayUIController controller)
        {
            this.controller = controller;
        }
        public override void onTick(Timer timer, float tick, float timeOut)
        {
            Vector3 originalPos;
            Transform updateText;
            Color orignalColor;
            if (timer == controller.timerScoreText)
            {
                originalPos = controller.scoreNotifyTextPos;
                updateText = controller.scoreNotifyText;
                
            } else if(timer == controller.timerAlertText)
            {
                originalPos = controller.alertTextPos;
                updateText = controller.alertText;
            } else
            {
                return;
            }
            //fade in
            Color color = updateText.GetComponent<Text>().color;
            color.a = (timeOut - tick) / timeOut;
            updateText.GetComponent<Text>().color = color;
            //move
            Vector3 targetPos = originalPos;
            targetPos.y += 70 *  tick / timeOut;
            updateText.localPosition = targetPos;
            
        }
        public override void onEnd(Timer timer)
        {
            //fade out
        }
    }
}
