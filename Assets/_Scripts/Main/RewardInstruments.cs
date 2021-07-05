using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RewardInstruments : MonoBehaviour
{

    /*Texture2D[] itemImages;
    Sprite[] itemSprites;

    int[] xPoses;
    int[] yPoses;*/

    public GameObject instrument1;
    public GameObject instrument2;
    public GameObject instrument3;
    public GameObject instrument4;
    public GameObject instrument5;
    public GameObject instrument6;
    public GameObject instrument7;
    public GameObject instrument8;
    public GameObject instrument9;
    public GameObject instrument10;
    public GameObject instrument11;
    public GameObject instrument12;

    GameObject[] itemButtons;

    int[] musicIntruments;

    public GameObject backButton;

    // Start is called before the first frame update
    void Start()
    {

        setMusicInstruments();

        setUpItemButtons();

        backButton.GetComponent<Button>().onClick.AddListener(backToPrevScene);
    }

    void backToPrevScene()
    {
        GameManager.backToPrevScene();
    }

    void setMusicInstruments()
    {           
        musicIntruments = new int[12];

        musicIntruments[0] = MusicInstrument.PIANO_INSTRUMENT;
        musicIntruments[1] = MusicInstrument.FLUTE_INSTRUMENT;
        musicIntruments[2] = MusicInstrument.SAX_INSTRUMENT;
        musicIntruments[3] = MusicInstrument.HARP_INSTRUMENT;
        musicIntruments[4] = MusicInstrument.TRUMPET_INSTRUMENT;
        musicIntruments[5] = MusicInstrument.HARMONICA_INSTRUMENT;
        musicIntruments[6] = MusicInstrument.GUITAR_INSTRUMENT;
        musicIntruments[7] = MusicInstrument.ELECTRIC_GUITAR_INSTRMENT;
        musicIntruments[8] = MusicInstrument.ACCORDION_INSTRUMENT;
        musicIntruments[9] = MusicInstrument.DIZI_INSTRUMENT;
        musicIntruments[10] = MusicInstrument.OCARINA_INSTRUMENT;
        musicIntruments[11] = MusicInstrument.XIAO_INSTRUMENT;
    }

    void setUpItemButtons()
    {
        itemButtons = new GameObject[12];
        itemButtons[0] = instrument1;
        itemButtons[1] = instrument2;
        itemButtons[2] = instrument3;
        itemButtons[3] = instrument4;
        itemButtons[4] = instrument5;
        itemButtons[5] = instrument6;
        itemButtons[6] = instrument7;
        itemButtons[7] = instrument8;
        itemButtons[8] = instrument9;
        itemButtons[9] = instrument10;
        itemButtons[10] = instrument11;
        itemButtons[11] = instrument12;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            int index = i;
            itemButtons[i].GetComponent<Button>().onClick.AddListener(delegate { onSelectInstrument(index); });
        }
    }

    void onSelectInstrument(int index)
    {

        AppContext.instance().setInstrument(musicIntruments[index]);
        Debug.Log("index: " + index + " Current instrument index: " + AppContext.instance().getInstrument());
        backToPrevScene();
    }

    /*void initReward()
    {
        itemImages = Resources.LoadAll("InstrumentImages") as Texture2D[];
        Debug.Log(itemImages.Length);
        itemSprites = new Sprite[itemImages.Length];
        int counter = 0;
        foreach (Texture2D texture in itemImages)
        {
            itemSprites[counter] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            counter++;
        }
        determineXPoses();
        determineYPoses();
    }

    void determineXPoses()
    {
        int numRow = Screen.width / 100 - 1;
        xPoses = new int[numRow];
        for (int i = 0; i < xPoses.Length; i++)
        {
            xPoses[i] = 50 + i * 100;
        }
    }

    void determineYPoses()
    { 
        
    }

    void createInstruments()
    {


        //int[] yPoses = {}
        
        for (int i = 0; i < itemImages.Length; i++)
        {
            instrumentSelectBox.GetComponent<Image>().sprite = itemSprites[i];
            Instantiate(instrumentSelectBox, new Vector3(xPoses[i % xPoses.Length], 50f, 0), Quaternion.identity);
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
