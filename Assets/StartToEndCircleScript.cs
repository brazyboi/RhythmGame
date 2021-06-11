using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartToEndCircleScript : MonoBehaviour
{
    public GameObject explosion1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EndCircle")
        {
            Instantiate(explosion1, collision.transform.position, Quaternion.identity);
            Destroy(this);
        }
    }

}
