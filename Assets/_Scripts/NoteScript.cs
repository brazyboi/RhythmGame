using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : GameBase
{
    public MusicNote melodyNote;
    public SoundPlayer sp;
    public GameObject xmark;
    GameObject sprite;
    bool wasClicked = false;

    public GameObject greenFX;
    public GameObject yellowFX;
    public GameObject purpleFX;

    Camera cam;

    GameObject bg;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        bg = GameObject.FindGameObjectWithTag("Background");
        sprite = this.transform.GetChild(0).gameObject;
        base.init();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(gameObject.transform.position);

        if (screenPos.y < -Screen.height)
        {
            if (!wasClicked)
            {
                GameObject mark = Instantiate(xmark, new Vector3(transform.position.x, manager.speed * manager.playTime / 100 - 5, -5), Quaternion.identity);
                mark.transform.SetParent(bg.transform);
                manager.combo = 0;
            }

            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        long clickTime = manager.playTime;

        GameObject fx;

        float diff = Mathf.Abs(clickTime - melodyNote.tick) / 100;

        UnityEngine.Debug.Log("diff = " + diff);

        if (diff <= 4)
        {
            manager.score += 300 * manager.combo;

            fx = Instantiate(greenFX, new Vector3(transform.position.x, manager.speed*manager.playTime/100 - 5, -5), Quaternion.identity);

        } else if (diff <= 6)
        {
            manager.score += 100 * manager.combo;

            fx = Instantiate(yellowFX, new Vector3(transform.position.x, manager.speed * manager.playTime / 100 - 5, -5), Quaternion.identity);

        }
        else
        {
            manager.score += 50 * manager.combo;
            fx = Instantiate(purpleFX, new Vector3(transform.position.x, manager.speed * manager.playTime / 100 - 5, -5), Quaternion.identity);

        }

        fx.transform.SetParent(GameObject.FindGameObjectWithTag("Background").transform);

        wasClicked = true;
        sp.playNote(melodyNote.getValue(), MusicInstrument.FLUTE_INSTRUMENT, 200, melodyNote.elapseTime, true);

        //UnityEngine.Debug.Log("clicked " + melodyNote.getValue());

        sprite.GetComponent<SpriteRenderer>().color = Color.red;

        manager.combo++;

    }

    private void OnMouseUp()
    {
        sp.stopNote(melodyNote.getValue(), 100, 500);
        sprite.GetComponent<SpriteRenderer>().color = Color.white;
        this.GetComponent<BoxCollider>().enabled = false;
    }

}
