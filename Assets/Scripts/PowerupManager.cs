using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This component will manage how to react when different powerups are collected. Does nothing unless PowerupActive is true.
 * also includes a method which will be called when pickups are triggered by the player */
public class PowerupManager : MonoBehaviour {
	private bool slowDownBoost;					//boolean to flag if the powerup collected was a slow down time booster
	private bool doublePoints;					//flag to track if the powerup collected was a point multiplier
	private float amount;						//value that can be passed with a powerup that denotes amount of "units" to award.
	private GameManager theGameManager;			//reference to game manager to find out if games' been reset and powerups should be deactivated
	private ScoreManager theScoreManager;		//reference to score manager so powerups that affect points gained can change the points per second
	private PlayerController thePlayer;			//reference to the player controller so that we can amend the amount of slow-down points the player has with a powerup
	private bool powerupActive;					//the main boolean that tells the component to make changes to the game based on the powerup that's currently active
	private float powerupTimeCounter;			//a counter that is used to count down as the powerup is active (denotes how long a powerup is active for)
	private float originalPointRate;			//we need this so that we keep track of the original points per second so we can revert to it once powerup expires

	void Start () {
		theGameManager = FindObjectOfType<GameManager> ();			
		theScoreManager = FindObjectOfType<ScoreManager> ();		
		thePlayer = FindObjectOfType<PlayerController> ();			
		originalPointRate = theScoreManager.pointsPerSecond;
		Debug.Log ("I'M FUCKING SHIT UP DAWG");
	}

	void Update () {
		if (powerupActive) {								//only do the effect if the powerup is active (important as our bools from the last powerup would otherwise still be accessible)
			powerupTimeCounter -= Time.deltaTime;			//reduce the active time 
			if (theGameManager.powerupReset) {				//just check to see if the game has been reset whilst the power up was active. powerupReset will get set to be true if it has
				powerupTimeCounter = 0;						//expire the time remaining of any powerup
				theGameManager.powerupReset = false;		//reset the boolean that says powerups need to be reset (that is set true when the game is reset through menu / death).
				Time.timeScale = 1f;						//revert to standard time
			}
			if (doublePoints) {													//if you've collected the double points powerup
				theScoreManager.pointsPerSecond = originalPointRate * 2;		//adjust the points per second within our score manager	
			}
			if (powerupTimeCounter <= 0) {										//if the time of the powerup has expired
				powerupActive = false;											//disable the powerup active flag
				theScoreManager.pointsPerSecond = originalPointRate;			//revert the points bonus to regular amount
				Time.timeScale = 1f;											//reset the timescale back to 1. IMPROVEMENT: Do we actually need this?
			}
		}
		if (slowDownBoost) {													//if the powerup was a one off boost, without a duration
			thePlayer.slowSpeedPoints = Mathf.Clamp(thePlayer.slowSpeedPoints+amount, 0, thePlayer.slowSpeedPointsMax);	//add to the players slow time points, but stop it from going over the max points
			slowDownBoost = false;												//reset the flag of this powerup
			powerupActive = false;												//reset the powerupActive flag (as it is turned on whenever the activate powerup method is called
		}
	}

	/* This method will be called when a collision is made with a pickup. It sets the variables so that the update method can execute the appropriate code */
	public void ActivatePowerup(bool isSlowTime, bool isDoublePoints, float theAmount, float theDuration){
		slowDownBoost = isSlowTime;												//set the variable to make it accessible within the Update method
		doublePoints = isDoublePoints;											//set the variable to make it accessible within the Update method
		amount = theAmount;														//set the variable to make it accessible within the Update method
		powerupTimeCounter = theDuration;										//set the variable to make it accessible within the Update method
		powerupActive = true;													//set the variable to make it accessible within the Update method
	}
}
