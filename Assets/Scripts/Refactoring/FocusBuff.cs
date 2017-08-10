using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This pickup allows the player to focus without using any focus points*/
public class FocusBuff : BuffPickup {
	private PlayerController thePlayerController;					//need this as score manager controls the scoring
	private ScoreManager theScoreManager;


	void Awake(){
		thePlayerController = FindObjectOfType<PlayerController>();		//we need this so we can call the add points function within it
		theScoreManager = FindObjectOfType<ScoreManager>();	
	}

	protected override void OnApply ()
	{
		//Debug.Log("On Apply in Focus Buff (Blue Gem)");
		thePlayerController.freeFocus = true;			//tell the player controller it no longer needs to expend focus points
		theScoreManager.setBuff (this, buffDuration);	//sorting the UI to display the buff
	}

	protected override void OnExpire ()
	{	
		//Debug.Log ("I've hit disableBuff FOCUS");
		//base.DisableBuff ();							//ensures code from the base class in this method is executed
		thePlayerController.freeFocus = false;			//enable the expendeture of focus points in the player controller
		theScoreManager.hideBuff(); //test	
	}

}
