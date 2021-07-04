using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayPerformance : MonoBehaviour
{
    public GameObject songTitlePanel;
    public GameObject scorePanel;
    public GameObject passfailPanel;
    public GameObject restartButton;
    public GameObject songsButton;

    Text songTitlePanelText;
    Text scorePanelText;
    Text passfailPanelText;

    // Start is called before the first frame update
    void Start()
    {
        songTitlePanelText = songTitlePanel.GetComponentInChildren<Text>();
        scorePanelText = scorePanel.GetComponentInChildren<Text>();
        passfailPanelText = passfailPanel.GetComponentInChildren<Text>();

        restartButton.GetComponent<Button>().onClick.AddListener(restartSong);
        songsButton.GetComponent<Button>().onClick.AddListener(redirectSongList);

    }

    void restartSong()
    {
        SceneManager.LoadScene("PlayScene2D", LoadSceneMode.Single);
        AppContext.instance().totalScore = 0;
    }

    void redirectSongList()
    {
        SceneManager.LoadScene("SongList", LoadSceneMode.Single);
        AppContext.instance().totalScore = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            changeText();
        }


        //checkTouch();

    }

    void checkTouch()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == restartButton)
                {
                    //restart level
                    Debug.Log("restarted");
                }

                if (hit.collider.gameObject == songsButton)
                {
                    //go to songs list
                    Debug.Log("songs list");
                }

            }
        }
    }

    void changeText()
    {
        long perfectPlayScore = ScoreUtils.calculateTotalScore(AppContext.instance().isWindInstrument(), SoundPlayer.singleton().midiEventMan.midiEventListMelody);
        scorePanelText.text = "" + AppContext.instance().totalScore + "/" + perfectPlayScore;

        if (AppContext.instance().failed)
        {
            passfailPanelText.text = "Fail";
            passfailPanelText.color = Color.red;
        }
        else
        {
            passfailPanelText.text = "Pass";
            passfailPanelText.color = Color.green;
        }

        
    }

}
