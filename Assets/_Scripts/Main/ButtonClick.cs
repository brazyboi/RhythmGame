using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void restartSong()
    {
        SceneManager.LoadScene("PlayScene2D", LoadSceneMode.Single);
    }

    public void redirectSongList()
    {
        SceneManager.LoadScene("SongList", LoadSceneMode.Single);
    }

}
