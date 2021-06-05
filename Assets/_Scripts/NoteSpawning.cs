using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawning : MonoBehaviour
{

    public GameObject noteObj;
    public float waitTime = 1;
    float xPos;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(noteSpawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnNote()
    {



    }
    
    IEnumerator noteSpawn()
    {
        while (true)
        {
            xPos = UnityEngine.Random.Range(-Screen.width + 200, Screen.width - 200) / 100;
            Instantiate(noteObj, new Vector3(xPos, (float)(4.75), 0), Quaternion.identity);
            yield return new WaitForSeconds(waitTime);

        }
    }

}
