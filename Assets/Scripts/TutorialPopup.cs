using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TutorialPopup : MonoBehaviour {
	/* Check for a trigger event. Pause the game and display a Canvas with backdrop & text. 
	 *  Include method for closing the canvas with a button click
	 * */
	public TutorialManager theTutorialManager;
	public string theMessage;

	// Use this for initialization
	void Start () {
		theTutorialManager = FindObjectOfType<TutorialManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			theTutorialManager.SetTutorialText(theMessage);
			//GameObject tip = Find//find it
			//setActive
			//set parameter of the text to display
		}
	}

}
