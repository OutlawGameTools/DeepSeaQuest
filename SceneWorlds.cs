using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class SceneWorlds : MonoBehaviour {

	public Button[] worldButtons;

	public Button watchAd;
	public Text watchAdText;
	public Button iapButton;
	public Text iapText;

	public GameObject normalPanel;
	public GameObject storyPanel;
	public GameObject mermaid;
	public Text storyText;
	public Text okButtonText;

	bool allWorldsUnlocked = true;


	// Use this for initialization
	void Start () 
	{

		// beta thing
		if (GameData.isBeta)
			UnlockAllWorlds();

		// for Mac/Win don't allow ads or IAP. Just play to unlock worlds/levels.
		#if UNITY_STANDALONE
		if (watchAd != null)
			watchAd.gameObject.SetActive(false);
		if (iapButton != null)
			iapButton.gameObject.SetActive(false);
		//UnlockAllWorlds();
		PlayerPrefs.SetInt("showAds", 0);
		#endif


		// should we show ads or ad buttons?
		// we can show ads unless they paid 99 cents
		int showAds = PlayerPrefs.GetInt("showAds", -1);
		if (showAds == -1)
			PlayerPrefs.SetInt("showAds", 1);
		else
		{
			if (showAds == 0)
			{
				//watchAd.gameObject.SetActive(false);
				#if UNITY_ADS
				watchAdText.text = "Watch a Video Ad to\nShow Your Appreciation";
				#endif
				if (iapButton != null)
					iapButton.gameObject.SetActive(false);
			}
			else
				GameData.showAds = true;
		}

		// see if we've set up world stuff
		GameData.currWorldNum = PlayerPrefs.GetInt("currWorldNum", -1);

		// if it hasn't been set yet, do the default
		if (GameData.currWorldNum == -1)
		{
			PlayerPrefs.SetInt("currWorldNum", 0);
			GameData.currWorldNum = 0;
		}

		WhichWorldsAreUnlocked();

		GameData.worldsUnlocked[0] = true;	// first world is always unlocked

		for (int x = 0; x < GameData.worldsUnlocked.Length; x++)
		{
			if (GameData.worldsUnlocked[x])
			{
				ActivateWorldButton(x);
			}
				
		}
		/*
		// not showing total world score on icons
		if (false)
		{
			// show the score for each level
			string[] worldNames = {"Shallow\nLevels", "Medium\nLevels", "Deep\nLevels", "Bonus\nLevels" };
			for (int x = 0; x < GameData.worldsUnlocked.Length; x++)
			{
				if (GameData.worldsUnlocked[x])
				{
					int score = GetWorldScore(x);
					if (worldButtons.Length > 0 && worldButtons[x] != null)
						worldButtons[x].GetComponentInChildren<Text>().text = worldNames[x] + "\n" + score;
				}
			}
		}
		*/

		#if (UNITY_ADS)
		if (allWorldsUnlocked)
		{
			watchAdText.text = "Watch a Video Ad to\nShow Your Appreciation";
		}

		watchAd.transform.DOShakeRotation(1.5f, 20f).SetDelay(0.5f);
		#endif
	}

	// the Wn Buttons
	void ActivateWorldButton(int worldNum)
	{
		SpriteRenderer[] sRends = worldButtons[worldNum].GetComponentsInChildren<SpriteRenderer>(true);
		foreach(SpriteRenderer sR in sRends)
		{
			string sName = sR.gameObject.name;
			if (sName == "Finished" && AllGamesPlayed(worldNum))
			{
				sR.gameObject.SetActive(true);
			}
			else if (sName == "Icon_lock")
			{
				sR.gameObject.SetActive(false);
				worldButtons[worldNum].interactable = true;
			}
		}
	}


	void WhichWorldsAreUnlocked()
	{
		// see what worlds should be unlocked

		for (int x = 0; x < GameData.worldNames.Length; x++)
		{
			int worldUnlocked = PlayerPrefs.GetInt(GameData.worldNames[x] + "Unlocked", 0);

			if (x == 0)
				worldUnlocked = 1;	// first world is always unlocked

			GameData.worldsUnlocked[x] = (worldUnlocked == 1);

			allWorldsUnlocked = allWorldsUnlocked && GameData.worldsUnlocked[x];
		}

	}
	
	public void SetWorld(int worldNum)
	{
		PlayerPrefs.SetInt("currWorldNum", worldNum);
		GameData.currWorldNum = worldNum;
		SceneManager.LoadScene ("levels");
	}

	void UnlockAllWorlds()
	{
		Debug.Log("In the Unlock all worlds function");

		for (int x = 0; x < GameData.worldsUnlocked.Length; x++)
		{
			if (! GameData.worldsUnlocked[x])
			{
				ActivateWorldButton(x);
				/*
				Button w = GameObject.Find("Canvas/W" + (x+1) + " Button").GetComponent<Button>();
				if (w != null)
				{
					w.gameObject.GetComponentInChildren<SpriteRenderer>(true).gameObject.SetActive(false);
					w.interactable = true;

					//worldButtons[x].GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
					//worldButtons[x].interactable = true;
				//break;
				}
				*/
			}
			PlayerPrefs.SetInt(GameData.worldNames[x] + "Unlocked", 1);
			GameData.worldsUnlocked[x] = true;
		}
		allWorldsUnlocked = true;
	}

	public void UnlockNextWorld()
	{
		Debug.Log("Unlock next world here (SceneWorld)");

		for (int x = 0; x < GameData.worldsUnlocked.Length; x++)
		{
			if (! GameData.worldsUnlocked[x])
			{
				ActivateWorldButton(x);
			/*
				Button w = GameObject.Find("Canvas/W" + (x+1) + " Button").GetComponent<Button>();
				if (w != null)
				{
					w.gameObject.GetComponentInChildren<SpriteRenderer>(true).gameObject.SetActive(false);
					w.interactable = true;

					//worldButtons[x].GetComponentInChildren<SpriteRenderer>().gameObject.SetActive(false);
					//worldButtons[x].interactable = true;
				}
				*/

				PlayerPrefs.SetInt(GameData.worldNames[x] + "Unlocked", 1);
				GameData.worldsUnlocked[x] = true;
				break;
			}

		}
		//SawMermaidStory();
	}

	#if (UNITY_ADS)
	public void ShowRewardedAd()
	{
		Debug.Log("ShowRewardedAd");
		if (Advertisement.IsReady())
		{
			Debug.Log("Advertisement.IsReady");
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show(null, options);
		}
	}
	#endif

	#if (UNITY_ADS)
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			UnlockNextWorld();

			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
	#endif

	int GetWorldScore(int wNum)
	{
		int score = 0;

		for (int x = 0; x < GameData.numGamesInLevel; x++)
		{
			string lvlName = "w" + wNum + GameData.levelNames[x] + "HighScore";
			score += PlayerPrefs.GetInt(lvlName, 0);
		}

		return score;
	}

	// pass in a world/level number and get back a true/false saying whether all games were played.
	bool AllGamesPlayed(int wNum)
	{
		int score = 0;
		bool allPlayed = true;

		for (int x = 0; x < GameData.numGamesInLevel; x++)
		{
			string lvlName = "w" + wNum + GameData.levelNames[x] + "HighScore";
			score = PlayerPrefs.GetInt(lvlName, 0);
			if (score == 0)
				allPlayed = false;
		}

		return allPlayed;
	}


	// do this after the person has purchased ads
	public void TurnOffAds()
	{
		Debug.Log("TurnOffAds");
		PlayerPrefs.SetInt("showAds", 0);
		GameObject.Find("Canvas/Normal Panel/NoAdsIAP").gameObject.SetActive(false);
		//if (iapButton != null)
		//	iapButton.gameObject.SetActive(false);

		// don't unlock all worlds, just allow them to be unlocked by playing, not by watching ads
		//UnlockAllWorlds();
		//allWorldsUnlocked = true;
		UnlockNextWorld();

		watchAdText.text = "Watch a Video Ad to\nShow Your Appreciation";
		//SceneManager.LoadScene("worlds");
	}
}
