using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBaseEx : MonoBehaviour
{


    public GameManager gameManager
    {
        get
        {
            if (_gameManager == null)
            {
                init();
            }
            return _gameManager;
        }
    }

    public SoundPlayer soundPlayer
    {
        get
        {
            if (_soundPlayer == null)
            {
                init();
            }
            return _soundPlayer;
        }
    }

    private GameManager _gameManager = null;
    private SoundPlayer _soundPlayer = null;
    // Start is called before the first frame update
    private void init()
    {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _soundPlayer = gameManager.soundPlayer;
    }

  
}
