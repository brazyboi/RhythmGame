using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNoteController : GameBaseEx
{
	private MusicNote note;
	public Transform particle;
	public Transform noteCube;

	GameObject noteObj;

	public float speed;
	long clickTime;
	bool initialClick = true;
	static bool onlyPrintOnce = false;
	bool printLog;

	public GameObject start;
	public GameObject end;


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
		noteObj = transform.GetChild(3).gameObject;
		createStartEndCircles();

		if (!onlyPrintOnce)
        {
			printLog = true;
			

		} else
        {
			printLog = false;
        }
		onlyPrintOnce = true;
		float xPos = UnityEngine.Random.Range(-5, 5);

		if (xPos == prevXPos)
		{
			xPos += 2;
		}
		updateNotePosition(xPos);
		prevXPos = xPos;
		noteState = NoteState.notClicked;

	}

	void createStartEndCircles()
    {
		long noteDuration = calculateNoteDuration();
		long remainDuration = calculateRemainDuration(noteDuration);

		//setting inital scale for calculations to work
		noteObj.transform.localScale = new Vector3(1.5f, calculateLengthByDuration(remainDuration), 0.1f);
		

	}

	float calculatePosYByTick(long tick)
    {
		return tick * gameManager.speed / 1000;
	}



	long calculateNoteDuration()
    {
		long noteDuration = 0;
		if (note.elapseTime < 500)
		{
			noteDuration = note.elapseTime;

		}
		else
		{
			noteDuration = note.tickGapNext - 100;

		}
		return noteDuration;
	}

	long calculateRemainDuration(long noteDuration)
    {
		long remainDuration = 1000;
		if (noteState == NoteState.notClicked)
		{
			remainDuration = noteDuration;

		}
		else //if (note.tick + noteDuration >= soundPlayer.playTime)
		{
			remainDuration = noteDuration - (soundPlayer.playTime - clickTime);
		}

		if (remainDuration < 10)
		{
			remainDuration = 10;
		}
		return remainDuration;
	}

	void updateNotePosition(float xPos)
	{

		long noteDuration = calculateNoteDuration();
		long remainDuration = calculateRemainDuration(noteDuration);

		float orignalLength = calculateLengthByDuration(noteDuration);
		float remainlength = calculateLengthByDuration(remainDuration);

		if (printLog)
		{
			UnityEngine.Debug.Log("local scale: " + transform.localScale.y + " local pos: " + transform.position.y);
		}
		noteObj.transform.localScale = new Vector3(1.5f, remainlength, 0.1f);

		//revert circles to correct scale
		
		float endyPos = calculatePosYByTick(note.tick) + orignalLength;
		float yPos = endyPos - remainlength / 2;
		if (printLog)
        {
			UnityEngine.Debug.Log("Calculate yPos=" + yPos + " yPos + orignalLength" + (yPos + orignalLength) + " CameraPostion=" + calculatePosYByTick(soundPlayer.playTime));
		}
		
		if (printLog)
		{
			UnityEngine.Debug.Log("Calculate yPos=" + yPos + " yPos - remainlength/2" + (yPos - remainlength/2) + " CameraPostion=" + calculatePosYByTick(soundPlayer.playTime));
			UnityEngine.Debug.Log("Note.tick=" + Note.tick +
				" yPos + remainlength/2 == " + (yPos + remainlength/2) + " posY=" + yPos + " remainlength=" + remainlength + " orignalLength=" + orignalLength + " remainDuration = " + remainDuration + " noteDuration=" + noteDuration +  "note.tickGapNext=" + note.tickGapNext + " note.elapseTime="+ note.elapseTime);
		}

		noteObj.transform.localPosition = new Vector3(xPos, yPos, 0);

		float endHoldYPos = endyPos - 1;// calculateEndPosY(noteDuration);
		float startHoldYPos = endyPos - remainlength + 1;

		moveStartEndCircles(startHoldYPos, endHoldYPos);
		
	}

	void moveStartEndCircles(float startPos, float endPos)
    {
		start.transform.position = new Vector3(noteObj.transform.localPosition.x, startPos, noteObj.transform.localPosition.z);
		end.transform.position = new Vector3(noteObj.transform.localPosition.x, endPos, noteObj.transform.localPosition.z);
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
				if (hit.collider.gameObject == start)
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

		noteObj.GetComponent<BoxCollider>().enabled = false;

		ParticleSystem p = particle.GetComponent<ParticleSystem>();
		Vector3 pos = particle.position;
		pos.y -= noteObj.transform.localScale.y / 2;
		particle.position = pos;
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
		updateNotePosition(noteObj.transform.localPosition.x);
		updateHitEffect();
		updateLinePos();
		autoDestroyWhenPass();
	}

	void updateLinePos()
    {
		LineRenderer lr = GetComponent<LineRenderer>();
		lr.SetPosition(0, start.transform.position);
		lr.SetPosition(1, end.transform.position);
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
