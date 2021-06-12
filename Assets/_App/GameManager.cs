using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SoundPlayer soundPlayer;
    public AppContext appContext;
    public long score;
    public float speed = 1.0f;
    // Use this for initialization
    void Start()
    {
        appContext = AppContext.instance();
        initSoundPlayer();
    }

    void initSoundPlayer()
    {
        soundPlayer = SoundPlayer.singleton();
        soundPlayer.setPlayMode(SoundPlayer.NON_STOP_TAP_PLAY);
        soundPlayer.setMelodyMute(true);
    }

    void Update()
    {
        float deltaTick = Time.deltaTime;
        soundPlayer.timerUpdate(deltaTick);

    }


}
