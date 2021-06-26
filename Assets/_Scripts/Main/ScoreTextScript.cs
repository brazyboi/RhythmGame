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
        timer.startTimer(fadeTime, scoreTextCallback);

        scoreNotifyText = scoreNotify.GetComponentInChildren<Text>();
        totalScoreText = totalScore.GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateScoreTexts(long noteScore, bool final)
    {
        timer.startTimer(fadeTime, scoreTextCallback);
        scoreNotifyText.text = "" + noteScore;
        if (final)
        {
            AppContext.instance().totalScore += noteScore;
            totalScoreText.text = "Score: " + AppContext.instance().totalScore;
        }
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
