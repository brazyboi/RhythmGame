using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private SoundPlayer soundPlayer;

    public SoundPlayer getSoundPlayer() {
        return soundPlayer;
    }

    void Start()
    {
        soundPlayer = SoundPlayer.singleton();
    }

    // Update is called once per frame
    void Update()
    {
        soundPlayer.timerUpdate(Time.deltaTime);
    }

}
