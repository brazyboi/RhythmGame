using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumNoteScript : MonoBehaviour
{

    public GameObject sprite;
	private SpriteRenderer spriteRenderer;
	private Timer timer;
    // Start is called before the first frame update
    void Start()
    {
		spriteRenderer = sprite.GetComponent<SpriteRenderer>();
		timer = Timer.createTimer(this.gameObject);
		timer.startTimer(1, new DrumNoteTimerCallback(this));
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
