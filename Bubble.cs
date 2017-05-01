using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour {

	public float riseSpeedMin = 50f;
	public float riseSpeedMax = 90f;

	// Use this for initialization
	void Start () 
	{
		float riseSpeed = Random.Range(riseSpeedMin, riseSpeedMax);
		GetComponent<Rigidbody2D>().AddForce(new Vector2(0, riseSpeed));
	}

	void OnBecameInvisible()
	{
		// get rid of the bubble when it goes off the screen
		Destroy(gameObject);
	}
}
