using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteDropping : MonoBehaviour
{
    public float speed;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);

        Vector3 screenPos = cam.WorldToScreenPoint(gameObject.transform.position);
        if (screenPos.y > Screen.height + 200)
        {
            Destroy(gameObject);
        }

    }
}
