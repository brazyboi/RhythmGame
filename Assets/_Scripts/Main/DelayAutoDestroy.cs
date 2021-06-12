using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAutoDestroy : MonoBehaviour {
	public float lifeDuration = 2.0f;
	// Use this for initialization
	void Start () {
		StartCoroutine (waitAndDestory());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator waitAndDestory() {
		yield return new WaitForSeconds (lifeDuration);
		Destroy (gameObject);
	}
}
