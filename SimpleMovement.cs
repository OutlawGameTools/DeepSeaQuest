using UnityEngine;
using System.Collections;

public class SimpleMovement : MonoBehaviour {

	public float xSpeed;
	public float ySpeed;
	public bool	randomizeX = false;
	public bool	randomizeY = false;
	public float minX;
	public float minY;
	public float maxX;
	public float maxY;

	public bool randomXDir = false;
	public bool randomYDir = false;

	private float xMultiplier = 1.0f;
	private float yMultiplier = 1.0f;

	// Use this for initialization
	void Start () 
	{
		if (randomizeY)
			ySpeed = Random.Range(minY, maxY);

		if (randomizeX)
			xSpeed = Random.Range(minX, maxX);

		if (randomXDir)
			if (Random.value < 0.5f)
				xMultiplier = -1;

		if (randomYDir)
		if (Random.value < 0.5f)
			yMultiplier = -1;


		GetComponent<Rigidbody2D>().AddForce(new Vector2(xSpeed*xMultiplier, ySpeed*yMultiplier));
		transform.localScale = new Vector3(xMultiplier, 1, 1);
	}

	void OnBecameInvisible()
	{
		// get rid of the bubble when it goes off the screen
		Destroy(gameObject);
	}
}
