﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNoteController : GameBaseEx
{
	private MusicNote note;
	public Transform particle;
	public Transform noteCube;

	public Transform collider;

	public GameObject touchTrack;
	public GameObject xMark;
	public GameObject touchEffect;
	
	public float speed;
	long clickTime;
	bool initialClick = true;
	static bool onlyPrintOnce = true;
	bool printLog;

	public GameObject start;
	public GameObject end;

	long noteScore;
	private KeyCode keyCode;
	private static KeyCode lastKeyCode = KeyCode.RightArrow;
	public ScoreDelegate scoreDelegate;

	private static int playingNoteCount = 0;

	enum NoteState
    {
		notClicked,
		playing,
		ended,
		missed
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
	private float xPos;
	// Use this for initialization
	void Start()
	{
		
	}


	void initNote()
    {
		keyCode = KeyCode.Space;// (lastKeyCode == KeyCode.LeftArrow) ? KeyCode.RightArrow : KeyCode.LeftArrow;
		lastKeyCode = keyCode;
		createStartEndCircles();
		if (!onlyPrintOnce)
        {
			printLog = true;
			

		} else
        {
			printLog = false;
        }
		onlyPrintOnce = true;
		xPos = UnityEngine.Random.Range(-5, 5);

		if (xPos == prevXPos)
		{
			xPos += 2;
		}
		//Debug.Log(xPos);
		updateNotePosition(xPos);
		prevXPos = xPos;
		noteState = NoteState.notClicked;
		hideTouchEffect();

	}

	void createStartEndCircles()
    {
		long noteDuration = calculateNoteDuration();
		long remainDuration = calculateRemainDuration(noteDuration);

		//setting inital scale for calculations to work
		noteCube.localScale = new Vector3(noteCube.localScale.x, calculateLengthByDuration(remainDuration), noteCube.localScale.z);
		

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
		noteDuration = 1000;
		return noteDuration;
	}

	long calculateRemainDuration(long noteDuration)
    {
		long remainDuration = 1000;
		if (noteState == NoteState.notClicked || noteState == NoteState.missed)
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
		noteCube.localScale = new Vector3(noteCube.localScale.x, remainlength, noteCube.localScale.z);

		//revert circles to correct scale
		float yPos = calculatePosYByTick(note.tick);
		transform.localPosition = new Vector3(xPos, yPos, 0);


		float noteEndPosY = orignalLength;
		float notePosY = noteEndPosY - remainlength / 2;
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
		noteCube.localPosition = new Vector3(0, notePosY, noteCube.localPosition.z);
		//Debug.Log(xPos);

		float endHoldYPos = noteEndPosY - 1;// calculateEndPosY(noteDuration);
		float startHoldYPos = noteEndPosY - remainlength + 1;

		moveStartEndCircles(startHoldYPos, endHoldYPos);
		touchEffect.transform.localPosition = new Vector3(touchEffect.transform.localPosition.x, startHoldYPos, touchEffect.transform.localPosition.z);
		
	}

	void moveStartEndCircles(float startPos, float endPos)
    {
		start.transform.localPosition = new Vector3(noteCube.localPosition.x, startPos, noteCube.localPosition.z);
		collider.localPosition = start.transform.localPosition;
		end.transform.localPosition = new Vector3(noteCube.localPosition.x, endPos, noteCube.localPosition.z);
	}

	float calculateLengthByDuration(long noteDuration)
    {
		float length = noteDuration;

		length = length * gameManager.speed / 1000;

		if(!AppContext.instance().isWindInstrument())
        {
			return 3.5f;
        }
		return length;
	}

	void checkTapDown()
    {
		bool bClick = false;
		if(Input.GetKeyDown(keyCode))
        {
			bClick = true;

		} 

		if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ) 
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
				if (hit.collider.gameObject == collider.gameObject)
				{
					//Debug.Log("Name = " + hit.collider.name);
					//Debug.Log("Tag = " + hit.collider.tag);
					//Debug.Log("Hit Point = " + hit.point);
					//Debug.Log("Object position = " + hit.collider.gameObject.transform.position);
					//Debug.Log("--------------");
					bClick = true;
				}
			}


		}
		if (bClick)
		{
			playingNoteCount++;
			playNote();
		}
	}

	bool checkTapTooEarly()
    {
		float playTime = SoundPlayer.singleton().playTime;
		if (note.tick - playTime > 400)
		{
			return true;
		}
		return false;
	}

	void checkTapRelease()
    {
		//UnityEngine.Debug.Log("releasedooooooooooooooooooooooooooooooo!s");
		if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(keyCode)) //|| (Input.touchCount == 0)
		{
			playingNoteCount--;
			disableAppearance();

			float diff = Mathf.Abs(start.transform.position.y - end.transform.position.y);
			Debug.Log("diff = " + diff);
			if (diff <= 0.5f)
			{
				hitParticle(particle);
			}
			scoreDelegate.updateScore(noteScore, true);

			if(Mathf.Abs(soundPlayer.playTime - note.tick) < 150) {
				scoreDelegate.updateSuperScore(100);
            } 

		}
	}


	void disableAppearance()
    {
		hideTouchEffect();
		soundPlayer.stopAllNote(500, 150);
		noteCube.gameObject.SetActive(false);
		start.gameObject.SetActive(false);
		end.gameObject.SetActive(false);
		noteState = NoteState.ended;
	}

	void checkTouch()
	{

		if (noteState == NoteState.notClicked)
		{

			if (!checkTapTooEarly() && playingNoteCount ==0)
			{
				checkTapDown();
			}

		}
		else if (noteState == NoteState.playing)
		{
			checkTapRelease();
		}

	}

	void calculateScore()
    {
		noteScore = (soundPlayer.playTime - clickTime)/10;
    }

	void showTouchEffect()
    {
		touchEffect.SetActive(true);
		touchTrack.SetActive(true);
    }

	void hideTouchEffect()
    {
		touchTrack.SetActive(false);
		touchEffect.SetActive(false);	
    }

	public void playNote()
	{
		if (initialClick)
		{
			clickTime = soundPlayer.playTime;
		}
		initialClick = false;
		noteState = NoteState.playing;
		//Debug.Log("NoteState: " + noteState);

		var c = collider.GetComponent<BoxCollider>();
		c.enabled = false;

		collider.GetComponent<BoxCollider>().enabled = false;

		hitParticle(particle);
		showTouchEffect();
		soundPlayer.playNote(note.value, AppContext.instance().getInstrument(), 255, note.tickGapNext + 5000, true);

	}

	void hitParticle(Transform particleHit)
    {
		ParticleSystem p = particleHit.GetComponent<ParticleSystem>();
		Vector3 pos = particleHit.localPosition;
		pos.y = start.transform.localPosition.y;
		pos.x = start.transform.localPosition.x;
		particleHit.localPosition = pos;
		p.Play();
	}

	// Update is called once per frame
	void Update()
	{
		if (note == null)
			return;

		float playTime = SoundPlayer.singleton().playTime;
		updateNotePosition(transform.localPosition.x);
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
		if (noteState == NoteState.playing)
		{
			calculateScore();
			scoreDelegate.updateScore(noteScore, false);
		} else if(noteState == NoteState.notClicked)
        {
			checkNoteMissPlay();
        }

		updateHitEffect();
		autoDestroyWhenPass();
		//indicateMiss();
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

	void checkNoteMissPlay()
    {

		if(note.tick >= soundPlayer.playTime + 200 )
        {
			return;
        }

		if(isOutOfScreen(start.transform, 0))
        {

			noteState = NoteState.missed;
			scoreDelegate.missPlayNote(0);
			Renderer renderer = noteCube.GetComponent<Renderer>();
			renderer.material.color = Color.red;
		}

	}

}
