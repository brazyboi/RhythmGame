using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNoteController : GameBaseEx
{
	private MusicNote note;
	public Transform particle;
	public Transform noteCube;
	public float speed;
	public bool autoHit;
	public MusicNote Note
	{
		set {
			note = value;
			initNote();
		}
		get { return note; }
	}

	private bool explosed;
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

		float length = note.elapseTime / 100 - 1.0f;

		if (length < 0)
		{
			length = 1.0f;
		}

		transform.position = new Vector3(xPos, note.tick * gameManager.speed / 100, 0);
		prevXPos = xPos;
	}


	void checkTouch()
	{
		float playTime = SoundPlayer.singleton().playTime;
		if (Mathf.Abs(note.tick - playTime) > 300)
		{
			return;
		}
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
					Debug.Log("Name = " + hit.collider.name);
					Debug.Log("Tag = " + hit.collider.tag);
					Debug.Log("Hit Point = " + hit.point);
					Debug.Log("Object position = " + hit.collider.gameObject.transform.position);
					Debug.Log("--------------");
					hitNote();
				}
			}

		}
		else if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
		{

		}
	}

	public void hitNote()
	{
		if (explosed)
		{
			return;
		}
		var c = gameObject.GetComponent<BoxCollider>();
		c.enabled = false;
		ParticleSystem p = particle.GetComponent<ParticleSystem>();
		p.Play();
		explosed = true;
		noteCube.gameObject.SetActive(false);
		soundPlayer.playNote(note.value, AppContext.instance().getInstrument(), 255, note.tickGapNext, true);
	}

	// Update is called once per frame
	void Update()
	{
		if (note == null)
			return;

		float playTime = SoundPlayer.singleton().playTime;
		if (!explosed)
		{
			if (autoHit)
			{
				if (note.tick <= playTime)
					hitNote();
			}
			else
			{
				checkTouch();
			}
		}


		if (explosed)
		{
			var pos = particle.localPosition;
			float z = pos.z + speed * Time.deltaTime * 1000;
			pos.z = z;
			particle.localPosition = pos;
		}
		else if (note.tick - playTime < 200)
		{
			var pos = transform.localPosition;
			pos.z = pos.z + speed * 0.2f * Time.deltaTime * 1000;
			transform.localPosition = pos;
		}
		if (note.tick + 1000 < playTime)
		{
			Destroy(gameObject);
		}
	}

}
