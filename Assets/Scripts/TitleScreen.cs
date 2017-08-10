using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* This component defines what the different buttons on the title screen should do */
public class TitleScreen : MonoBehaviour {
	public string playGameLevel;					//you are able to specify the name of the Game level you want to load when pressing play game button

	public void PlayGame(){							//this is linked to the click event of the Play button
		SceneManager.LoadScene (playGameLevel);		//this changes the current scene to the specified level
	}

	public void QuitGame(){							//this is linked to the click event of the Quit button
		Application.Quit ();						//exits the application (won't work within unity editor)
	}
}
