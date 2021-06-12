using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCamera: GameBaseEx
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, gameManager.speed * soundPlayer.playTime / 100, 0);
    }
}
