using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneMenu : MonoBehaviour {

	public GameObject finishMostButton;
	public GameObject quitButton;
	public GameObject playButton;
	public Text msg;

	public string gameID = "nnnnnn";

	void Start () {

		//GameData.showStory = true;

		if (GameData.isBeta)
		{
			// see if the trial/beta version has timed out
			/*
			long epochTicks = new System.DateTime(2016, 07, 21).Ticks;
			long seconds = ((System.DateTime.UtcNow.Ticks - epochTicks) / System.TimeSpan.TicksPerSecond);
			if (seconds > 0)
			{
				playButton.gameObject.SetActive(false);
				msg.text = "This beta version has expired.";
			}
			Debug.Log(epochTicks);
			Debug.Log(seconds);
			*/
		}
			
		GameData.worldsUnlocked = new bool[4];
		GameData.levelsUnlocked = new bool[GameData.numGamesInLevel];

		int pAudio = PlayerPrefs.GetInt("playAudio", 1);
		if (pAudio == 0)
		{
			GameData.playAudio = false;
			SoundManager.MuteMusic(true);
			SoundManager.MuteSFX(true);
		}
		else
		{
			GameData.playAudio = true;
		}

//		#if UNITY_IOS
//		if (Advertisement.isSupported) // If the platform is supported,
//			Advertisement.Initialize(gameID); // initialize Unity Ads.
//		else
//			Debug.Log("Advertisement not initialized/supported.");
//		#endif

		// turn it off for mobile builds
		quitButton.SetActive(false);
		#if UNITY_STANDALONE
			quitButton.SetActive(true);
		#endif

		GameData.showStory = true;

		// only show the button if we're running in the editor or if god mode is turned on.
		if (! Application.isEditor)
			finishMostButton.gameObject.SetActive(false);

	}

	void Update()
	{
		//Debug.Log("a");
		bool altKey = Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt);

		if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.F4) && altKey))
			QuitGame();
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void FinishMost()
	{
		for (int level = 0; level < 4; level++)
			for (int game = 0; game < 8; game++)
			{
				string lvlName = "w" + level.ToString() + GameData.levelNames[game];

				PlayerPrefs.SetInt(lvlName + "Score", 1);
				PlayerPrefs.SetInt(lvlName + "HighScore", 1);
				PlayerPrefs.SetInt(lvlName + "Unlocked", 1);
				PlayerPrefs.SetInt(GameData.worldNames[level] + "Unlocked", 1);
			}
		
	}

}
