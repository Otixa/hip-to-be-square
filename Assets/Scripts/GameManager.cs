using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This component will connect with various other classes to control the flow of the game.
 * this includes initialisation of the game, pausing, restarting, keeping scores. */
public class GameManager : MonoBehaviour {
	
	public PlayerController thePlayer;					//as we will be setting the player position at the start of game, we need his controller
	private Vector3 playerStartPoint;					//we need to store the start point of player so the player restarts consistently
	//public Transform sectionGenerator;				//NOW USE "THE MAP GEN" IN ORDER TO GET THE TRANSFORM
	private Vector3 sectionStartPoint;					//we need to store where the section generator actually started 
	private ObjectDestroyer[] destroyableObjectList;	//create this so that we can get a reference to all destroyable objects, in order to remove them when restarting the game
	private ScoreManager theScoreManager;				//we need a reference to the score manager, so that when a player is killed we can stop the scoring!
	public DeathMenu theDeathMenu;						//we need a reference to the death menu, so that we can turn it on when player dies
	private MapSectionGenerator theMapGen;				//reference needed so we can stop generating map when we accumulate enough points
	private CameraController theCameraController;		//reference needed so we can stop following the player at end of level!
	public bool powerupReset;	//UNUSUED NEED TO DELETE  Used to track if powerups need to be reset (such as on death), checked by power up manager before applying powerup effect
	public bool disableMapGen;							//A switch to turn off the map generator in levels where you want to use a predefined level
	private Powerups[] thePowerups;						//an array to check all powerups and find those which should reactivate upon game restart
	private PickupPoints[] theCoins;					//an array to check all powerups and find those which should reactivate upon game restart

	void Start () {
		theScoreManager = FindObjectOfType<ScoreManager> ();
		theMapGen = FindObjectOfType<MapSectionGenerator>();			
		theCameraController = FindObjectOfType<CameraController> ();
		sectionStartPoint = theMapGen.transform.position;				//keep note of the start position of section generator so that we can reset back to start position !!
		playerStartPoint = thePlayer.transform.position;				//keep note of the start position of the player so that we can reset back to start position
		if (disableMapGen) {											//if you do not want map generation
			theMapGen.stopGeneration = true;							//disable variables that allow the map generation with the MapSectionGenerator script
			theMapGen.endOfLevel = true;
		}
	}

	void Update () { }

	public void RestartGame(){
		theScoreManager.scoringEnabled = false;			//the have died, so stop giving them points per second
		thePlayer.gameObject.SetActive (false);			//deactivate the player - this will stop most things, as map gen / camera are based off the player movement
		theDeathMenu.gameObject.SetActive(true);		//activate the death screen
		if(!disableMapGen){								//this is to allow us to disable map generation on some levels
			theMapGen.stopGeneration = false;				//tell the mapGenerator we need any sections (including the final section) to be spawned
			theMapGen.endOfLevel = false;					//tell the mapGenerator that it should be generating random sections
		}
		theCameraController.endOfLevel = false;			//let the camera know that the level hasn't finished, and so we need to follow the player
	}

