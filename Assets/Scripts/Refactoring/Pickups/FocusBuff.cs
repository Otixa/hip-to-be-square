using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This pickup allows the player to focus without using any focus points*/
public class FocusBuff : BuffPickup {
	private GenericPlayer player;					//need this as score manager controls the scoring
	//private ScoreManager theScoreManager;         //old method
    //private float slowDownModifier;                               //IMPROVEMENT: different buffs have different slow down modifiers?
                                          

	protected override void Awake(){
        base.Awake();
        //buffDuration = 2f;                                              //Focus Buff Specific Time
        player = FindObjectOfType<GenericPlayer>();		                //we need this so we can call the add points function within it
		//theScoreManager = FindObjectOfType<ScoreManager>();	        //old way of doing
	}

	protected override void OnApply ()
	{
		player.freeFocus = true;			                    //tell the player controller it no longer needs to expend focus points
        //UIManager.Instance.SetBuff (buffDuration);	            //sorting the UI to display the buff
		//theScoreManager.setBuff (this, buffDuration);	        //old way of doing
	}

	protected override void OnExpire ()
	{	
		//Debug.Log ("I've hit disableBuff FOCUS");
		//base.DisableBuff ();							//ensures code from the base class in this method is executed
		player.freeFocus = false;                       //enable the expendeture of focus points in the player controller
        //UIManager.Instance.HideBuff();                  //test	
	}

}
