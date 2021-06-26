using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimerCallback
{
    public abstract void onTick(float tick, float timeOut);
    public abstract void onEnd();
}

public class Timer : MonoBehaviour
{
    TimerCallback callback;
    float tick;
    float timeOut;

    bool active = false;

    public void startTimer(float timeOut, TimerCallback callback)
    {
        active = true;
        tick = 0;
        this.timeOut = timeOut;
        this.callback = callback;
    }

    void updateTimer()
    {
        if (active)
        {
            tick += Time.deltaTime;
            
            if (tick > timeOut)
            {
                tick = timeOut;
            }
            
            callback.onTick(tick, timeOut);

            if (tick >= timeOut)
            {
                active = false;
                callback.onEnd();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        updateTimer();
    }
}
