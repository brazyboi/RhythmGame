using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextScript : MonoBehaviour
{
    private float fadeTime = 0.5f;

    Timer timerScoreText;
    Timer timerAlertText;

    public Transform scoreNotifyText;
    public Transform alertText;
    public Text totalScoreText;

    private Vector3 scoreNotifyTextPos;
    private Vector3 alertTextPos;

    ScoreTextCallback scoreTextCallback;

    // Start is called before the first frame update
    void Start()
    {
        scoreTextCallback = new ScoreTextCallback(this);
        scoreNotifyText.GetComponent<Text>().text = "";
        totalScoreText.text = "";
        alertText.GetComponent<Text>().text = "";
        scoreNotifyTextPos = scoreNotifyText.localPosition;
        alertTextPos = alertText.localPosition;
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
        ScoreTextScript scoreTextScript;
        public ScoreTextCallback(ScoreTextScript scoreTextScript)
        {
            this.scoreTextScript = scoreTextScript;
        }
        public override void onTick(Timer timer, float tick, float timeOut)
        {
            Vector3 originalPos;
            Transform updateText;
            Color orignalColor;
            if (timer == scoreTextScript.timerScoreText)
            {
                originalPos = scoreTextScript.scoreNotifyTextPos;
                updateText = scoreTextScript.scoreNotifyText;
                
            } else if(timer == scoreTextScript.timerAlertText)
            {
                originalPos = scoreTextScript.alertTextPos;
                updateText = scoreTextScript.alertText;
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
            Debug.Log("reached onTick, tick = " + tick + " timeOut = " + timeOut);
        }
        public override void onEnd(Timer timer)
        {
            //fade out
        }
    }
}
