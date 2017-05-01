using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Cover : MonoBehaviour {

	public Sprite hiddenObj;
	public bool isHidden = false;
	public Sprite coverSprite;

	public GameData gameData;
	public Play play;
	public GridCell gc;

	public GameObject explodePrefab;

	public AudioClip clickSound;
	public AudioClip matchSound;
	public AudioClip noMatchSound;

	public string word;

	// Use this for initialization
	void Start () {
	
		gameData = GameObject.Find("SceneCode").GetComponent<GameData>();
		play = GameObject.Find("SceneCode").GetComponent<Play>();
		gc = GetComponent<GridCell>();
	}
	
	void OnMouseUpAsButton()
	{
		// ignore clicks/taps that fall through from Pause screen
		if (Time.timeScale < 1)
			return;

		// don't allow selections while animations and stuff are happening
		if (!GameData.canClick)
			return;
		
		// if cell is not active (pseudo-deleted) then no clicky
		//if (!gc.active)
		//	return;
		
		if (!isHidden && !play.secondCardTurned)
		{
			if (GameData.playAudio)
				AudioSource.PlayClipAtPoint(clickSound, transform.position);

			isHidden = true;

			if (!play.firstCardTurned)
			{
				play.coversTurned[0] = gameObject;
				play.firstCardTurned = true;
			}
			else
			{
				play.coversTurned[1] = gameObject;
				play.secondCardTurned = true;
			}

			// if skull or dynamite has been found, mark it so we can do something bad to the player
			if (hiddenObj.name == "skull")
				play.skullFound = true;
			if (hiddenObj.name == "dynamite")
				play.dynamiteFound = true;
			
			ShowHiddenObject();

			if (play.firstCardTurned && play.secondCardTurned)
			{
				GameData.canClick = false;

				play.TwoCoversTurned();
				//play.Invoke("TwoCoversTurned", 0f);
			}
		}

		//Destroy(gameObject);
	}

	public void ShowHiddenObject()
	{
		coverSprite = GetComponent<SpriteRenderer>().sprite;
		GetComponent<SpriteRenderer>().sprite = hiddenObj;
		gameObject.name = hiddenObj.name;
	}

	public void HideObject()
	{
		float pause = GameData.noMatchPause;
		if (play.matched)
			pause = GameData.matchPause;
		else
		{
			if (GameData.playAudio)
				AudioSource.PlayClipAtPoint(noMatchSound, transform.position);
		}
		Invoke("RecoverObject", pause);
	}

	public void RecoverObject()
	{
		if (play.matched)
		{
			if (GameData.playAudio)
				AudioSource.PlayClipAtPoint(matchSound, transform.position);
			if (play.twoHalves)
			{
				transform.DOMove(new Vector3(0f, -3.4f,0f), 0.5f);
				Destroy(gameObject, 0.5f);
			}
			else
			{
				transform.DOMove(new Vector3(1f, 3.4f,0f), 0.5f);
				GetComponent<SpriteRenderer>().DOFade(0f, 0.5f);
				Destroy(gameObject, 0.5f);
			}
		}
		else
		{
			GetComponent<SpriteRenderer>().sprite = coverSprite;
			isHidden = false;
		}

		GameData.canClick = true;
			
	}

	public void ExplodeCover()
	{
		Instantiate(explodePrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

}
