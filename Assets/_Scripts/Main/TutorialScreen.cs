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
    public GameObject shortNote1;
    public GameObject shortNote2;
    public GameObject longNote;
    

    public List<string> notifs = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        
        setNotifs();
        shortNote1.SetActive(false);
        shortNote2.SetActive(false);
        longNote.SetActive(false);
        popupNextButton.GetComponent<Button>().onClick.AddListener(() => updatePopupText(0));
        PlayerData.seeTutorial();
    }

    void setNotifs()
    {
        notifs.Add("In this game, notes will fall down according the melody of the song.");
        notifs.Add("Tap at the precise time to gain more points. ");
        notifs.Add("There is a glowing yellow line that indicates the optimal time.");
        notifs.Add("For short notes like these, tap or press any of the WASD and arrow keys.");
        notifs.Add("For long notes like these, tap and hold the note or press any of the WASD and arrow keys.");
        /*notifs.Add("Here's an example of a short note: ");
        notifs.Add("short");
        notifs.Add("Here's an example of a long note: ");
        notifs.Add("long");*/
        notifs.Add("If you miss too many notes, you will fail the level.");
        notifs.Add("And that's all! Now begin playing!");
    }

    void updatePopupText(int index)
    {
        if (index == notifs.Count)
        {
            GameManager.backToPrevScene();
            return;
        }

        if (index == 2)
        {
            baseLine.SetActive(true);
        }

        if (index == 3)
        {
            shortNote1.SetActive(true);
            shortNote2.SetActive(true);
        } else if (index == 4)
        {
            longNote.SetActive(true);
            shortNote1.SetActive(false);
            shortNote2.SetActive(false);
        } else
        {
            longNote.SetActive(false);
            shortNote1.SetActive(false);
            shortNote2.SetActive(false);
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
