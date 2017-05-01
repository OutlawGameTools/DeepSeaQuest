using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Com.LuisPedroFonseca.ProCamera2D;

public class Play : MonoBehaviour {

	public bool firstCardTurned = false;
	public bool secondCardTurned = false;

	public GameObject[] coversTurned = new GameObject[2];

	public bool skullFound = false;
	public bool dynamiteFound = false;

	public bool atLeastOneGood = false;
	public bool matched = false;
	public bool twoHalves = false;
	public string newWholeName;
	public GameObject newWhole;

	public Dictionary<string, GameObject> wholeObjs = new Dictionary<string, GameObject>();

	public Button pauseBtn;
	public GameObject gameOverPanel;
	public GameObject levelOverPanel;
	public Text winScoreText;

	private bool newHighScore = false;
	private Scoring scoring;

	public Sprite coverSprite;

	public AudioClip clickSound;
	public AudioClip matchSound;
	public AudioClip noMatchSound;

	public GameObject matchedCovers;
	public GameObject explodeCover;
	public GameObject mergeEffect;

	public GameObject bonusDiver;
	public GameObject bonusAngler;
	public GameObject bonusPuffer;
	public GameObject bonusUrchin;
	public GameObject bonusSubmarine;
	public GameObject bonusMermaid;
	public GameObject bonusTreasureChest;
	public GameObject winParticles;

	public GameObject tutorial;

	DriftingText dt;
	CreateGrid cg;
	ScenePlay scenePlay;

	bool showAds = true;

	private float randY;

	void Start () 
	{
		scoring = GameObject.Find("SceneCode").GetComponent<Scoring>();
		dt = GameObject.Find("SceneCode").GetComponent<DriftingText>();
		cg = GameObject.Find("SceneCode").GetComponent<CreateGrid>();
		scenePlay = GameObject.Find("SceneCode").GetComponent<ScenePlay>();

		wholeObjs.Add("diver", bonusDiver);
		wholeObjs.Add("angler", bonusAngler);
		wholeObjs.Add("puffer", bonusPuffer);
		wholeObjs.Add("urchin", bonusUrchin);
		wholeObjs.Add("submarine", bonusSubmarine);
		wholeObjs.Add("mermaid", bonusMermaid);
		wholeObjs.Add("treasurechest", bonusTreasureChest);

		// only show tutorial panel on first level of first world
		tutorial.SetActive(GameData.currWorldNum == 0 && GameData.currLevelNum == 0);

	}

