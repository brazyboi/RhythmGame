using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumNoteScript : MonoBehaviour
{

    public Sprite sprite1;
	public Sprite sprite2;
	public Sprite sprite3;
	public Sprite sprite4;

	Sprite sprite;

	public GameObject spriteObj;

	private SpriteRenderer spriteRenderer;
	private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
		chooseRandomSprite();
		spriteRenderer = spriteObj.GetComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		timer = Timer.createTimer(this.gameObject);
		timer.startTimer(1, new DrumNoteTimerCallback(this));
    }

	void chooseRandomSprite()
    {
		int rand = (int) (Random.Range(1, 4));
		if (rand == 1)
        {
			sprite = sprite1;
        } else if (rand == 2)
        {
			sprite = sprite2;
        } else if (rand == 3)
        {
			sprite = sprite3;
		} else
        {
			sprite = sprite4;
        }
    }

	public void restartDrumNote()
    {
		if (timer != null)
		{
			timer.startTimer(1, new DrumNoteTimerCallback(this));
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	class DrumNoteTimerCallback : TimerCallback
	{
		DrumNoteScript controller;
		public DrumNoteTimerCallback(DrumNoteScript controller)
		{
			this.controller = controller;
		}
		public override void onTick(Timer timer, float tick, float timeOut)
		{
			Color color = controller.spriteRenderer.color;
			color.a = 1.0f * (timeOut - tick) / timeOut;
			controller.spriteRenderer.color = color;
			Vector3 scale = controller.transform.localScale;
			scale.x = scale.y = 1f * 15 * tick / timeOut;
			controller.transform.localScale = scale;
			//Debug.Log("reached onTick, tick = " + tick + " timeOut = " + timeOut);
		}
		public override void onEnd(Timer timer)
		{
			//Destroy(controller.gameObject);
			//Debug.Log("reached onEnd");
			Color color = controller.spriteRenderer.color;
			color.a = 0;
			controller.spriteRenderer.color = color;
		}
	}
}
