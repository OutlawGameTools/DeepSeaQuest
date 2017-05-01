using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneOptions : MonoBehaviour {

	public Button audioOnSwitch;
	public Button audioOffSwitch;
	public Text audioText;

	public GameObject restoreButton;
	public Text restoreText;

	public GameObject resetButton;
	public Text resetText;

	void Start () {

		#if UNITY_STANDALONE
		restoreButton.SetActive(false);
		restoreText.gameObject.SetActive(false);

		//audioText.gameObject.transform.position.y = 97;

		#endif

		//print("playAudio" + PlayerPrefs.GetInt("playAudio"));

		int pAudio = PlayerPrefs.GetInt("playAudio", -1);

		// if it hasn't been set yet, do the default
		if (pAudio == -1)
			PlayerPrefs.SetInt("playAudio", 1);
		
		if (pAudio == 0)
		{
			audioOnSwitch.gameObject.SetActive(false);
			audioOffSwitch.gameObject.SetActive(true);
		}
	}

	// reset all of the scores as well as the locked levels.
	public void ResetGame()
	{
		for (int w = 0; w < 4; w++)
		{
			//#if UNITY_IOS
			// reset the worlds
			PlayerPrefs.SetInt(GameData.worldNames[w] + "Unlocked", 0);
			GameData.worldsUnlocked[w] = false;
			//#endif

			print("GameData.numGamesInLevel " + GameData.numGamesInLevel);
			// reset the levels in each world
			for (int x = 0; x < GameData.numGamesInLevel; x++)
			{
				string lvlName = "w" + w.ToString() + GameData.levelNames[x];
				print(lvlName);
				int levelUnlocked = 0;

				if (x == 0)
					levelUnlocked = 1;	// first world is always unlocked

				GameData.levelsUnlocked[x] = (levelUnlocked == 1);
				PlayerPrefs.SetInt(lvlName + "Unlocked", levelUnlocked);

				// now delete the scores
				PlayerPrefs.SetInt(lvlName + "Score", 0);
				PlayerPrefs.SetInt(lvlName + "HighScore", 0);

			}

		}
		//#if UNITY_IOS
		// turn the first world back on
		PlayerPrefs.SetInt(GameData.worldNames[0] + "Unlocked", 1);
		GameData.worldsUnlocked[0] = true;
		//#endif

		PlayerPrefs.SetInt("mermaidStory", 0);

	}


}
