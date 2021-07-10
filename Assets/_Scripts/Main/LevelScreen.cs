using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScreen : MonoBehaviour
{

    public GameObject level1;
    public GameObject level2;
    public GameObject level3;
    public GameObject level4;
    public GameObject level5;
    public GameObject level6;
    public GameObject level7;
    public GameObject level8;
    public GameObject level9;

    public GameObject lockIcon;

    public List<GameObject> levelButtons;
    

    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    void init()
    {

        levelButtons.Add(level1);
        levelButtons.Add(level2);
        levelButtons.Add(level3);
        levelButtons.Add(level4);
        levelButtons.Add(level5);
        levelButtons.Add(level6);
        levelButtons.Add(level7);
        levelButtons.Add(level8);
        levelButtons.Add(level9);

        for (int i = 0; i < levelButtons.Count; i++)
        { 
            /*if (i >= PlayerData.getPlayerLevel() / 10)
            {
                GameObject locked = Instantiate(lockIcon, new Vector3(0,0,0), Quaternion.identity);
                locked.transform.parent = levelButtons[i].transform.parent;
                locked.transform.localPosition = new Vector3(0, 0, 0);
                levelButtons[i].transform.parent.GetComponentInChildren<Text>().text = "";
                
                continue;
            }*/

            int index = i;
            levelButtons[i].GetComponent<Button>().onClick.AddListener(() => directToLevelSongList(index));
        }

    }

    void directToLevelSongList(int index)
    {
        AppContext.instance().curSongListLevel = index;
        GameManager.gotoSongList();
    }

    // Update is called once per frame
    void Update()
    {
        SoundPlayer.singleton().timerUpdate(Time.deltaTime);
    }
}
