using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour
{
    public MusicNote melodyNote;
    public SoundPlayer sp;

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(gameObject.transform.position);
        if (screenPos.y < -Screen.height)
        {
            Destroy(gameObject);
        }
    }

    private void OnMouseDown()
    {
        sp.playNote(melodyNote.getValue(), MusicInstrument.FLUTE_INSTRUMENT, 200, melodyNote.elapseTime, true);

        UnityEngine.Debug.Log("clicked " + melodyNote.getValue());

        StartCoroutine(destroyAfterComplete());
    }

    IEnumerator destroyAfterComplete()
    {
        yield return new WaitForSeconds(melodyNote.elapseTime / 100);
        Destroy(this.gameObject);
    }
}
