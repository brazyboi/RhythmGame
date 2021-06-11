using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovingScript : GameBase
{
    // Start is called before the first frame update

    void Start()
    {
        base.init();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, manager.speed * manager.playTime/100, 0);
    }
}
