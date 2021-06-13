using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrumNoteController : GameBaseEx
{
	public Transform explosion;
	public long explosionTime
	{
		set {
			placeByTime(value);
		}
	}

	void placeByTime(long time)
    {

		transform.position = new Vector3(0, (time + 2500) * gameManager.speed / 1000, -11);
		Transform o = Instantiate(explosion, new Vector3(0, 0, 0), Quaternion.identity);
		o.parent = transform;
		o.localPosition = new Vector3();
	}

	
}
