﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNoteController : GameBaseEx
{
	private MusicNote note;
	public Transform particle;
	public Transform noteCube;
	public Transform collider;

	public Transform fluteNoteUp;
	public Transform fluteNoteDown;
	public Transform fluteNoteBar;
	public Transform fluteNoteCircle;

	public Transform fluteNoteBarUpInside;
	public Transform fluteNoteBarDownInside;
	public Transform fluteNoteBarCenter;

	public GameObject xMark;
	public GameObject touchEffect;
	
	public float speed;
	long clickTime;
	bool initialClick = true;
	static bool onlyPrintOnce = true;
	bool printLog;

	long noteScore;
	private KeyCode keyCode;
	public ScoreDelegate scoreDelegate;
	private AppContext appContext = AppContext.instance();

	
	private float scaleX;
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
	private long holdingNoteScore;
	private long lastRemainingDuration;
	// Use this for initialization
	void Start()
	{
		
	}


	void initNote()
    {
		keyCode = KeyCode.Space;// (lastKeyCode == KeyCode.LeftArrow) ? KeyCode.RightArrow : KeyCode.LeftArrow;
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

		fluteNoteUp.gameObject.SetActive(isHoldingNote());
		fluteNoteDown.gameObject.SetActive(isHoldingNote());
		fluteNoteBar.gameObject.SetActive(isHoldingNote());
		scaleX = appContext.isWindInstrument() ? fluteNoteBar.localScale.x : noteCube.localScale.x;
		if (isHoldingNote())
		{
			fluteNoteCircle.gameObject.SetActive(false);
			noteCube.gameObject.SetActive(false);
			

		} else
        {
			fluteNoteCircle.gameObject.SetActive(appContext.isWindInstrument());
			noteCube.gameObject.SetActive(!appContext.isWindInstrument());
			
		}


	}

	void createStartEndCircles()
    {
	//	long noteDuration = calculateNoteDuration();
	//	long remainDuration = calculateRemainDuration(noteDuration);

		//setting inital scale for calculations to work
	//	noteCube.localScale = new Vector3(noteCube.localScale.x, calculateLengthByDuration(remainDuration), noteCube.localScale.z);
		

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
		long remainDuration;
		if (noteState == NoteState.notClicked || noteState == NoteState.missed)
		{
			remainDuration = noteDuration;
			lastRemainingDuration = remainDuration;

		}
		else if(noteState == NoteState.playing)
		{
			remainDuration = noteDuration - (soundPlayer.playTime - clickTime);
			lastRemainingDuration = remainDuration;
		}
		else //NoteState.ended
		{
			remainDuration = lastRemainingDuration;

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
		fluteNoteBar.localScale = new Vector3(fluteNoteBar.localScale.x, remainlength - 1, fluteNoteBar.localScale.z);
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
		fluteNoteBar.localPosition = new Vector3(0, notePosY, fluteNoteBar.localPosition.z);
		//Debug.Log(xPos);

		float endHoldYPos = noteEndPosY - 0.5f;// calculateEndPosY(noteDuration);
		float startHoldYPos = noteEndPosY - remainlength + 0.5f;

		moveStartEndCircles(startHoldYPos, endHoldYPos);
		touchEffect.transform.localPosition = new Vector3(touchEffect.transform.localPosition.x, startHoldYPos, touchEffect.transform.localPosition.z);
		updateCollider();
	}

	void moveStartEndCircles(float startPos, float endPos)
    {
		fluteNoteCircle.localPosition = new Vector3(noteCube.localPosition.x, startPos, noteCube.localPosition.z);
		fluteNoteDown.localPosition = new Vector3(noteCube.localPosition.x, startPos, noteCube.localPosition.z);
		fluteNoteUp.localPosition = new Vector3(noteCube.localPosition.x, endPos, noteCube.localPosition.z);

	}

	void updateCollider()
    {
		if (appContext.isWindInstrument())
		{
			collider.localPosition = fluteNoteDown.transform.localPosition;
			Vector3 scale = noteCube.localScale;
			scale.y = 2.5f;
			collider.localScale = scale;
		} else
        {
			collider.localPosition = noteCube.localPosition;
			Vector3 scale = noteCube.localScale;
			collider.localScale = noteCube.localScale;
		}


	}

	float calculateLengthByDuration(long noteDuration)
    {
		float length = noteDuration;

		length = length * gameManager.speed / 1000;

		if(!appContext.isWindInstrument())
        {
			return 3.5f;
        }
		return length;
	}

	void checkTapDown()
    {
		bool bClick = false;
		if(Input.GetKeyDown(keyCode) && appContext.playingNoteCount==0)
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
			appContext.playingNoteCount++;
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
			appContext.playingNoteCount--;
			disableAppearance();

			float diff = Mathf.Abs(fluteNoteDown.transform.position.y - fluteNoteUp.transform.position.y);
			Debug.Log("diff = " + diff);
			if (diff <= 0.5f)
			{
				hitParticle(particle);
			}
			calculateReleaseTimingScore();
			Timer.createTimer(this.gameObject).startTimer(0.5f, new NoteFadeOutTimerCallback(this));
		}
	}


	void disableAppearance()
    {
		hideTouchEffect();
		soundPlayer.stopAllNote(500, 150);
		noteCube.gameObject.SetActive(false);
		noteState = NoteState.ended;
		
	}

	void checkTouch()
	{

		if (noteState == NoteState.notClicked)
		{

			if (!checkTapTooEarly())
			{
				checkTapDown();
			}

		}
		else if (noteState == NoteState.playing)
		{
			checkTapRelease();
		}

	}

	bool isHoldingNote()
    {
		if (appContext.isWindInstrument())
        {
			if(calculateNoteDuration() > 300)
            {
				return true;
            } else
            {
				return false;
            }
			

        } else
        {
			return false;
        }

	}

	void calculateTapScore()
	{
		if (isHoldingNote())
		{
			//do nothing
			holdingNoteScore = 0;
		}
		else
		{
			float tapGapTime = Mathf.Abs((float)note.tick - soundPlayer.playTime);
			string scoreText = "";
			float score = 0;
			if (tapGapTime < 100)
			{
				scoreText = "GREAT";
				score = 20 - tapGapTime * 5 / 100;
			}
			else if (tapGapTime < 300)
			{
				scoreText = "GOOD";
				score = 15 - (tapGapTime - 100) * 5 / 200;
			}
			else if (tapGapTime < 500)
			{
				scoreText = "JUST";
				score = 10 - (tapGapTime - 300) * 5 / 200;
			}
			else
			{
				scoreText = "JUST";
				score = 5;
			}
			scoreDelegate.updateScore(scoreText,(long) score, true);
		}
	}

	void calculateHoldingScore()
    {
		if (isHoldingNote())
		{
			long holdingTime = soundPlayer.playTime - clickTime;
			if (soundPlayer.playTime < note.tick + calculateNoteDuration())
			{//only update if before reach to end.
				noteScore = holdingTime / 10;
				holdingNoteScore = noteScore;
				scoreDelegate.updateScore("HOLDING", noteScore, false);
			}
		} else
        {
			//do nothing
        }
    }

	void calculateReleaseTimingScore()
	{
		if (isHoldingNote())
		{
			float tapGapTime = Mathf.Abs((float)note.tick + calculateNoteDuration() - soundPlayer.playTime);
			string scoreText = "";
			float score = 0;
			scoreDelegate.updateScore("GOOD", holdingNoteScore, true);
			if (tapGapTime < 100)
			{
				scoreText = "SUPER";
				score = 50;
				scoreDelegate.updateSuperScore("SUPER!!!", (long) score);
			}
		}
	}

	void showTouchEffect()
    {
		Color c = Color.green;
		fluteNoteBarDownInside.GetComponent<SpriteRenderer>().color = c;
		fluteNoteBarUpInside.GetComponent<SpriteRenderer>().color = c;
		fluteNoteBarCenter.GetComponent<Renderer>().material.color = c;
		touchEffect.SetActive(true);
    }

	void hideTouchEffect()
    {
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
		soundPlayer.playNote(note.value, appContext.getInstrument(), 255, note.tickGapNext + 5000, true);
		calculateTapScore();

	}

	void hitParticle(Transform particleHit)
    {
		ParticleSystem p = particleHit.GetComponent<ParticleSystem>();
		Vector3 pos = particleHit.localPosition;
		pos.y = fluteNoteDown.transform.localPosition.y;
		pos.x = fluteNoteDown.transform.localPosition.x;
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
			calculateHoldingScore();
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
		if (soundPlayer.playTime > note.tick + calculateNoteDuration() + 1000)
		{
			Destroy(gameObject);
		}
	}

	void checkNoteMissPlay()
    {

		if(note.tick >= soundPlayer.playTime + 300 )
        {
			return;
        }

		if(isOutOfScreen(fluteNoteDown.transform, 0))
        {

			noteState = NoteState.missed;
			scoreDelegate.missPlayNote(0);
			Renderer renderer = noteCube.GetComponent<Renderer>();
			renderer.material.color = Color.red;
		}

	}


	class NoteFadeOutTimerCallback : TimerCallback
	{
		MusicNoteController controller;
		public NoteFadeOutTimerCallback(MusicNoteController controller)
		{
			this.controller = controller;
		}
		public override void onTick(Timer timer, float tick, float timeOut)
		{
			float alpha = (timeOut - tick) / timeOut;
			controller.setChildrenAlpha(controller.fluteNoteCircle, alpha);
			controller.setChildrenAlpha(controller.fluteNoteBar, alpha);
			controller.setChildrenAlpha(controller.fluteNoteDown, alpha);
			controller.setChildrenAlpha(controller.fluteNoteUp, alpha);
			//	controller.fluteNoteCircle.GetComponent<RenderTexture>().
		}
		public override void onEnd(Timer timer)
		{
			Destroy(controller.gameObject);
		}
	}

}
