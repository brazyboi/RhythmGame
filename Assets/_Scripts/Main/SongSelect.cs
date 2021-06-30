using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongSelect : MonoBehaviour
{
    public GameObject songTitlePanel;
    public GameObject playButton;

    Text songTitlePanelText;

    // Start is called before the first frame update
    void Start()
    {
        songTitlePanelText = songTitlePanel.GetComponent<Text>();
        playButton.GetComponent<Button>().onClick.AddListener(playGame);
    }

    void playGame()
    {
        SceneManager.LoadScene("PlayScene2D", LoadSceneMode.Single);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