	public void TwoCoversTurned()
	{
		GameData.numTries++;

		// look for a match
		matched = IsThereAMatch();

		// if we match half a good sprite with a dynamite sprite, count it as no match
		if (atLeastOneGood && dynamiteFound)
			dynamiteFound = false;

		// don't match the skulls
		// yes, match them because they may be the last two pieces
		//if (skullFound)
		//	matched = false;
		
		float pause = GameData.noMatchPause;
		if (matched)
			pause = GameData.matchPause;

		// do this if we found a match
		if (twoHalves)	// found two halves of a whole
		{
			GameData.matchesFound++;

			randY = Random.Range(-2, 2);
			coversTurned[0].transform.DOMove(new Vector3(0f, randY, 0f), 0.5f);
			coversTurned[0].GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(()=>scoring.AddToScore(100, true));
			StartCoroutine(DestroyCell(coversTurned[0], 1.5f));

			coversTurned[1].transform.DOMove(new Vector3(0f, randY, 0f), 0.5f);
			coversTurned[1].GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(()=>scoring.AddToScore(100, true));
			StartCoroutine(DestroyCell(coversTurned[1], 1.5f));

			firstCardTurned = false;
			secondCardTurned = false;

			skullFound = false;
			dynamiteFound = false;

			atLeastOneGood = false;

			SoundManager.PlaySFX("mergehalvesalt");

			Invoke("MakeNewSwimmer", 0.5f);

			if (tutorial != null)
			{
				Destroy(tutorial);
			}

			GameData.canClick = true;

			CheckForFinish();
		}
		else if (matched)
		{
			skullFound = false;	// no swapping if skulls were matched

			GameData.matchesFound++;

			dt.MakeDriftingText("50", coversTurned[0].transform.position, 2f);
			dt.MakeDriftingText("50", coversTurned[1].transform.position, 2f);
			coversTurned[0].GetComponent<Rigidbody2D>().DORotate(359f, 0.25f).SetDelay(0.2f);
			coversTurned[1].GetComponent<Rigidbody2D>().DORotate(359f, 0.25f).SetDelay(0.2f);

			Invoke("HandleMatchedPair", GameData.matchPause);
			}
		else // there was no match
		{
			if (!skullFound && !dynamiteFound)
			{
				if (GameData.playAudio)
					SoundManager.PlaySFX("sinkDrain1");

				if (scoring.score > 0)
					dt.MakeDriftingText("-20", new Vector2(1f, 3.33f), 1f, -0.5f);
				scoring.AddToScore(-20);

				coversTurned[0].transform.DOShakeRotation(0.7f, 40f).SetDelay(0.2f);
				coversTurned[1].transform.DOShakeRotation(0.7f, 40f).SetDelay(0.2f);
				Invoke("HideObjects", pause);
			}
		}

		if (skullFound)
		{
			// get the row & col of the other object
			int2 posOne = coversTurned[0].GetComponent<GridCell>().gridPos;
			int2 posTwo = coversTurned[1].GetComponent<GridCell>().gridPos;
			bool swapRows = true;
			if (posOne.y == posTwo.y)	//int2 x/y is row/col
				swapRows = false;
			SwapCells(swapRows, posOne, posTwo);
			firstCardTurned = false;
			secondCardTurned = false;
			skullFound = false;
			dynamiteFound = false;
			atLeastOneGood = false;
			Invoke("HideObjects", pause);
		}
		else if (dynamiteFound)
		{
			// get the color of the other object and see
			string nameOne = coversTurned[0].name;
			string nameTwo = coversTurned[1].name;
			string colorToGrab = nameOne;
			if (nameOne == "dynamite")
				colorToGrab = nameTwo;
			if (nameOne == nameTwo)
				Invoke("HideObjects", pause);
			else
			{
				StartCoroutine(KillAllMatchingGems(colorToGrab, "dynamite"));
				//Destroy(coversTurned[0]);
				//Destroy(coversTurned[1]);
				firstCardTurned = false;
				secondCardTurned = false;
				skullFound = false;
				dynamiteFound = false;
				atLeastOneGood = false;
			}
		}
			

	}


	void HandleMatchedPair()
	{
		coversTurned[0].transform.DOMove(new Vector3(1f, 3.4f, 0f), 0.5f);
		coversTurned[0].GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(()=>scoring.AddToScore(50, true));
		StartCoroutine(DestroyCell(coversTurned[0], 1f));

		coversTurned[1].transform.DOMove(new Vector3(1f, 3.4f, 0f), 0.5f);
		coversTurned[1].GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnComplete(()=>scoring.AddToScore(50, true));
		StartCoroutine(DestroyCell(coversTurned[1], 1f));

		if (GameData.playAudio)
			SoundManager.PlaySFX("woosh8");
		
		firstCardTurned = false;
		secondCardTurned = false;

		skullFound = false;
		dynamiteFound = false;

		atLeastOneGood = false;

		GameData.canClick = true;

		CheckForFinish();
	}

	bool IsThereAMatch()
	{
		bool match = false;
		Sprite oneObj = coversTurned[0].GetComponent<SpriteRenderer>().sprite;
		Sprite twoObj = coversTurned[1].GetComponent<SpriteRenderer>().sprite;

		if (MatchingLeftRight(oneObj.name, twoObj.name))
		{
			oneObj = twoObj;
			twoHalves = true;
		}
		else
			twoHalves = false;

		if (oneObj == twoObj)
			match = true;

		return (match);
	}

	private bool MatchingLeftRight(string name1, string name2)
	{
		atLeastOneGood = false;
		bool match = false;
		bool firstGood = false;
		bool secondGood = false;
		string firstName = "";
		string secondName = "";
		string suffix1 = "";
		string suffix2 = "";

		string[] splitString = name1.Split('-');
		if (splitString.Length > 1)
		{
			firstName = splitString[0];
			firstGood = (splitString[1] == "right") || (splitString[1] == "left");
			suffix1 = splitString[1];
			atLeastOneGood = true;
		}

		splitString = name2.Split('-');
		if (splitString.Length > 1)
		{
			secondName = splitString[0];
			secondGood = (splitString[1] == "right") || (splitString[1] == "left");
			suffix2 = splitString[1];
			atLeastOneGood = true;
		}

		if (firstGood && secondGood && (suffix1 != suffix2) && (firstName == secondName))
		{
			match = true;
			newWholeName = splitString[0];
		}
		else
			newWholeName = "";

		return match;
	}

