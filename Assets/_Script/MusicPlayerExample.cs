﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayerExample : MonoBehaviour
{
    
    SoundPlayer soundPlayer;
    AppContext appContext;
    // Use this for initialization
    void Start()
    {
        appContext = AppContext.instance();

        initSoundPlayer();
        startPlay();
    }


    void initSoundPlayer() {
        soundPlayer = SoundPlayer.singleton();
        soundPlayer.playerDelegate = new Player3DDelegate(this);
        soundPlayer.setPlayMode(SoundPlayer.LEARN_PLAY);
        soundPlayer.setMelodyMute(false);
    }

    // Update is called once per frame
    void Update()
    {
       
        float deltaTick = Time.deltaTime;
        soundPlayer.timerUpdate(deltaTick);

    }

    void startPlay()
    {
        /*
        string name = appContext.songItem.path.Replace(".sht", ".mid");
        string fileLocation = Application.streamingAssetsPath + "/songs/" + name;
        soundPlayer.loadMusic(fileLocation, false, appContext.songItem.melody);
        */
        string fileLocation = Application.streamingAssetsPath + "/songs/tian_kong_zhi_cheng.mid";
        soundPlayer.loadMusic(fileLocation, false, 0);
        soundPlayer.seek(0);
        soundPlayer.startPlay(false);
    }

 
    void placeNote(MusicNote note)
    {
       
    }

    void placeDrumExplosion(long size)
    {
        switch (size)
        {
            case SoundPlayer.DRUM_HIT_HEAVY:
                {
                    
                }
                break;
            case SoundPlayer.DRUM_HIT_MEDIUM:
                {
                    
                }
                break;
        }

    }

    void playMelodyNoteNotify()
    {
        //hitText.GetComponent<TextHitEffect>().hit("GREAT!!!!");
        //hitScore += 100;
    }

    class Player3DDelegate : PianoPlayerDelegate
    {
        MusicPlayerExample controller;
        public Player3DDelegate(MusicPlayerExample c)
        {
            controller = c;
        }
        public override void playNoteStart()
        {

        }
        public override void playNoteEnd()
        {

        }
        public override void playNoteNotify(MusicNote midiEvent)
        {
            //UnityEngine.Debug.Log("playNoteNotify: " + midiEvent.value + " tick:" + midiEvent.tick);
        }
        public override void playComingNoteNotify(MusicNote midiEvent)
        {
            controller.placeNote(midiEvent);
        }
        public override void playEvent(int eventId, long info1, long info2)
        {
            //UnityEngine.Debug.Log("playEvent: " + eventId);
            switch (eventId)
            {
                case SoundPlayer.PLAY_EVENT_FIRE_SOLOPLAY_NOTE://PLAY_EVENT_FIRE_NOTE:
                    controller.playMelodyNoteNotify();
                    break;
                case SoundPlayer.PLAY_EVENT_FIRE_DRUM_NOTE:
                    controller.placeDrumExplosion(info1);
                    break;
            }


        }
    }
}


