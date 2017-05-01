using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour {

	public static bool BigFishBuild = true; // shows the BFG splash screen and disables ads & stuff

	public static bool isBeta = false;			// unlock all worlds and games
	public static bool isBetaLevels = false;	// show just one game per level
	public static bool isBetaOnlyGood = false;	// show or not dynamite, skulls, etc.

	public static bool showStory = false;

	public static string[] worldNames = {"worldOne", "worldTwo", "worldThree", "worldFour" };

	public static string[] levelNames = 
	{
		"levelOne", "levelTwo", "levelThree", "levelFour",
		"levelFive", "levelSix", "levelSeven", "levelEight", "levelNine"
	};
	
	public static bool showAds = true;

	public static bool[] worldsUnlocked;
	public static bool[] levelsUnlocked;

	public static int currWorldNum = 0;
	public static int currLevelNum = 1;
	public static int numGamesInLevel = 9;

	public static bool playAudio = true;

	public static int matchesFound = 0;
	public static int numTries = 0;

	public static float noMatchPause = 1.0f;
	public static float matchPause = 0.5f;

	public static bool canClick = true;

	public static Dictionary<int2, GameObject> gridPieces;
	public static Dictionary<int2, GameObject> gridCells;

}
