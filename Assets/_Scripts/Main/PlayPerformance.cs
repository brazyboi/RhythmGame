using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPerformance : MonoBehaviour
{
    public GameObject songTitlePanel;
    public GameObject scorePanel;
    public GameObject passfailPanel;

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
            scorePanelText.text = "Score: " + AppContext.instance().totalScore;

            if (AppContext.instance().failed)
            {
                passfailPanelText.text = "Fail";
                passfailPanelText.color = Color.red;
            } else
            {
                passfailPanelText.text = "Pass";
                passfailPanelText.color = Color.green;
            }

        }
        
    }

}
