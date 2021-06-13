using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class InGameScript : GameBase
{
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        base.init();

    }

    // Update is called once per frame
    void Update()
    {
        comboText.text = "x" + manager.combo;
        scoreText.text = "Score: " + manager.score; 
    }
}
