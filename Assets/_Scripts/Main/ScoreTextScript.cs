using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextScript : MonoBehaviour
{
    public GameObject scoreNotify;
    public GameObject totalScore;

    public float fadeTime = 1.0f;

    Timer timer;

    Text scoreNotifyText;
    Text totalScoreText;

    ScoreTextCallback scoreTextCallback;

    // Start is called before the first frame update
    void Start()
    {
        scoreTextCallback = new ScoreTextCallback(this);
        timer = GetComponent<Timer>();

        scoreNotifyText = scoreNotify.GetComponentInChildren<Text>();
        totalScoreText = totalScore.GetComponentInChildren<Text>();
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
        timer.startTimer(fadeTime, scoreTextCallback);
        scoreNotifyText.text = text;
    }

    void fadeOut(float tick, float timeOut)
    {
        Color color = scoreNotifyText.color;
        color.a = (timeOut-tick)/timeOut;
        scoreNotifyText.color = color;
    }
    

    class ScoreTextCallback : TimerCallback
    {
        ScoreTextScript scoreTextScript;
        public ScoreTextCallback(ScoreTextScript scoreTextScript)
        {
            this.scoreTextScript = scoreTextScript;
        }
        public override void onTick(float tick, float timeOut)
        {
            //fade in
            scoreTextScript.fadeOut(tick, timeOut);
            Debug.Log("reached onTick, tick = " + tick + " timeOut = " + timeOut);
        }
        public override void onEnd()
        {
            //fade out
        }
    }
}
