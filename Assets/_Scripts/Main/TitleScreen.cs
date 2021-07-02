using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public GameObject playButton;
    public GameObject rewardButton;
    public GameObject shareButton;

    // Start is called before the first frame update
    void Start()
    {
        playButton.GetComponent<Button>().onClick.AddListener(startGame);
        rewardButton.GetComponent<Button>().onClick.AddListener(rewards);
    }

    void startGame()
    {
        SceneManager.LoadScene("SongList", LoadSceneMode.Single);
    }

    void rewards()
    {
        SceneManager.LoadScene("RewardScreen", LoadSceneMode.Single);
    }

    void share()
    {
        // show popup for share
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