	private void MakeNewSwimmer()
	{
		print("newWholeName " + newWholeName);
		newWhole = wholeObjs[newWholeName];
		GameObject expl = CFX_SpawnSystem.GetNextObject(mergeEffect);
		expl.transform.position = new Vector3(0f, randY, 0f);
		dt.MakeDriftingText("200", new Vector3(0f, randY, 0f), 2f);
		if (newWhole != null)
			Instantiate(newWhole, new Vector3(0f, randY, 0f), Quaternion.identity);
		
	}

	public void HideObjects()
	{
		firstCardTurned = false;
		secondCardTurned = false;

		skullFound = false;
		dynamiteFound = false;

		atLeastOneGood = false;

		RecoverObject(coversTurned[0]);
		RecoverObject(coversTurned[1]);

		GameData.canClick = true;

		CheckForFinish();
	}
		
	public void RecoverObject(GameObject obj)
	{
		if (!matched)
		{
			obj.GetComponent<SpriteRenderer>().sprite = coverSprite;
			//isHidden = false;
			coversTurned[0].GetComponent<Cover>().isHidden = false;
			coversTurned[1].GetComponent<Cover>().isHidden = false;

			GameData.canClick = true;

		}

	}
		
	/*
	 *  Have to look for the Cover > Cover > hiddenObj to match against the name
	 * */

	IEnumerator KillAllMatchingGems(string theName, string catalyst)
	{
		List<GameObject> matchingCells = new List<GameObject>();

		for (int row = 0; row < cg.numRows; row++)
		{
			for(int col = 0; col < cg.numCols; col++)
			{
				int2 gridPos = new int2(col, row);
				if (GameData.gridPieces.ContainsKey(gridPos))
				{
					GameObject gObj = GameData.gridPieces[gridPos];
					if (gObj != null && gObj.GetComponent<Cover>().hiddenObj.name == theName 
						|| gObj != null && gObj.GetComponent<Cover>().hiddenObj.name == catalyst)
					{
						matchingCells.Add(gObj);
						if (gObj.GetComponent<SpriteRenderer>().sprite.name == "seashell2")
						{
							SoundManager.PlaySFX("splash");
						}
						gObj.GetComponent<Cover>().ShowHiddenObject();
						GameObject expl = CFX_SpawnSystem.GetNextObject(matchedCovers);
						expl.transform.position = gObj.transform.position;

						yield return new WaitForSeconds(0.25f);
					}
				}
			}
		}

		bool captured = false;
		int c = 1;
		// now that we have a list of the matching gems, blow them up one at a time.
		if (matchingCells.Count > 0)
		{
			foreach(GameObject gObj in matchingCells)
			{
				yield return new WaitForSeconds(0.5f);

				SoundManager.PlaySFX("implosion");
				GameObject expl = CFX_SpawnSystem.GetNextObject(explodeCover);
				expl.transform.position = gObj.transform.position;
				ProCamera2DShake.Instance.Shake("New ShakePreset");
				if (scoring.score > 0)
				{
					dt.MakeDriftingText("-10", gObj.transform.position);
					scoring.AddToScore(-10);
				}
				StartCoroutine(DestroyCell(gObj));
				//Application.CaptureScreenshot("Screenshot" + c + ".png");
				//c++;
			}

			GameData.matchesFound += matchingCells.Count/2;

			GameData.canClick = true;

			//Invoke("CheckForFinish", 1f);
			CheckForFinish();
		}

	}

	void ShowCellObjects()
	{
		string allLines = "";

		for (int row = 0; row < cg.numRows; row++)
		{
			string oneLine = "";
			for(int col = 0; col < cg.numCols; col++)
			{
				int2 gridPos = new int2(col, row);
				if (GameData.gridPieces.ContainsKey(gridPos))
					oneLine += "1";
				else
					oneLine += "0";

			}
			allLines += '\n' + oneLine;
		}
		Debug.Log(allLines);
	}

