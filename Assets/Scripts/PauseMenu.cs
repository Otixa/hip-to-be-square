using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* This script will handle both the pause button actions, and the calling of the pause menu itself 
 * it will need to be attached to the pause button rather than the pause menu - as the pause menu
 * will be deactive, and therefore script won't be able to be interacted with. Pause button will
 * be active always though. */
public class PauseMenu : MonoBehaviour {
	public string mainMenuScene;		//this will store the name of the main menu scene
	public GameObject pauseMenu;		//we pass into this function via unity the pause menu - could be done with a Find alternatively

	public void PauseGame(){			//method linked to the click event of the Pause button
		Time.timeScale = 0f;			//this sets the rate of frames to be 0. 
		pauseMenu.SetActive (true);		//this enables the pause menu, which is a canvas based UI
	}


	public void ResumeGame(){			//method linked to the pressing of the Resume Game button
		Time.timeScale = 1f;			//restore the time to detault speed
		pauseMenu.SetActive (false);	//disable the canvas based UI for the Pause menu
	}
		
	public void Restart () {			//method linked to the click event of the restart button
		Time.timeScale = 1f;			//reset the time to its original scale
		FindObjectOfType<GameManager>().Reset();	//this is quicker than assigning it to a variable etc. 
		pauseMenu.SetActive (false);	//disable the canvas based UI for the Pause menu
	}
		
	public void MainMenu () {			//method linked to the Main menu button in the pause menu
		Time.timeScale = 1f;			//reset the time to its normal speed
		SceneManager.LoadScene (mainMenuScene);	//change the unity scene to be the main menu scene (the name is passed in via parameter).
	}
}