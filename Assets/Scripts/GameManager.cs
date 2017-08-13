using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* This component will connect with various other classes to control the flow of the game.
 * this includes initialisation of the game, pausing, restarting, keeping scores. */
public class GameManager : MonoBehaviour {
	
	public GenericPlayer player;					    //as we will be setting the player position at the start of game, we need his controller
	private ObjectDestroyer[] destroyableObjectList;	//create this so that we can get a reference to all destroyable objects, in order to remove them when restarting the game
	private ScoreManager scoreManager;				//we need a reference to the score manager, so that when a player is killed we can stop the scoring!
	public DeathMenu deathMenu;						//we need a reference to the death menu, so that we can turn it on when player dies
	private MapSectionGenerator mapGen;				//reference needed so we can stop generating map when we accumulate enough points

    private Vector3 playerStartPoint;					//we need to store the start point of player so the player restarts consistently            !! player resets himself due to resettableObject
    private Vector3 sectionStartPoint;					//we need to store where the section generator actually started                             !! this is likely still needed, as isn't part of resettable
    public bool disableMapGen;                          //A switch to turn off the map generator in levels where you want to use a predefined level

    //private Powerups[] thePowerups;						//an array to check all powerups and find those which should reactivate upon game restart   !! resettableObject may fix this
	private PickupPoints[] theCoins;					//an array to check all powerups and find those which should reactivate upon game restart   !! resettableObject may fix this

	void Start () {
		scoreManager = FindObjectOfType<ScoreManager> ();            //!! will try track score within the game manager on next refactor
		mapGen = FindObjectOfType<MapSectionGenerator>();			 
        player = FindObjectOfType<GenericPlayer>();
        sectionStartPoint = mapGen.transform.position;				//keep note of the start position of section generator so that we can reset back to start position  !!set this in inspector 
		playerStartPoint = player.transform.position;               //keep note of the start position of the player so that we can reset back to start position         !!resettable obj may fix this
        
        if (disableMapGen) {										//if you do not want map generation (such as on tutorial levels)
			mapGen.stopGeneration = true;							//disable variables that allow the map generation with the MapSectionGenerator script
			mapGen.endOfLevel = true;                               //!!prefer if we weren't changing fields in another class here
        }
	}

    //!! when refactor, handle the scoring in here
	void Update () { }

    //restart simply calls the death menu, then the death menu will call the Reset function... probbaly can get rid of this.
	public void RestartGame(){
		scoreManager.scoringEnabled = false;			//the have died, so stop giving them points per second                  !!if distance travelled, then won't need this
		player.gameObject.SetActive (false);			//deactivate the player - this will stop most things, as map gen / camera are based off the player movement  !!question if we need this
		deathMenu.gameObject.SetActive(true);		    //activate the death screen                                             !! events will stop the need for this being changed here
		if(!disableMapGen){								//this is to allow us to disable map generation on some levels          !! Map generator should have a reset() that handles it's own shit
			mapGen.stopGeneration = false;				//tell the mapGenerator we need any sections (including the final section) to be spawned
			mapGen.endOfLevel = false;					//tell the mapGenerator that it should be generating random sections
		}
	}

	public void Reset(){                                //this is the set of actions to perform when we need to restart the game from the start of the level            !! this will stay.

        //When the game resets, we need to disable any generated content so that the level is empty for the next attempt
        destroyableObjectList = FindObjectsOfType<ObjectDestroyer> ();	//all generated content will have this attached to it. Maybe rename the class "generatedContent"
		for (int i = 0; i < destroyableObjectList.Length; i++) {		//turn off each object that is of type Object Destroyer - meaning all the generated content.
			if (!destroyableObjectList [i].keepActive) {                //Exclude those flagged as wanted to be kept active (!! Don't think we should do this, as should be a destroyableObject if we want to keep it acive?)
				destroyableObjectList [i].gameObject.SetActive (false); //disable them
			}
		}

		//A working method to deactivate buffs. problem is, there will only be one active buff so this is inefficient          !!There is a static BuffPickup item that holds reference to the active buff
		BuffPickup[] activeBuffs = Resources.FindObjectsOfTypeAll<BuffPickup> ();                                               //maybe we just need to access that / have the buff listen for events and call                                   //the cancel() function on reset? Reset event would be desirable. 
		for (int i = 0; i < activeBuffs.Length; i++) {
			if (activeBuffs [i].IsInvoking()) {
				activeBuffs [i].Cancel();
			}
		}

		player.transform.position = playerStartPoint; 				//reset the player starting point                       !!this should be handled by the Reset function of resettableObjects
		mapGen.transform.position = sectionStartPoint;				//reset the section generator starting point            
		player.gameObject.SetActive (true);							//set the player to be enabled                          !!don't think we need this if we get rid of disabling player on death. 
		scoreManager.scoreCounter = 0;								//set score back to 0 ready for a new game,             
		scoreManager.scoringEnabled = true;                         //re-enable scoring once the new game begin             !!if distance travelled, then won't need this
        scoreManager.spawnedEnd = false;							//reset this, allowing us to meet the winning criteria again
		Time.timeScale = 1f;										//reset the time to regular speed, incase player died during slow down      !!this should happen upon buff resetting? 
		player.playerStats.focus = player.maxFocus;                 //reset the slowdown points of the player

        if (!disableMapGen){	
			mapGen.stopGeneration = false;								//reset the boolean that stops all generation, incase they died at end of level
			mapGen.endOfLevel = false;									//reset boolean that stops the random platforms being generated, and starts the "end zone" generation
		}

		mapGen.ResetLevelExit();                                        //this method should be called from within the level generator, perhaps reacting to an event

        //!!logically - if we don't want something to be removed, we don't attack the destroyableObject script to it? Right!? maybe add non-destroyable prefabs for ease of start level creation?
        PartOfTheScenery[] theScenery = Resources.FindObjectsOfTypeAll<PartOfTheScenery> ();			//find everything of type PartOfTheScenery / tag of scenery
		for (int i = 0; i < theScenery.Length; i++) {									//loop through							
			theScenery [i].transform.position = theScenery [i].startPos; 				//reset it position
			theScenery [i].gameObject.SetActive(true);									//this is to handle non randomly generated pickups that need to be respawned							
		}

        //!!this should now all be handled by the ResettableObject settings, and the Sleep/Wakeup rigidbody settings. Velocity could be achieved by increasing the mass of the object.
		ThudBlock[] thudBlocks = Resources.FindObjectsOfTypeAll<ThudBlock> ();			            //find everything of type thudblock
		for (int i = 0; i < thudBlocks.Length; i++) {									            //loop through	
			thudBlocks [i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;		//reset the physics-based settings
			thudBlocks [i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			thudBlocks [i].GetComponent<Collider2D> ().enabled = true;
		}
	}
}