	void SwapCells(bool swapRows, int2 posOne, int2 posTwo)
	{
		int firstCol, firstRow, secondCol, secondRow;

		if (swapRows)
		{	// int2 is row/col, which is y/x, so y=row and x=col

			ShowCellObjects();


			firstCol = posOne.x;
			secondCol = firstCol;
			firstRow = posOne.y;
			secondRow = posTwo.y;
			//print("first row:" + firstRow + " second row:" + secondRow);
			for(int col = 0; col < cg.numCols; col++)
			{
				int2 firstGridPos = new int2(col, firstRow);
				int2 secondGridPos = new int2(col, secondRow);
				Vector3 firstPosition = GameData.gridCells[firstGridPos].transform.position;
				Vector3 secondPosition = GameData.gridCells[secondGridPos].transform.position;
				//Debug.Log(firstPosition);
				//Debug.Log(secondPosition);

				//if (GameData.gridPieces.ContainsKey(firstGridPos) && GameData.gridPieces.ContainsKey(secondGridPos))
				//{

				GameObject gObjOne = null;
				GameObject gObjTwo = null;

				if (GameData.gridPieces.ContainsKey(firstGridPos))
					gObjOne = GameData.gridPieces[firstGridPos];
				if (GameData.gridPieces.ContainsKey(secondGridPos))
					gObjTwo = GameData.gridPieces[secondGridPos];

				if (gObjOne != null)
					{
						gObjOne.transform.DOMove(secondPosition, 1f);
						// now update each piece
						gObjOne.GetComponent<GridCell>().gridPos = secondGridPos;
						//GameData.gridPieces.Remove(firstGridPos);
						//GameData.gridPieces.Add(secondGridPos, gObjTwo);
						GameData.gridPieces[secondGridPos] = gObjOne;
					}
				else
					GameData.gridPieces.Remove(secondGridPos);

				if (gObjTwo != null)
				{
					gObjTwo.transform.DOMove(firstPosition, 1f);
					// now update each piece
					gObjTwo.GetComponent<GridCell>().gridPos = firstGridPos;
					//GameData.gridPieces.Remove(secondGridPos);
					//GameData.gridPieces.Add(firstGridPos, gObjOne);
					GameData.gridPieces[firstGridPos] = gObjTwo;
				}
				else
					GameData.gridPieces.Remove(firstGridPos);

				//}
			}
		}
		else
		{	// int2 is row/col, which is y/x, so y=row and x=col
			firstCol = posOne.x;
			secondCol = posTwo.x;
			firstRow = posOne.y;
			secondRow = firstRow;
			print("first col:" + firstCol + " second col:" + secondCol);
			for(int row = 0; row < cg.numRows; row++)
			{
				int2 firstGridPos = new int2(firstCol, row);
				int2 secondGridPos = new int2(secondCol, row);
				Vector3 firstPosition = GameData.gridCells[firstGridPos].transform.position;
				Vector3 secondPosition = GameData.gridCells[secondGridPos].transform.position;

				GameObject gObjOne = null;
				GameObject gObjTwo = null;

				if (GameData.gridPieces.ContainsKey(firstGridPos))
					gObjOne = GameData.gridPieces[firstGridPos];
				if (GameData.gridPieces.ContainsKey(secondGridPos))
					gObjTwo = GameData.gridPieces[secondGridPos];

				if (gObjOne != null)
				{
					gObjOne.transform.DOMove(secondPosition, 1f);
					// now update each piece
					gObjOne.GetComponent<GridCell>().gridPos = secondGridPos;
					//GameData.gridPieces.Remove(firstGridPos);
					//GameData.gridPieces.Add(secondGridPos, gObjTwo);
					GameData.gridPieces[secondGridPos] = gObjOne;
				}
				else
					GameData.gridPieces.Remove(secondGridPos);

				if (gObjTwo != null)
				{
					gObjTwo.transform.DOMove(firstPosition, 1f);
					// now update each piece
					gObjTwo.GetComponent<GridCell>().gridPos = firstGridPos;
					//GameData.gridPieces.Remove(secondGridPos);
					//GameData.gridPieces.Add(firstGridPos, gObjOne);
					GameData.gridPieces[firstGridPos] = gObjTwo;
				}
				else
					GameData.gridPieces.Remove(firstGridPos);
			}
		}
		// get rid of skulls at the end
		// no, leave them there until they are matched

		//GameData.canClick = true;

		print ("Done Swapping");

	}


