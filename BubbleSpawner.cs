using UnityEngine;
using System.Collections;

public class BubbleSpawner : MonoBehaviour {

	public GameObject bubblePrefab;
 
	public float bubbleSpawnRate = 2f;
	public float xMin = -4f;
	public float xMax = 4f;
	public float yMin = -3f;
	public float yMax = -3f;
	public float bubbleSizer = 1;

	public float riseSpeedMin = 50f;
	public float riseSpeedMax = 90f;

	// Use this for initialization
	void Start () 
	{
		InvokeRepeating("MakeABubble", 2f, bubbleSpawnRate);
	}
	
	void MakeABubble()
	{
		Vector3 bubblePos = new Vector3(Random.Range(xMin, xMax), yMax, 0f);
		GameObject bubble = Instantiate(bubblePrefab, bubblePos, Quaternion.identity) as GameObject;

		bubble.GetComponent<Bubble>().riseSpeedMin = riseSpeedMin;
		bubble.GetComponent<Bubble>().riseSpeedMax = riseSpeedMax;

		if (bubbleSizer != 1)
		{
			float x = bubble.transform.localScale.x * bubbleSizer;
			float y = bubble.transform.localScale.y * bubbleSizer;
			bubble.transform.localScale = new Vector3(x, y, 1);
		}
	}
}
