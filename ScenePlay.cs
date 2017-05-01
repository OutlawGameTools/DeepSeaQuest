using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScenePlay : MonoBehaviour {

	public Button audioOnSwitch;
	public Button audioOffSwitch;

	CreateGrid createGrid;

	// Use this for initialization
	void Start () 
	{
		createGrid = GameObject.Find("SceneCode").GetComponent<CreateGrid>();

		//if (PlayerPrefs.GetInt("playAudio") == 0)
		//	GameObject.Find("SceneCode").GetComponent<AudioSource>().Stop();
	}

	public void PauseGame()
	{
		if (GameData.playAudio)
		{
			audioOnSwitch.gameObject.SetActive(true);
			audioOffSwitch.gameObject.SetActive(false);
		}
		else
		{
			audioOnSwitch.gameObject.SetActive(false);
			audioOffSwitch.gameObject.SetActive(true);
		}

		Time.timeScale = 0;

	}
	public void ResumeGame()
	{
		Time.timeScale = 1;
	}

	public void GoingBack()
	{
		ResumeGame();
		//GameData.showStory = false;
	}

	// using the currWorldNum and currLevelNum, unlock the next locked level
	public void UnlockNextLevel()
	{
		Debug.Log("Unlock next level here -- currently: " + GameData.currLevelNum);

		if (GameData.currLevelNum < GameData.numGamesInLevel-1) 	// only do this if we're not at the last level
		{
			//GameData.currLevelNum++;
			string lvlName = "w" + GameData.currWorldNum.ToString() + GameData.levelNames[GameData.currLevelNum+1] + "Unlocked";
			Debug.Log("Unlock " + lvlName);
			PlayerPrefs.SetInt(lvlName, 1);
			PlayerPrefs.Save();
			GameData.levelsUnlocked[GameData.currLevelNum+1] = true;
		}

	}


	public void PlayNextLevel()
	{
		if (GameData.currLevelNum < GameData.numGamesInLevel-1)
		{
			GameData.currLevelNum++;
			createGrid.SetThisLevel();
		}
		else
		{
			if (GameData.showStory)
				SceneManager.LoadScene ("story");
			else
				SceneManager.LoadScene ("worlds");
		}
		
	}
}
