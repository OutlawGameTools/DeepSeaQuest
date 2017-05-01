using UnityEngine;
using System.Collections;

public class SwipeTrail : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) || Input.GetMouseButton(0)))
		{
			Vector3 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

			this.transform.position = new Vector3(touchPos.x, touchPos.y, 0);
		}

	}
}
