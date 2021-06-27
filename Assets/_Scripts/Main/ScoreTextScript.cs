using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextScript : MonoBehaviour
{
    public float fadeTime = 1.0f;

    Timer timerScoreText;
    Timer timerAlertText;

    public Text scoreNotifyText;
    public Text totalScoreText;
    public Text alertText;

    ScoreTextCallback scoreTextCallback;

    // Start is called before the first frame update
    void Start()
    {
        scoreTextCallback = new ScoreTextCallback(this);
        scoreNotifyText.text = "";
        totalScoreText.text = "";
        alertText.text = "";
    }

    // Update is called once per frame
    void Update()
    {

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
        scoreNotifyText.text = text;
    }

    public void updateAlertText(string text)
    {
        if (timerAlertText == null)
        {
            timerAlertText = this.gameObject.AddComponent<Timer>() as Timer;
        }
        timerAlertText.startTimer(fadeTime, scoreTextCallback);
        alertText.text = text;
    }


    class ScoreTextCallback : TimerCallback
    {
        ScoreTextScript scoreTextScript;
        public ScoreTextCallback(ScoreTextScript scoreTextScript)
        {
            this.scoreTextScript = scoreTextScript;
        }
        public override void onTick(Timer timer, float tick, float timeOut)
        {
            if (timer == scoreTextScript.timerScoreText)
            {
                //fade in
                Color color = scoreTextScript.scoreNotifyText.color;
                color.a = (timeOut - tick) / timeOut;
                scoreTextScript.scoreNotifyText.color = color;
                Debug.Log("reached onTick, tick = " + tick + " timeOut = " + timeOut);
            } else if(timer == scoreTextScript.timerAlertText)
            {
                Color color = scoreTextScript.alertText.color;
                color.a = (timeOut - tick) / timeOut;
                scoreTextScript.alertText.color = color;
                Debug.Log("reached 2 onTick, tick = " + tick + " timeOut = " + timeOut);
            }
        }
        public override void onEnd(Timer timer)
        {
            //fade out
        }
    }
}
