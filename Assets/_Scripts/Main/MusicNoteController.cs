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
	static bool onlyPrintOnce = false;
	bool printLog;
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
		if(!onlyPrintOnce)
        {
			printLog = true;
			

		} else
        {
			printLog = false;
        }
		onlyPrintOnce = true;
		float xPos = UnityEngine.Random.Range(-Screen.width, Screen.width) / 70;

		if (xPos == prevXPos)
		{
			xPos += 2;
		}
		updateNotePosition(xPos);
		prevXPos = xPos;
		noteState = NoteState.notClicked;

	}

	float calculatePosYByTick(long tick)
    {
		return tick * gameManager.speed / 1000;
	}

	void updateNotePosition(float xPos)
	{
		//Calculate note duration and current remainDuration;
		long noteDuration = 0;
		if(note.elapseTime < 500)
        {
			noteDuration = note.elapseTime;

		} else
        {
			noteDuration = note.tickGapNext - 100;

		}
		long remainDuration = 1000;
		if (noteState == NoteState.notClicked)
		{
			remainDuration = noteDuration;

		} else //if (note.tick + noteDuration >= soundPlayer.playTime)
		{
			remainDuration = noteDuration - (soundPlayer.playTime - note.tick);
		}  

		if(remainDuration < 10)
        {
			remainDuration = 10;
        }
		//End Calculation --- DON'T MODIFY ABOVE CODE

		float orignalLength = calculateLengthByDuration(noteDuration);
		float remainlength = calculateLengthByDuration(remainDuration);

		if (printLog)
		{
			UnityEngine.Debug.Log("local scale: " + transform.localScale.y + " local pos: " + transform.position.y);
		}
		transform.localScale = new Vector3(1.5f, remainlength, 1f);

		float yPos = calculatePosYByTick(note.tick);
		if (printLog)
        {
			UnityEngine.Debug.Log("Calculate yPos=" + yPos + " yPos + orignalLength" + (yPos + orignalLength) + " CameraPostion=" + calculatePosYByTick(soundPlayer.playTime));

		}
		yPos = yPos + orignalLength - remainlength / 2;
		if (printLog)
		{
			UnityEngine.Debug.Log("Calculate yPos=" + yPos + " yPos - remainlength/2" + (yPos - remainlength/2) + " CameraPostion=" + calculatePosYByTick(soundPlayer.playTime));
			UnityEngine.Debug.Log("Note.tick=" + Note.tick +
				" yPos + remainlength/2 == " + (yPos + remainlength/2) + " posY=" + yPos + " remainlength=" + remainlength + " orignalLength=" + orignalLength + " remainDuration = " + remainDuration + " noteDuration=" + noteDuration +  "note.tickGapNext=" + note.tickGapNext + " note.elapseTime="+ note.elapseTime);
		}

		transform.localPosition = new Vector3(xPos, yPos, 0);

	}

	float calculateLengthByDuration(long noteDuration)
    {
		float length = noteDuration;

		length = length * gameManager.speed / 1000;

		return length;
	}

	void checkTapDown()
    {
		if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
		{
			
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
				}
			}


		}
	}

	bool checkTapTooLate()
    {
		float playTime = SoundPlayer.singleton().playTime;
		if (Mathf.Abs(note.tick - playTime) > 500)
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

		}
		else if (noteState == NoteState.playing)
		{
			checkTapRelease();
		}

	}


	public void playNote()
	{
		if (initialClick)
		{
			clickTime = soundPlayer.playTime;
		}
		initialClick = false;
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
			if (note.tick <= playTime && noteState == NoteState.notClicked)
			{
				playNote();
			}
		}
		else
		{
			checkTouch();
		}
		updateNotePosition(transform.localPosition.x);
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
		if (Mathf.Abs(note.tick - soundPlayer.playTime) > 8000)
		{
			Destroy(gameObject);
		}
	}

}
