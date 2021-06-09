using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatingBarsScript : MonoBehaviour
{
    public GameObject bar;
    public GameObject separator;
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject go = Instantiate(separator, new Vector3(-3.5f + 1.0f * i, 0, -5), Quaternion.identity);
            go.transform.SetParent(this.GetComponent<Transform>());
        }
    }

}
