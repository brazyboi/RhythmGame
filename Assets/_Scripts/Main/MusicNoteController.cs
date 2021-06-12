using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNoteController : GameBaseEx
{
	private MusicNote note;
	public Transform particle;
	public Transform noteCube;
	public float speed;
	long clickTime;
	bool initialClick = true;
	bool playing = true;

	enum NoteState
    {
		notClicked,
		playing,
		ended
    }

	NoteState noteState;
	public MusicNote Note
	{
		set {
			note = value;
			initNote();
		}
		get { return note; }
	}

	private static float prevXPos;
	// Use this for initialization
	void Start()
	{
		
	}


	void initNote()
    {
		float xPos = UnityEngine.Random.Range(-Screen.width + 400, Screen.width - 400) / 100;

		if (xPos == prevXPos)
		{
			xPos += 1;
		}
		putNotePosition(xPos);
		prevXPos = xPos;
		noteState = NoteState.notClicked;

	}

	void putNotePosition(float xPos)
	{
		float length;
		if (soundPlayer.playTime < note.tick)
        { 
			length = calculateLengthByDuration(note.tickGapNext);

		} else
        {
			length = calculateLengthByDuration(note.tick + note.tickGapNext - soundPlayer.playTime);
        }
		transform.localScale = new Vector3(1.5f, length, 1f);

		transform.position = new Vector3(prevXPos, note.tick * gameManager.speed / 100 + length / 2, 0);

		transform.position = new Vector3(transform.position.x, transform.position.y + gameManager.speed * (soundPlayer.playTime - clickTime) / 100, transform.position.z);
		transform.localScale = new Vector3(1.5f, transform.localScale.y - gameManager.speed * (soundPlayer.playTime - clickTime) / 100, 1f);


	}

	float calculateLengthByDuration(long noteDuration)
    {
		float length = noteDuration;

		length = length * gameManager.speed / 300;

		return length;
	}

	void checkTapDown()
    {
		if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
		{
			if (initialClick)
			{
				clickTime = soundPlayer.playTime;
			}
			/*
			Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
			if (hit != null && hit.collider != null && hit.collider.gameObject == gameObject) {
				//Debug.Log ("I'm hitting "+hit.collider.name);
				hitNote ();
			}*/

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.gameObject == gameObject)
				{
					//Debug.Log("Name = " + hit.collider.name);
					//Debug.Log("Tag = " + hit.collider.tag);
					//Debug.Log("Hit Point = " + hit.point);
					//Debug.Log("Object position = " + hit.collider.gameObject.transform.position);
					//Debug.Log("--------------");
					playNote();
					initialClick = false;

				}
			}


		}
	}

	bool checkTapTooLate()
    {
		float playTime = SoundPlayer.singleton().playTime;
		if (Mathf.Abs(note.tick - playTime) > 1000)
		{
			return true;
		}
		return false;
	}

	void checkTapRelease()
    {
		UnityEngine.Debug.Log("releasedooooooooooooooooooooooooooooooo!s");
		if (Input.GetMouseButtonUp(0)) //|| (Input.touchCount == 0)
		{
			
			soundPlayer.stopAllNote(500, 150);
			noteCube.gameObject.SetActive(false);
			noteState = NoteState.ended;
		}
	}

	void checkTouch()
	{

		if (noteState == NoteState.notClicked)
        {
			
			if (!checkTapTooLate())
            {
				checkTapDown();
			}
			
        } else if (noteState == NoteState.playing)
        {
			checkTapRelease();
			putNotePosition(transform.localPosition.x);
        }

		
	}


	public void playNote()
	{
		noteState = NoteState.playing;
		Debug.Log("NoteState: " + noteState);

		var c = gameObject.GetComponent<BoxCollider>();
		c.enabled = false;
		ParticleSystem p = particle.GetComponent<ParticleSystem>();
		p.Play();
		soundPlayer.playNote(note.value, AppContext.instance().getInstrument(), 255, note.tickGapNext + 5000, true);

	}

	// Update is called once per frame
	void Update()
	{
		if (note == null)
			return;

		float playTime = SoundPlayer.singleton().playTime;

		if (soundPlayer.getPlayMode() == SoundPlayer.LEARN_PLAY)
		{
			if (note.tick <= playTime)
				playNote();
		}
		else
		{
			checkTouch();
		}

		updateHitEffect();
		autoDestroyWhenPass();
	}

	void updateHitEffect()
    {
		if(noteState == NoteState.notClicked)
        {
			return;
        }
		var pos = particle.localPosition;
		float z = pos.z + speed * Time.deltaTime * 1000;
		pos.z = z;
		particle.localPosition = pos;
	}

	void autoDestroyWhenPass()
    {
		if (note.tick + 8000 < soundPlayer.playTime)
		{
			Destroy(gameObject);
		}
	}

}
