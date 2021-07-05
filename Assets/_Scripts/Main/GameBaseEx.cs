using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBaseEx : MonoBehaviour
{

    Camera mainCamera;
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
        _soundPlayer = SoundPlayer.singleton();
    }


   protected bool isOutOfScreen(Transform t, float yOffset)
    {
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        var pos = mainCamera.WorldToScreenPoint(t.position);
        pos.y += yOffset;
        bool outOfBounds = !Screen.safeArea.Contains(pos);
        if(outOfBounds)
        {
            UnityEngine.Debug.Log("OutofBounds pos: " + pos.ToString());
            UnityEngine.Debug.Log("OutofBounds area: " + Screen.safeArea.ToString());
        }

        return outOfBounds;
    }


    protected void setChildrenAlpha(Transform trans, float alpha) {
        foreach(Transform t in trans)
        {
            SpriteRenderer r = t.GetComponent<SpriteRenderer>();
            if(r!=null)
            {
                Color color = r.color;
                color.a = alpha;
                r.color = color;
                continue;
            }
            Renderer r2 = t.GetComponent<Renderer>();
            if (r2 != null)
            {
                Color color = r2.material.color;
                color.a = alpha;
                r2.material.color = color;
                continue;
            }
        }
    }


}