	public void Reset(){								//this is the set of actions to perform when we need to restart the game from the start of the level
		destroyableObjectList = FindObjectsOfType<ObjectDestroyer> ();	//get every object that has the script object destroyer attached to it7
		for (int i = 0; i < destroyableObjectList.Length; i++) {		//turn off each object that is of type Object Destroyer - meaning all the generated content.
			if (!destroyableObjectList [i].keepActive) {
				destroyableObjectList [i].gameObject.SetActive (false);
			}
		}
		//I NEED A WAY TO RESET THE BUFFS WHEN THE LEVEL RESTARTS (without reloading the scene).
		BuffPickup[] activeBuffs = Resources.FindObjectsOfTypeAll<BuffPickup> ();
		//Debug.Log ("Amount of active point buffs "+activeBuffs.Length);
		for (int i = 0; i < activeBuffs.Length; i++) {
			if (activeBuffs [i].IsInvoking()) {
				//Debug.Log ("a buff is still invoking");
				activeBuffs [i].Cancel();
			}
		}

		//string theOtixa = "dont tilt bro";

		thePlayer.transform.position = playerStartPoint; 				//reset the player starting point
		theMapGen.transform.position = sectionStartPoint;				//reset the section generator starting point           !!
		thePlayer.gameObject.SetActive (true);							//set the player to be enabled
		theScoreManager.scoreCounter = 0;								//set score back to 0 ready for a new game, 
		theScoreManager.scoringEnabled = true;							//re-enable scoring once the new game begin
		theScoreManager.spawnedEnd = false;								//reset this, allowing us to meet the winning criteria again
		Time.timeScale = 1f;											//reset the time to regular speed, incase player died during slow down
		powerupReset = true;											//this boolean ensures any powerups that were running upon death / game reset
		thePlayer.slowSpeedPoints = thePlayer.slowSpeedPointsMax;		//reset the slowdown points of the player
		if(!disableMapGen){	
			theMapGen.stopGeneration = false;								//reset the boolean that stops all generation, incase they died at end of level
			theMapGen.endOfLevel = false;									//reset boolean that stops the random platforms being generated, and starts the "end zone" generation
		}
		theCameraController.endOfLevel = false;							//reset the camera back to the regular mode where it follows the player
		theMapGen.ResetLevelExit();										//resets the level exit off screen out of reach of the player

		//thePowerups = Resources.FindObjectsOfTypeAll<Powerups> ();		//caution, this find method can look through every single thing - can be ineffective
		//for (int i = 0; i < thePowerups.Length; i++) {							//loop through
			//Debug.Log("I'm in the array of powerups");
		//	if (thePowerups [i].respawnOnReset) {								//check if it's respawnOnReset is true
		//		thePowerups [i].gameObject.SetActive (true);					//set it to be active
		//	}
		//}
		//theCoins = Resources.FindObjectsOfTypeAll<PickupPoints> ();	//caution, this find method can look through every single thing - can be ineffective
		//for (int i = 0; i < theCoins.Length; i++) {							//loop through
			//Debug.Log("I'm in the array of powerups");
		//	if (theCoins [i].respawnOnReset) {								//check if it's respawnOnReset is true
		//		theCoins [i].gameObject.SetActive (true);					//set it to be active
		//	}
		//}

		PartOfTheScenery[] theScenery = Resources.FindObjectsOfTypeAll<PartOfTheScenery> ();			//find everything of type PartOfTheScenery / tag of scenery
		for (int i = 0; i < theScenery.Length; i++) {									//loop through							
			theScenery [i].transform.position = theScenery [i].startPos; 				//reset it position
			theScenery [i].gameObject.SetActive(true);									//this is to handle non randomly generated pickups that need to be respawned
			//new Vector3(theScenery[i].startPos.x, theScenery[i].startPos.y, theScenery[i].startPos.z); 							
		}
		//reset their co-ordinates
		ThudBlock[] thudBlocks = Resources.FindObjectsOfTypeAll<ThudBlock> ();			//find everything of type thudblock
		for (int i = 0; i < thudBlocks.Length; i++) {									//loop through	
			//Debug.Log("IM MEANT TO SET DYNAMIC LIKE");
			thudBlocks [i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;		//reset the gravity
			//thudBlocks [i].GetComponent<Rigidbody2D>().gravityScale = 0;									//this is to handle non randomly generated pickups that need to be respawned
			//new Vector3(theScenery[i].startPos.x, theScenery[i].startPos.y, theScenery[i].startPos.z); 	
			//thudBlocks [i].GetComponent<Rigidbody2D>(). AddForce(Vector2.up * 20f, ForceMode2D.Impulse);
			thudBlocks [i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			thudBlocks [i].GetComponent<Collider2D> ().enabled = true;
		}
	}


	/*COMMENTARY: This is old code that was used before we used a death me		nu, which caused a delay before executing the code
	 *
	 * public IEnumerator RestartGameCo (){
		//the have died, so stop giving them points per second
		theScoreManager.scoringEnabled = false;
		//deactivate the player - this will stop most things, as map gen / camera are based off the player movement
		thePlayer.gameObject.SetActive (false);

		//cause a delay, so player has time to look at score and prepare to restart
		yield return new WaitForSeconds(0.5f);

		//get every object that has the script object destroyer attached to it
		destroyableObjectList = FindObjectsOfType<ObjectDestroyer> ();
		//turn off each object that is of type Object Destroyer - meaning all the generated content.
		for (int i = 0; i < destroyableObjectList.Length; i++) {
			destroyableObjectList[i].gameObject.SetActive (false);
		}

		//reset the player and the section generator starting points
		thePlayer.transform.position = playerStartPoint;
		sectionGenerator.position = sectionStartPoint;
		thePlayer.gameObject.SetActive (true);

		//set score back to 0 ready for a new game, and re-enable scoring once the new game begins.
		theScoreManager.scoreCounter = 0;
		theScoreManager.scoringEnabled = true;
	}*/

}
