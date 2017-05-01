using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class SceneStory : MonoBehaviour {

	public GameObject storyPanel;
	public GameObject mermaid;
	public Text storyText;
	public Text okButtonText;

	// Use this for initialization
	void Start () 
	{
		int mermaidStoryNum = PlayerPrefs.GetInt("mermaidStory", 0);
		if (mermaidStoryNum < 5 && GameData.showStory)
		{
			storyPanel.SetActive(true);
			mermaid.SetActive(true);
			ShowMermaidStory(mermaidStoryNum);
			storyPanel.transform.position = new Vector2(0f, 5f);
			storyPanel.transform.DOMove(new Vector3(0f, 0f, 0f), 1.5f);
		}
		else
		{
			SceneManager.LoadScene("worlds");
		}
		GameData.showStory = false;

		// see if we've set up world stuff
		GameData.currWorldNum = PlayerPrefs.GetInt("currWorldNum", -1);

		// if it hasn't been set yet, do the default
		if (GameData.currWorldNum == -1)
		{
			PlayerPrefs.SetInt("currWorldNum", 0);
			GameData.currWorldNum = 0;
		}

	}

	void ShowMermaidStory(int mermaidStoryNum)
	{
		/*
My name is Jocelyn and I need
help finding my way home.
I left a trail of gems, but I need
you to match them up.
I must get home to my family.

Please help me!		 


You're doing so well, I think
I'll be home soon! Be very
careful during the next section,
there is dynamite that blows up 
the gems!

Are you ready?


I'm glad you're the one helping
me, you're very good at this! Be
careful in the Deep, the pirate
skulls will cause strange things
to happen when you choose one.

Ready to match gems?


Thank you for doing such a great
job; I think I'm very near home!

Oh no, this next area has both
dynamite and pirate skulls!
Please be careful!


You did it! You matched every gem
and I can see my home in the reef!

Thank you for rescuing me. I hope
you come back and play again.
I'll leave gems out for you to match.

		* */

		switch (mermaidStoryNum)
		{
		case 0:
			storyText.text = "My name is Jocelyn and I need\nhelp finding my way home.\nI left a trail of gems, but I need\nyou to match them up.\nI must get home to my family.\n\nPlease help me!";
			okButtonText.text = "I'll Help You!";
			break;
		case 1:
			storyText.text = "You're doing so well, I think\nI'll be home soon! Be very\ncareful during the Medium section;\nthere are bundles of dynamite\nthat blow up the gems!\n\nAre you ready?";
			okButtonText.text = "I'm Ready!";
			break;
		case 2:
			storyText.text = "I'm glad you're the one helping\nme, you're very good at this!\nBe careful in the Deep, the pirate\nskulls cause strange things\nto happen when you choose one.\n\nReady to match gems?";
			okButtonText.text = "Ready to Match!";
			break;
		case 3:
			storyText.text = "Thank you for doing such a great\njob; I think I'm very near home!\n\nOh no, the Bonus area has both\ndynamite and pirate skulls!\nPlease be careful!";
			okButtonText.text = "I'm Ready!";
			break;
		case 4:
			storyText.text = "You did it! You matched every gem\nand I can see my home in the reef!\n\nThank you for rescuing me. I hope\nyou come back and play again.\nI'll leave gems out for you to match.";
			okButtonText.text = "You're Welcome!";
			break;
		}
	}
		

	public void SawMermaidStory()
	{
		int mermaidStoryNum = PlayerPrefs.GetInt("mermaidStory", 0);
		mermaidStoryNum++;
		PlayerPrefs.SetInt("mermaidStory", mermaidStoryNum);
	}

}
