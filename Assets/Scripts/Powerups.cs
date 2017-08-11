using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//***************************************OBSOLETE - NO LONGER USED***************************************************//

/* This component will be attached to pick ups, and specifies the type of pickup that it is. It is also used in ]
 * order to randomly choose the type of pickup that is generated when it's set to be active in the game world */
public class Powerups : MonoBehaviour {
	public bool slowDownBoost;		//boolean to flag if the powerup collected was a slow down time booster
	public bool pointsBoost;		//flag to track if the powerup collected was a point multiplier
	public float amount;			//value that can be passed with a powerup that denotes amount of "units" to award.
	public float duration;			//denotes how long a powerup is active for
	private PowerupManager thePowerupManager;		//reference to the powerup manager, that we call on-trigger
	public Sprite[] powerupSprites;	//array of the different graphics for the different power ups - this allows us to use one object to represent several different powerups#
	public bool respawnOnReset;

	void Start () {
		thePowerupManager = FindObjectOfType<PowerupManager> ();			//reference to the powerupmanager so we can pass it the details of the current powerup
	}

	void Awake(){										//Awake is called whenever an object is set to be active
		if (!respawnOnReset) {
			int powerupSelector = Random.Range (0, 2);		//when a powerup is spawned, this will decide what type of powerup that will be spawned based on chance
			switch (powerupSelector) {						//list of the different settings for the different type of powerups
			case 1:		//POINTS INCREASING POWERUP
				pointsBoost = true;							//set this powerup to be a pointsBoost powerup
				duration = 5;								//set the length of this powerup (to be passed to the powerup manager to handle)
				break;										//need to break in a switch statement so we don't continue executing any other code
			case 0:		//SLOW DOWN TIME POINTS INCREASE
				slowDownBoost = true;						//set this powerup to be a slowdownBoost
				amount = 1;									//set the amount of slow down points to award for collecting this powerup
				break;
			}
			GetComponent<SpriteRenderer> ().sprite = powerupSprites [powerupSelector];		//set the spirte (picture) for the powerup based on what powerup it is
		} else {
			//nothing needs to change
		}
	}

	void Update () { }

	void OnTriggerEnter2D(Collider2D other){				//whenever something activates my trigger
		if (other.tag == "Player") {						//check if it's the player (rather than coin / spike..)
			thePowerupManager.ActivatePowerup (slowDownBoost, pointsBoost, amount, duration);		//send the information about the powerup to the powerup manager using the ActivatePowerup function
			gameObject.SetActive (false);					//once you've gained the effect of the powerup, disable the object from the game world.
		}
	}
}