	public void CheckForFinish()
	{
		//print(GameData.matchesFound+1 + "  " +  (GameObject.Find("SceneCode").GetComponent<CreateGrid>().numCells/2));

		//Debug.Log("GameData.matchesFound " + GameData.matchesFound);

		if (GameData.matchesFound == (GameObject.Find("SceneCode").GetComponent<CreateGrid>().totalActiveCells/2))
		{
			print("WINNER!");

			scenePlay.UnlockNextLevel();

			SoundManager.PlaySFX("winSFXalt");
			Invoke("ShowWinningScreen", 1.0f);
		}
	}

	void ShowWinningScreen()
	{
		if (tutorial != null)
		{
			Destroy(tutorial);
		}

		newHighScore = false;
		string lvlName = "w" + GameData.currWorldNum.ToString() + GameData.levelNames[GameData.currLevelNum];

		// save the score if it's better than what we've done before
		if (PlayerPrefs.GetInt(lvlName + "Score", 0) < scoring.score)
			PlayerPrefs.SetInt(lvlName + "Score", scoring.score);

		if (scoring.score > PlayerPrefs.GetInt(lvlName + "HighScore"))
		{
			newHighScore = true;
			PlayerPrefs.SetInt(lvlName + "HighScore", scoring.score);
		}

		// disable Pause so it can't be clicked while Game Over screen is showing.
		pauseBtn.gameObject.SetActive(false);

		if (newHighScore)
		{
			GameObject wParts = Instantiate(winParticles, new Vector3(-4, 4, 0), Quaternion.identity) as GameObject;
			wParts.transform.DOMoveX(3f, 3f);
			StartCoroutine(DoWinParticles(wParts));
		}

		gameOverPanel.SetActive(true);
		winScoreText.text = "Number of tries: " + GameData.numTries + '\n' + "Total score: " + scoring.score;
		if (newHighScore)
			winScoreText.text += "\nNEW HIGH SCORE!";
		else
			winScoreText.text += "\nHigh score: " + PlayerPrefs.GetInt(lvlName + "HighScore");

		if (GameData.currLevelNum == GameData.numGamesInLevel-1)
		{
			levelOverPanel.transform.position = new Vector2(3f, 4f);
			levelOverPanel.SetActive(true);
			levelOverPanel.transform.DOMove(new Vector3(3f, -3f, 0f), 1.5f);
			levelOverPanel.transform.DOShakeScale(2.5f, 0.2f).SetDelay(1.5f);

			// unlock the next world if we're a desktop (standalone) version, or if they bought the no-ads pack.
			bool standAlone = false;
			#if UNITY_STANDALONE
			standAlone = true;
			#endif
			if ( standAlone || (PlayerPrefs.GetInt("showAds", 1) == 0) )
				UnlockNextWorld();

			int mermaidStoryNum = GameData.currWorldNum+1;
			PlayerPrefs.SetInt("mermaidStory", mermaidStoryNum);

			GameData.showStory = true;
		}
	}

	IEnumerator DoWinParticles(GameObject obj) {
		//emitter.emit = true;
		yield return new WaitForSeconds(3);
		obj.GetComponent<ParticleSystem>().Stop();
	}

	void UnlockNextWorld()
	{
		Debug.Log("Unlock next world here");
		int x;
		for (x = 0; x < GameData.worldsUnlocked.Length; x++)
		{
			if (! GameData.worldsUnlocked[x])
			{
				PlayerPrefs.SetInt(GameData.worldNames[x] + "Unlocked", 1);
				GameData.worldsUnlocked[x] = true;
				break;
			}
		}
	}


	IEnumerator DestroyCell(GameObject gObj, float delay = 0f)
	{
		yield return new WaitForSeconds(delay);
		//gObj.GetComponent<GridCell>().active = false;
		//gObj.GetComponent<SpriteRenderer>().sprite = null;
		GameData.gridPieces.Remove(gObj.GetComponent<GridCell>().gridPos);
		//gObj.GetComponent<Cover>().enabled = false;
		//gObj = null;
		Destroy(gObj);
	}
}
