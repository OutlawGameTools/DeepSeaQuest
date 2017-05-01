using UnityEngine;
using System.Collections;

public class Audio : MonoBehaviour 
{

	public AudioClip bgMusic;


	public void TurnOffMusic()
	{
		GameData.playAudio = false;
		PlayerPrefs.SetInt("playAudio", 0);
		// turn off any bg music
		SoundManager.MuteMusic(true);
		SoundManager.MuteSFX(true);
		//GameObject.Find("SceneCode").GetComponent<AudioSource>().Stop();
	}

	public void TurnOnMusic()
	{
		GameData.playAudio = true;
		PlayerPrefs.SetInt("playAudio", 1);
		// turn on any bg music
		//GameObject.Find("SceneCode").GetComponent<AudioSource>().Play();
		SoundManager.MuteMusic(false);
		SoundManager.MuteSFX(false);
	}


}
