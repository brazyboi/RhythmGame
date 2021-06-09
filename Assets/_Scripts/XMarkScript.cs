using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMarkScript : MonoBehaviour
{
    public float speed = 2.0f;
    public float max = 1f;

    public float delayTime = 0.5f;

    SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        sprite.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time * speed, max));
        StartCoroutine(delayDestroy());
    }

    IEnumerator delayDestroy()
    {

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
