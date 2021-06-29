using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreDelegate
{
    public abstract void updateScore(long score, bool final);
    public abstract void updateSuperScore(long score);
    public abstract void missPlayNote(long missScore);
}

public class GamePlayer : GameBaseEx
{
    public GameObject musicNote;
    public GameObject drumNote;

    public GameObject inGameScores;

    public GameObject playPerformance;

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
        
    }

    void init()
    {
        noteScoreDelegate = new NoteScoreDelegate(this);
        appContext.musicNoteDisplayDuration = 3500;
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
        string fileLocation;
        int melodyChannel = 0;
        if (appContext.songItem == null)
        {
            fileLocation = Application.streamingAssetsPath + "/songs/tian_kong_zhi_cheng.mid";
            melodyChannel = 0;
        } else
        {
            string name = appContext.songItem.path.Replace(".sht", ".mid");
            fileLocation = Application.streamingAssetsPath + "/songs/" + name;
            melodyChannel = appContext.songItem.melody;
            UnityEngine.Debug.Log("filelocation: " + fileLocation);
        }
        soundPlayer.loadMusic(fileLocation, false, melodyChannel);
        soundPlayer.seek(0);
        soundPlayer.startPlay(false);
    }

    void placeNote(MusicNote note)
    {
        GameObject noteObject;
        noteObject = Instantiate(musicNote, new Vector3(0, 0, 0), Quaternion.identity);
        MusicNoteController c = noteObject.GetComponent<MusicNoteController>();
        c.Note = note;
        c.scoreDelegate = noteScoreDelegate;
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

    void onGameFailed()
    {
        //pause player
        
        soundPlayer.pausePlay();
        AppContext.instance().failed = true;

        playPerformance.SetActive(true);

        GameObject[] notes = GameObject.FindGameObjectsWithTag("MusicNote");
        foreach (GameObject note in notes)
        {
            note.SetActive(false);
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
            }


        }
    }

    class NoteScoreDelegate : ScoreDelegate
    {
        GamePlayer gamePlayer;
        ScoreTextScript scoreTextScript;
        public NoteScoreDelegate(GamePlayer c)
        {
            gamePlayer = c;
            scoreTextScript = gamePlayer.inGameScores.GetComponent<ScoreTextScript>();
        }
        public override void updateScore(long score, bool final)
        {
            if (final)
            {
                gamePlayer.appContext.totalScore += score;
                scoreTextScript.updateTotalScoreTexts("" + gamePlayer.appContext.totalScore);
            }
            scoreTextScript.updateScoreTexts("" + score);

        }
        public override void updateSuperScore(long score)
        {
            
        }
        public override void missPlayNote(long missScore)
        {
            
            gamePlayer.appContext.totalScore -= missScore;
            if(gamePlayer.appContext.totalScore < 0)
            {
                gamePlayer.appContext.totalScore = 0;
            }
            scoreTextScript.updateTotalScoreTexts("" + gamePlayer.appContext.totalScore);
            scoreTextScript.updateScoreTexts("MISS");
            gamePlayer.onGameFailed();
        }

    }

}
