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

    Text songTitlePanelText;

    // Start is called before the first frame update
    void Start()
    {
        songTitlePanelText = songTitlePanel.GetComponent<Text>();
        playButton.GetComponent<Button>().onClick.AddListener(playGame);
        selectButton.GetComponent<Button>().onClick.AddListener(instrumentSelect);
    }

    void instrumentSelect()
    {
        AppContext.instance().prevScene = "SongSelect";
        SceneManager.LoadScene("RewardScreen", LoadSceneMode.Single);
    }

    void playGame()
    {
        UnityEngine.Debug.Log("Play click");
        SceneManager.LoadScene("PlayScene2D", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
