using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            changeText();
        }


        checkTouch();

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
        scorePanelText.text = "Score: " + AppContext.instance().totalScore;

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
