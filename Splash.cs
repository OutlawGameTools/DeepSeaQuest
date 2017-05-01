using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour {

	public Image bgImage;
	float delay = 0;

	IEnumerator Start()
	{
		yield return new WaitForSeconds(delay);
		SceneManager.LoadScene("menu");
	}

	void Awake()
	{
		if (GameData.BigFishBuild)
		{
			delay = 3f;
			bgImage.gameObject.SetActive(true);
		}
	}
}
