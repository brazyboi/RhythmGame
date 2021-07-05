using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreDelegate
{
    public abstract void updateScore(string text, long score, bool final);
    public abstract void updateSuperScore(string text, long score);
    public abstract void missPlayNote(long missScore);
}

public class GamePlayer : GameBaseEx
{
    public GameObject musicNote;

    public GameObject playerUI;
    public GameObject drumNote;
    public GameObject playPerformance;
    public GameObject musicNotesParent;
    public GameObject cameraContainer;
    NoteScoreDelegate noteScoreDelegate;

    AppContext appContext;

    // Use this for initialization
    void Start()
    {
        appContext = AppContext.instance();
        init();
        startPlay();
    }

    void Update()
    {
        float deltaTick = Time.deltaTime;
        soundPlayer.timerUpdate(deltaTick);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        UnityEngine.Debug.Log("OnApplicationFocus!");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        UnityEngine.Debug.Log("OnApplicationPause! pauseStatus=" + pauseStatus);
        if (pauseStatus)
        {
        //    soundPlayer.stopAllNote(0, 0);
        //    soundPlayer.pausePlay();
        } else
        {

        }
        //TODO: show pause UI
    }

    void init()
    {
        noteScoreDelegate = new NoteScoreDelegate(this);
        appContext.musicNoteDisplayDuration = 3500;
        soundPlayer.playerDelegate = new Player3DDelegate(this);
        soundPlayer.setPlayMode(SoundPlayer.NON_STOP_TAP_PLAY);
        soundPlayer.setMelodyMute(soundPlayer.getPlayMode() != SoundPlayer.LEARN_PLAY);
    }


    void startPlay()
    {
        /*
        string name = appContext.songItem.path.Replace(".sht", ".mid");
        string fileLocation = Application.streamingAssetsPath + "/songs/" + name;
        soundPlayer.loadMusic(fileLocation, false, appContext.songItem.melody);
        */
        soundPlayer.adjustBaseNoteByInstrument();
        appContext.playingNote = false;
        foreach (Transform child in this.musicNotesParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        string fileLocation;
        int melodyChannel = 0;
        if (appContext.songItem == null)
        {
            fileLocation = "songs/tian_kong_zhi_cheng.mid";
            melodyChannel = 0;
        } else
        {
            string name = appContext.songItem.path.Replace(".sht", ".mid");
            fileLocation =  "songs/" + name;
            melodyChannel = appContext.songItem.melody;
            UnityEngine.Debug.Log("filelocation: " + fileLocation);
        }
        soundPlayer.loadMusic(fileLocation, false, melodyChannel);
        soundPlayer.seek(0);
        soundPlayer.startPlay(false);

        /* test distance of note between 100ms.
        soundPlayer.pausePlay();
        for (int i=0;i<10; i++)
        {
            MusicNote note = new MusicNote();
            note.tick = i * 200;
            note.elapseTime = 50;
            note.tickGapNext = 50;
            placeNote(note);
        } */

    }

    void placeNote(MusicNote note)
    {
        GameObject noteObject;
        noteObject = Instantiate(musicNote, new Vector3(0, 0, 0), Quaternion.identity, musicNotesParent.transform);
        MusicNoteController c = noteObject.GetComponent<MusicNoteController>();
        c.Note = note;
        c.scoreDelegate = noteScoreDelegate;
    }

    GameObject createDrumNote(bool reuseOld)
    {
        if(reuseOld)
        {
            Transform t;
            t = cameraContainer.transform.Find("DrumNoteReuse");
            if (t != null)
            {
                return t.gameObject;
            }
        }
        GameObject o = Instantiate(drumNote, new Vector3(0, 0, 0), Quaternion.identity);
        o.transform.SetParent(cameraContainer.transform);
        o.transform.localPosition = new Vector3(0, 10, 0);
        if (reuseOld)
        {
            o.name = "DrumNoteReuse";
        } else
        {
            o.name = "DrumNoteNew";
        }
        return o;

    }

    void placeDrumExplosion(long size)
    {
        switch (size)
        {
            case SoundPlayer.DRUM_HIT_HEAVY:
                {
                    createDrumNote(false).GetComponent<DrumNoteScript>().restartDrumNote(DrumNoteScript.HEAVY_DRUM_SCALE, true);
                }
                break;
            case SoundPlayer.DRUM_HIT_MEDIUM:
                {
                    createDrumNote(true).GetComponent<DrumNoteScript>().restartDrumNote(DrumNoteScript.LIGHT_DRUM_SCALE, false);
                }
                break;
        }

    }

    void playMelodyNoteNotify()
    {
        //hitText.GetComponent<TextHitEffect>().hit("GREAT!!!!");
        //hitScore += 100;
    }

    void onGameFailed()
    {
        //pause player
        
        soundPlayer.pausePlay();
        AppContext.instance().failed = true;
        Instantiate(playPerformance, new Vector3(0, 0, 0), Quaternion.identity);
        foreach (Transform child in this.musicNotesParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject baseLine = GameObject.FindGameObjectWithTag("BaseLine");
        baseLine.SetActive(false);

    }

    void onGamePlayCompleted()
    {
        soundPlayer.pausePlay();
        AppContext.instance().failed = false;
        Instantiate(playPerformance, new Vector3(0, 0, 0), Quaternion.identity);
        foreach (Transform child in this.musicNotesParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject baseLine = GameObject.FindGameObjectWithTag("BaseLine");
        baseLine.SetActive(false);

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
                case SoundPlayer.PLAY_EVENT_COMPLETE:
                    gamePlayer.onGamePlayCompleted();
                    break;
            }


        }
    }

    class NoteScoreDelegate : ScoreDelegate
    {
        GamePlayer gamePlayer;
        PlayUIController playUIController;
        public NoteScoreDelegate(GamePlayer c)
        {
            gamePlayer = c;
            playUIController = gamePlayer.playerUI.GetComponent<PlayUIController>();
        }
        public override void updateScore(string text, long score, bool final)
        {
            if (final)
            {
                gamePlayer.appContext.totalScore += score;
                playUIController.updateTotalScoreTexts("" + gamePlayer.appContext.totalScore);
            }
            playUIController.updateScoreTexts(text + "\n\r" + score);

        }
        public override void updateSuperScore(string text, long score)
        {
            gamePlayer.appContext.totalScore += score;
            playUIController.updateScoreTexts(text + "\n\r" + score);
            // scoreTextScript.updateAlertText(text + "\n\r" + score);

        }
        public override void missPlayNote(long missScore)
        {
            
            gamePlayer.appContext.totalScore -= missScore;
            if(gamePlayer.appContext.totalScore < 0)
            {
                gamePlayer.appContext.totalScore = 0;
            }
            playUIController.updateTotalScoreTexts("" + gamePlayer.appContext.totalScore);
            playUIController.updateScoreTexts("MISS" + "\n\r" + missScore);
           // gamePlayer.onGameFailed();
        }

    }

}
