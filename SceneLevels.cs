using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneLevels : MonoBehaviour {

	bool allLevelsUnlocked = true;

	public GameObject cellPrefab;

	public float gridCenterX = 0f;
	public float gridCenterY = 0f;

	int numCols = 3;
	int numRows = 3;
	int numCells;
	int totalActiveCells = 0;

	float coverWidth = 1f;
	float coverHeight = 0.85f;
	public float rowSpacing = 1.25f;
	public float colSpacing = 1.25f;

	public Canvas canvas;

	void Awake() 
	{
		if (GameData.isBetaLevels)
		{
			numCols = 1;
			numRows = 1;
			GameData.numGamesInLevel = 1;
		}
	}

	// Use this for initialization
	void Start () 
	{
		GameData.currLevelNum = PlayerPrefs.GetInt("currLevelNum", -1);

		// if it hasn't been set yet, do the default
		if (GameData.currLevelNum == -1)
		{
			PlayerPrefs.SetInt("currLevelNum", 0);
			GameData.currLevelNum = 0;
		}

		WhichLevelsAreUnlocked();

		BuildLevelChooser();
	}

	void WhichLevelsAreUnlocked()
	{
		// see what levels should be unlocked

		for (int x = 0; x < GameData.numGamesInLevel; x++) // jij removed -1
		{
			string lvlName = "w" + GameData.currWorldNum.ToString() + GameData.levelNames[x];
			int levelUnlocked = PlayerPrefs.GetInt(lvlName + "Unlocked", 0);

			if (x == 0)
				levelUnlocked = 1;	// first world is always unlocked

			GameData.levelsUnlocked[x] = (levelUnlocked == 1);
			//Debug.Log(lvlName + " " + GameData.levelsUnlocked[x].ToString());

			allLevelsUnlocked = allLevelsUnlocked && GameData.levelsUnlocked[x];
		}

		GameData.levelsUnlocked[0] = true;
	}


	void BuildLevelChooser()
	{
		CreateAGrid();
	}

	public void SetLevel(int levelNum)
	{
		GameData.currLevelNum = levelNum;
	}

	void CreateAGrid () {

		//		gridBlocks = new Dictionary<int2, GameObject>();

		float xPos = gridCenterX - ((numCols - 1) * colSpacing) / 2;
		float yPos = gridCenterY + ((numRows - 1) * rowSpacing) / 2;

		int blockCount = 0;

		for (int row = 0; row < numRows; row++)
		{
			for(int col = 0; col < numCols; col++)
			{
				string lvlName = "w" + GameData.currWorldNum.ToString() + GameData.levelNames[blockCount] + "HighScore";

				float cellX = xPos + (col * colSpacing);
				float cellY = yPos - (row * rowSpacing);
				Vector2 startPos = new Vector2(cellX, cellY);

				GameObject cellObj = Instantiate(cellPrefab, startPos, Quaternion.identity) as GameObject;

				Text[] textComps = cellObj.GetComponentsInChildren<Text>();
				foreach(Text tComp in textComps)
					if (tComp.name == "LevelNum")
						tComp.text = (blockCount+1).ToString();
					else if (tComp.name == "Score")
						tComp.text = PlayerPrefs.GetInt(lvlName, 0).ToString();
							
				cellObj.transform.SetParent(canvas.transform, true);
				cellObj.transform.localScale = new Vector3(coverWidth, coverHeight, 0);

				Button btn = cellObj.GetComponent<Button>();
				btn.onClick.AddListener(delegate {
					ButtonTapped(btn, blockCount); });

				int2 gridPos = new int2(row,col);
				cellObj.GetComponent<GridCell>().gridPos = gridPos;
				cellObj.GetComponent<GridCell>().numSeq = blockCount++;
				//cellObj.GetComponent<Cover>().hiddenObj = availableSprites[blockCount-2];

				// see if we should show the lock icon or not
				if (GameData.levelsUnlocked[blockCount-1])
				{
					Component[] lvlSprites;
					lvlSprites = btn.GetComponentsInChildren( typeof(SpriteRenderer), true);
					if (lvlSprites != null)
					{
						foreach (SpriteRenderer sprRend in lvlSprites)
						{
							if (sprRend.name == "Lock")
								sprRend.gameObject.SetActive(false);
						}
					}
					btn.interactable = true;
				}
			}
		}
	}

	void ButtonTapped(Button me, int levelNum)
	{
		int theLevel = me.GetComponent<GridCell>().numSeq;
		SetLevel(theLevel);
		SceneManager.LoadScene ("play");
	}
		

}
