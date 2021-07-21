using System.Collections;
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

	Timer cubeNoteFade;
	float fadeTime;

	//long noteScore;
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

	private KeyCode[] playKeys = { KeyCode.Space, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow};

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
	private  long holdingNoteScore;
	private  long lastRemainingDuration;
	// Use this for initialization
	void Start()
	{
		
	}


	void initNote()
    {
		//createStartEndCircles();
		printLog = false;
		onlyPrintOnce = true;
		xPos = UnityEngine.Random.Range(-Screen.width/200 - 1, Screen.width/200 +1);

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
		return ScoreUtils.calculateNoteDuration(note);
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
			Vector3 scale = fluteNoteBar.localScale;
			scale.y = scale.x * 1.1f;
			collider.localScale = scale;
		} else
        {
			collider.localPosition = noteCube.localPosition;
			Vector3 scale = noteCube.localScale;
			scale.x = scale.x * 1.1f;
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

	KeyCode playKey;
	bool isPlayKeyDown()
    {
		foreach(KeyCode key in playKeys)
        {
			if(Input.GetKeyDown(key))
            {
				playKey = key;
				return true;
            }
        }
		return false;
	}

	bool isPlayKeyUp()
	{
		/*
		foreach (KeyCode key in playKeys)
		{
			if (!Input.GetKeyUp(key))
			{
				return false;
			}
		}
		return true;*/
		return Input.GetKeyUp(playKey);
	}

	void checkTapDown()
    {
		bool bClick = false;
		if(!appContext.playingNote && isPlayKeyDown())
        {
			bClick = true;
		}
		if (soundPlayer.getPlayMode() == SoundPlayer.LEARN_PLAY)
		{
			if (note.tick <= soundPlayer.playTime && noteState == NoteState.notClicked)
			{
				bClick = true;
			}
		}
		else if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ) 
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
			appContext.playingNote = true;
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
		bool shouldRelease = false;
		if (soundPlayer.getPlayMode() == SoundPlayer.LEARN_PLAY)
		{
			if (note.tick + calculateNoteDuration() <= soundPlayer.playTime)
			{
				shouldRelease = true;
			}
		} else if (Input.GetMouseButtonUp(0) || isPlayKeyUp()) //|| (Input.touchCount == 0)
		{
			shouldRelease = true;
		}
		if(shouldRelease)
        {
			appContext.playingNote = false;
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
		//noteCube.gameObject.SetActive(false);
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
		
		var scoreResult = ScoreUtils.calculateTapNoteScore(soundPlayer.playTime, note.tick);
		scoreDelegate.updateScore(scoreResult.Item1, (long)scoreResult.Item2, true);
		
	}

	void calculateHoldingScore()
    {
		long noteScore = ScoreUtils.calculateHoldingNoteScore(note, appContext.isWindInstrument(), soundPlayer.playTime, clickTime);
		if(noteScore > 0)
        {
			holdingNoteScore = noteScore;
			scoreDelegate.updateScore("HOLDING", noteScore, false);
		}
    }

	void calculateReleaseTimingScore()
	{
		if (isHoldingNote())
		{
			float score = ScoreUtils.calculateReleaseTimingScore(note, appContext.isWindInstrument(), soundPlayer.playTime);
			
			scoreDelegate.updateScore("GOOD", holdingNoteScore, true);
			if (score > 0)
			{
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
		if (!appContext.isWindInstrument())
        {
			Timer.createTimer(this.gameObject).startTimer(0.5f, new NoteFadeOutTimerCallback(this));
		}
		int volume = appContext.isWindInstrument() ? 100 : 180; // volume 0~255
		soundPlayer.playNote(note.value, appContext.getInstrument(), volume, note.tickGapNext + 500, true);
		if (soundPlayer.getPlayMode() == SoundPlayer.TAP_PLAY)
		{
			soundPlayer.hit(0, false);
		}
		calculateTapScore();


	}

	void hitParticle(Transform particleHit)
    {
		ParticleSystem p = particleHit.GetComponent<ParticleSystem>();
		Vector3 pos = particleHit.localPosition;
		if (appContext.isWindInstrument())
        {
			pos.y = fluteNoteDown.transform.localPosition.y;
			pos.x = fluteNoteDown.transform.localPosition.x;
		} else {
			pos.y = noteCube.transform.localPosition.y;
			pos.x = noteCube.transform.localPosition.x;
		}

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
		
		checkTouch();
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
			if(noteState == NoteState.playing)
            {
				//should reset playingNote in case skip checkTapRelease() 
				appContext.playingNote = false;

			}
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
			controller.setChildrenAlpha(controller.noteCube, alpha);


			//	controller.fluteNoteCircle.GetComponent<RenderTexture>().
		}
		public override void onEnd(Timer timer)
		{
			Destroy(controller.gameObject);
		}
	}

}
