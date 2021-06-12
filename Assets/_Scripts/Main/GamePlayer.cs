using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : GameBaseEx
{

  
    public GameObject musicNote;
    public GameObject drumNote;
    // Use this for initialization
    void Start()
    {

        init();
        startPlay();
    }

    void init()
    {
        AppContext.instance().musicNoteDisplayDuration = 3500;
        soundPlayer.playerDelegate = new Player3DDelegate(this);
        soundPlayer.setPlayMode(SoundPlayer.NON_STOP_TAP_PLAY);
        soundPlayer.setMelodyMute(true);
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
        GameObject noteObject;
        noteObject = Instantiate(musicNote, new Vector3(0, 0, 0), Quaternion.identity);
        MusicNoteController c = noteObject.GetComponent<MusicNoteController>();
        c.Note = note;
    }

    void placeDrumExplosion(long size)
    {
        switch (size)
        {
            case SoundPlayer.DRUM_HIT_HEAVY:
                {
                    GameObject drumExplosion;
                    drumExplosion = Instantiate(drumNote, new Vector3(0, 0, 0), Quaternion.identity);
                    DrumNoteController c = drumExplosion.GetComponent<DrumNoteController>();
                    c.explosionTime = soundPlayer.playTime;
                }
                break;
            case SoundPlayer.DRUM_HIT_MEDIUM:
                {
                    GameObject drumExplosion;
                    drumExplosion = Instantiate(drumNote, new Vector3(0, 0, 0), Quaternion.identity);
                    DrumNoteController c = drumExplosion.GetComponent<DrumNoteController>();
                    c.explosionTime = soundPlayer.playTime;
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
        GamePlayer gamePlayer;

        public Player3DDelegate(GamePlayer c)
        {
            gamePlayer = c;
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
            gamePlayer.placeNote(midiEvent);
        }
        public override void playEvent(int eventId, long info1, long info2)
        {
            //UnityEngine.Debug.Log("playEvent: " + eventId);
            switch (eventId)
            {
                case SoundPlayer.PLAY_EVENT_FIRE_SOLOPLAY_NOTE://PLAY_EVENT_FIRE_NOTE:
                    gamePlayer.playMelodyNoteNotify();

                    break;
                case SoundPlayer.PLAY_EVENT_FIRE_DRUM_NOTE:
                    gamePlayer.placeDrumExplosion(info1);
                    break;
            }


        }
    }

}
