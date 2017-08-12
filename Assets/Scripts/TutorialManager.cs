using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {
	public Text tut_text;
	public bool popupActive;
	public GameObject tutorialMessage;
	private GenericPlayer thePlayerController;

	// Use this for initialization
	void Start () {
        //SetTutorialText("Let's learn a little about Joe. ");
        thePlayerController = FindObjectOfType<GenericPlayer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (popupActive && Input.GetKeyDown(KeyCode.Space)) {
			popupActive = false;
			tutorialMessage.SetActive (false);//disable the UI text
			Time.timeScale = 1f;
			thePlayerController.enabled = true;
		}
	}

	public void SetTutorialText(string message){
		Time.timeScale = 0f;			//pause the game
		tut_text.text = message;
		tutorialMessage.SetActive (true);//enable UI text
		popupActive = true;
		thePlayerController.enabled = false;	//disable the player controller script - FOR NOW
	}

}
