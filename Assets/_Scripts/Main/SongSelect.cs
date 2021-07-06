using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongSelect : MonoBehaviour
{
    public GameObject songTitlePanel;
    public GameObject playButton;
    public GameObject selectButton;
    public GameObject instrumentImage;
    public GameObject backButton;

    Image image;

    public Sprite instrument1;
    public Sprite instrument2;
    public Sprite instrument3;
    public Sprite instrument4;
    public Sprite instrument5;
    public Sprite instrument6;
    public Sprite instrument7;
    public Sprite instrument8;
    public Sprite instrument9;
    public Sprite instrument10;
    public Sprite instrument11;
    public Sprite instrument12;

    Text songTitlePanelText;

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {
        songTitlePanelText = songTitlePanel.GetComponent<Text>();
        songTitlePanelText.text = AppContext.instance().songItem.title;

        image = instrumentImage.GetComponent<Image>();

        setInstrumentImage();

        playButton.GetComponent<Button>().onClick.AddListener(playGame);
        selectButton.GetComponent<Button>().onClick.AddListener(instrumentSelect);
        backButton.GetComponent<Button>().onClick.AddListener(backToSongList);
    }

    void backToSongList()
    {
        GameManager.gotoSongList();
    }

    void instrumentSelect() 
    {
        GameManager.prevScene = "SongSelect";
        GameManager.gotoRewardScreen();
    }

    void playGame()
    {
        GameManager.gotoPlayScene();
    }

    void setInstrumentImage()
    {
        switch (AppContext.instance().getInstrument())
        {
            case MusicInstrument.PIANO_INSTRUMENT:
                image.sprite = instrument1;
                break;
            case MusicInstrument.FLUTE_INSTRUMENT:
                image.sprite = instrument2;
                break;
            case MusicInstrument.SAX_INSTRUMENT:
                image.sprite = instrument3;
                break;
            case MusicInstrument.HARP_INSTRUMENT:
                image.sprite = instrument4;
                break;
            case MusicInstrument.TRUMPET_INSTRUMENT:
                image.sprite = instrument5;
                break;
            case MusicInstrument.HARMONICA_INSTRUMENT:
                image.sprite = instrument6;
                break;
            case MusicInstrument.GUITAR_INSTRUMENT:
                image.sprite = instrument7;
                break;
            case MusicInstrument.ELECTRIC_GUITAR_INSTRMENT:
                image.sprite = instrument8;
                break;
            case MusicInstrument.ACCORDION_INSTRUMENT:
                image.sprite = instrument9;
                break;
            case MusicInstrument.DIZI_INSTRUMENT:
                image.sprite = instrument10;
                break;
            case MusicInstrument.OCARINA_INSTRUMENT:
                image.sprite = instrument11;
                break;
            case MusicInstrument.XIAO_INSTRUMENT:
                image.sprite = instrument12;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
