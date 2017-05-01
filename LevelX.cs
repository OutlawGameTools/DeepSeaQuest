using UnityEngine;
using System.Collections;

public class LevelX : MonoBehaviour {

	public int worldNum = 1;
	public int levelNum = 1;


	int[,] levelmap = new int[4,6] 
	{
		{1,1,1,1,1,1},
		{1,0,0,0,0,1},
		{1,0,0,0,0,1},
		{1,1,1,1,1,1}
	};

}
