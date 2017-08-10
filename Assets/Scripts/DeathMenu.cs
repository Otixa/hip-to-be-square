using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*This component contains the actions that will be taken when you click on the Restart or Main Menu buttons within the DeathMenu Canvas*/
public class DeathMenu : MonoBehaviour {

	public string mainMenuScene;								//a way to speicify the name of the main menu scene
	private GameManager theGameManager;							//reference to the game manager

	public void Restart () {									//this function will be linked to the click of the restart button
		theGameManager = FindObjectOfType<GameManager>();		//locate the game manager so we can get it to restart the game
		theGameManager.Reset ();								//this function resets the game back to the starting state
		gameObject.SetActive (false);							//deactivate the death mennu so it no longer remains displayed on screen
	}

	public void MainMenu () {									//this function will be linked to the click of the Main Menu button
		SceneManager.LoadScene (mainMenuScene);					//load the scene, using the name of the sceen set as a parameter to this function
	}
}
