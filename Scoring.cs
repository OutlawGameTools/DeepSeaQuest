using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Scoring : MonoBehaviour {

	public Text scoreText;
	public int score;

	// Use this for initialization
	void Start () 
	{
		score = 0;
	}

	public void ResetScore()
	{
		AddToScore(-score);
	}

	public void AddToScore(int addThis = 100, bool playSFX = false)
	{
		//Debug.Log("AddToScore " + addThis);
		score += addThis;
		if (score < 0)
			score = 0;
		UpdateScoreOnScreen();
		if (playSFX)
			SoundManager.PlaySFX("Reward 3");
		
	}

	void UpdateScoreOnScreen()
	{
		scoreText.text = "Score: " + score;
	}
}
