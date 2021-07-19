using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreen : GameBaseEx
{
    public GameObject popupNextButton;
    public GameObject popupText;
    public GameObject popupPrefab;
    public GameObject baseLine;
    //public GameObject tapNotif
    

    public List<string> notifs = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
        setNotifs();
        popupNextButton.GetComponent<Button>().onClick.AddListener(() => updatePopupText(0));
    }

    void setNotifs()
    {
        notifs.Add("In this game, notes will fall down according the melody of the song.");
        notifs.Add("Tap at the precise time to gain more points. ");
        notifs.Add("There is a glowing yellow line that indicates the optimal time.");
        /*notifs.Add("Here's an example of a short note: ");
        notifs.Add("short");
        notifs.Add("Here's an example of a long note: ");
        notifs.Add("long");*/
        notifs.Add("And that's all! Now begin playing!");
    }

    void updatePopupText(int index)
    {
        if (index == notifs.Count)
        {
            GameManager.backToPrevScene();
            return;
        }

        /*if (notifs[index] == "short")
        {
            popupPrefab.SetActive(false);
            placeShortNote();
            baseLine.SetActive(true);
        }
        else if (notifs[index] == "long")
        {
            popupPrefab.SetActive(false);
            placeLongNote();
            baseLine.SetActive(true);
        }
        elseif (notifs[index] == "tutorial play")
        {
            
            popupPrefab.SetActive(false);
            playTutorialSong(); 
        }
        else
        {
            popupPrefab.SetActive(true);
            baseLine.SetActive(false);
        }*/

        popupText.GetComponent<Text>().text = notifs[index];

        index++;

        popupNextButton.GetComponent<Button>().onClick.AddListener(() => updatePopupText(index));
    }

    void placeShortNote()
    {
        //MusicNote n = new MusicNote();
        //n.tick = 2000;
        
    }

    void placeLongNote()
    {

    }

    void playTutorialSong()
    {
        //AppContext.instance().songItem = ;
        GameManager.gotoPlayScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
