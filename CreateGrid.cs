using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class CreateGrid : MonoBehaviour {

//	public Dictionary<int2, GameObject> gridBlocks;

	public GameObject cellPrefab;
	public GameObject coverPrefab;
	public Sprite[] cellSprites;
	public Sprite[] goodSprites;
	public Sprite[] badSprites;

	public float gridCenterX = 0f;
	public float gridCenterY = 0f;

	public int numCols = 10;
	public int numRows = 6;
	public int numCells;
	public int totalActiveCells = 0;

	float coverWidth = 1f;
	float coverHeight = 0.85f;
	public float rowSpacing = 1.25f;
	public float colSpacing = 1.25f;

	int numGoodCells = 3;
	int numBadCells = 1;

	[SerializeField]
	private Sprite[] availableSprites;

	LevelMaps levelMaps;

	int[,] currLevel;

	Scoring scoring;

	void Awake()
	{
		scoring = GameObject.Find("SceneCode").GetComponent<Scoring>();
	}

	void Start()
	{
		SetThisLevel();
	}

	public void SetThisLevel()
	{

		scoring.AddToScore(-scoring.score);	// erase any current score

		levelMaps = GameObject.Find("SceneCode").GetComponent<LevelMaps>();

		Debug.Log("GameData.currWorldNum " + GameData.currWorldNum );
		Debug.Log("GameData.currLevelNum " + GameData.currLevelNum );
		//currLevel = levelMaps.levels.ElementAt(GameData.currLevelNum);
		currLevel = levelMaps.worlds.ElementAt(GameData.currWorldNum).ElementAt(GameData.currLevelNum);

		GameData.numGamesInLevel = levelMaps.worlds.ElementAt(GameData.currWorldNum).Count;
			
		// get the dimensions of the grid from the level map
		numRows = currLevel.GetLength(0);
		numCols = currLevel.GetLength(1);

		GameData.matchesFound = 0;
		GameData.numTries = 0;

		totalActiveCells = 0;

		numCells = numCols * numRows;

		GameData.gridPieces = new Dictionary<int2, GameObject>();
		GameData.gridCells = new Dictionary<int2, GameObject>();

		//--------------------------
		// change size of objects and spacing based on numRows * numCols

		if (numCells >= 41)
		{
			print("numCells >= 41");
			rowSpacing = 1f;
			colSpacing = 1f;
			coverWidth = 0.6f;
			coverHeight = 0.5f;
			numBadCells = 2;
			numGoodCells = 5;
		}

		if (numCells <= 40)
		{
			print("numCells <= 40");
			rowSpacing = 1.2f;
			colSpacing = 1.25f;
			coverWidth = 0.8f;
			coverHeight = 0.65f;
			numBadCells = 2;
			numGoodCells = 3;
		}

		if (numCells <= 24)
		{
			print("numCells <= 24");
			rowSpacing = 1.5f;
			colSpacing = 1.65f;
			coverWidth = 1f;
			coverHeight = 0.85f;
			numBadCells = 0;
			numGoodCells = 1;
		}

		if (GameData.currWorldNum == 0)
		{
			numBadCells = 0;
			numGoodCells = 2;
		}
		if (GameData.currWorldNum == 1)
		{
			numBadCells = 1;
			numGoodCells = 3;
		}
		if (GameData.currWorldNum == 2)
		{
			numBadCells = 2;
			numGoodCells = 4;
		}
		if (GameData.currWorldNum == 3)
		{
			numBadCells = 2;
			numGoodCells = 5;
		}

		if (GameData.isBetaOnlyGood)
		{
			numBadCells = 0;
			numGoodCells = 0;
		}


		CountActiveSprites();
		availableSprites = new Sprite[totalActiveCells];

		int idx = 0;
		int halfway = (totalActiveCells - (numGoodCells*2) - (numBadCells*2)) / 2;
		for (int x = 0; x < totalActiveCells; x++)
		{
			if (x % halfway == 0)
				idx = 0;

			availableSprites[x] = cellSprites[idx++];
			if (idx > cellSprites.Length-1)
				idx = 0;
		}

		if (!GameData.isBetaOnlyGood)
		{
			// do we need to add any goodCells?
			if (numGoodCells > 0)
			{
				idx = 0;
				int thisMany = numGoodCells * 2;
				for (int x = 0; x < thisMany; x++)
				{
					availableSprites[x++] = goodSprites[idx++];
					availableSprites[x] = goodSprites[idx++];
				}
			}
				
			// do we need to add any badCells?
			if (numBadCells > 0)
			{
				idx = 0;
				int thisMany = numBadCells * 2;
				for (int x = (numGoodCells) * 2; x < (numGoodCells * 2) + thisMany; x++)
				{
					availableSprites[x++] = badSprites[idx];
					availableSprites[x] = badSprites[idx++];
				}
			}
		}

		ShuffleSprites();
		CreateAGrid();
	}

	// how many cells will have something in them? 
	void CountActiveSprites()
	{
		for (int row = 0; row < numRows; row++)
			for(int col = 0; col < numCols; col++)
				if (currLevel[row,col] > 0)
					totalActiveCells++;
	}

	void CreateAGrid () {

//		gridBlocks = new Dictionary<int2, GameObject>();

		float xPos = gridCenterX - ((numCols - 1) * colSpacing) / 2;
		float yPos = gridCenterY + ((numRows - 1) * rowSpacing) / 2;

		int blockCount = 1;

		for (int row = 0; row < numRows; row++)
		{
			for(int col = 0; col < numCols; col++)
			{
				float cellX = xPos + (col * colSpacing);
				float cellY = yPos - (row * rowSpacing);
				Vector2 startPos = new Vector2(cellX, cellY);
				int2 gridPos = new int2(col,row); // which col = x, which row = y

				// add grid cell marker whether there's anything to see or not
				GameObject cellObj = Instantiate(cellPrefab, startPos, Quaternion.identity) as GameObject;
				cellObj.GetComponent<GridCell>().gridPos = gridPos;
				cellObj.GetComponent<GridCell>().xPos = cellX;
				cellObj.GetComponent<GridCell>().yPos = cellY;
				cellObj.transform.SetParent(GameObject.Find("SceneCode").transform);

				GameData.gridCells.Add(gridPos, cellObj);

				if (currLevel[row,col] > 0)
				{
					GameObject coverObj = Instantiate(coverPrefab, startPos, Quaternion.identity) as GameObject;
					coverObj.transform.localScale = new Vector3(coverWidth, coverHeight, 0);
					coverObj.GetComponent<GridCell>().gridPos = gridPos;

					coverObj.GetComponent<Cover>().hiddenObj = availableSprites[blockCount-1];

					GameData.gridPieces.Add(gridPos, coverObj);

					blockCount++;
				}
				//print(blockCount-2);
//				gridBlocks.Add(gridPos, cellObj);

			}
		}
	}

	void ShuffleSprites()
	{
		for (int x = 0; x < availableSprites.Length; x++)
		{
			Sprite first = availableSprites[x];

			int idx = Random.Range(0, availableSprites.Length);
			Sprite second = availableSprites[idx];

			availableSprites[idx] = first;
			availableSprites[x] = second;
		}
	}

	/*
	void DitchAllGridBlocks()
	{
		for (int i = 0; i < numCols; i++)
		{
			for(int j = 0; j < numRows; j++)
			{
				int2 gridPos = new int2(i,j);
				if (gridBlocks[gridPos] != null)
					Destroy(gridBlocks[gridPos]);
			}
		}

		gridBlocks.Clear();
	}
	*/

}