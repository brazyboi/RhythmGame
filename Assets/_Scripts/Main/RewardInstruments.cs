using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardInstruments : MonoBehaviour
{
    public GameObject instrumentSelectBox;

    Texture2D[] itemImages;
    Sprite[] itemSprites;

    int[] xPoses;
    int[] yPoses;

    int index;

    // Start is called before the first frame update
    void Start()
    {
        initReward();
    }

    void initReward()
    {
        itemImages = Resources.LoadAll("InstrumentImages") as Texture2D[];
        Debug.Log(itemImages.Length);
        itemSprites = new Sprite[itemImages.Length];
        int counter = 0;
        foreach (Texture2D texture in itemImages)
        {
            itemSprites[counter] = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            counter++;
        }
        determineXPoses();
        determineYPoses();
    }

    void determineXPoses()
    {
        int numRow = Screen.width / 100 - 1;
        xPoses = new int[numRow];
        for (int i = 0; i < xPoses.Length; i++)
        {
            xPoses[i] = 50 + i * 100;
        }
    }

    void determineYPoses()
    { 
        
    }

    void createInstruments()
    {


        //int[] yPoses = {}
        
        for (int i = 0; i < itemImages.Length; i++)
        {
            instrumentSelectBox.GetComponent<Image>().sprite = itemSprites[i];
            Instantiate(instrumentSelectBox, new Vector3(xPoses[i % xPoses.Length], 50f, 0), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onSelectInstrument()
    {

    }
}
