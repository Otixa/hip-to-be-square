using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevelPopup : MonoBehaviour {
	public Text theTextBox;							//this is the reference to the text box that shows thext in the popup Window
	//public string theMessage;						//this is used to store the text that will be displayed in the popup box
	public string theLevel;							//this is used to store the name of the scene to load when continue is next pressed
				
	void Start () {}

	void Update () {}

	public void ContinueButton(){				//this will be connected to the click event of the Continue button
		gameObject.SetActive(false);
		SceneManager.LoadScene (theLevel);		//load the scene specified when the setupPopup method was called
	}

	public void setupPopup(string winMessage, string nextLevel){
		theTextBox.text = winMessage;			//find the win text, update its text to display winMessage
		theLevel = nextLevel;					//store the level that will be used when the Continue button is clicked
	}
}
