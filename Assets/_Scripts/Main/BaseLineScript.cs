﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLineScript : GameBaseEx
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, soundPlayer.playTime * gameManager.speed / 100, 0);
    }
}
