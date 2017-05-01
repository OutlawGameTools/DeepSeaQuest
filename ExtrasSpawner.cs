using UnityEngine;
using System.Collections;

public class ExtrasSpawner : MonoBehaviour {

	public GameObject[] extrasPrefabs;

	public float extrasRate = 12f;

	// Use this for initialization
	void Start () 
	{
		InvokeRepeating("MakeAnExtra", 12f, extrasRate);
	}

	void MakeAnExtra(string theName="any")
	{
		int idx=0;
		if (theName == "any")
			idx = Random.Range(0, extrasPrefabs.Length);
		else
		{
			for (int x = 0; x < extrasPrefabs.Length; x++)
				if (extrasPrefabs[x].name == theName)
					idx = x;
		}

		GameObject theExtra = extrasPrefabs[idx];

		Vector3 thePos = new Vector3(-6f * theExtra.transform.localScale.x, Random.Range(-3f, 3f), 0f);


		Instantiate(theExtra, thePos, Quaternion.identity);
	}

}
