using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* This componenet is responsible for generating the level. For the sake of memory, it's
 * important to utilise object pools so that the same prefabs get reused once they are off
 * of the screen. It is responsible for random map generation from a preset of map sections.
 * Uses the mapGenerationPoint which will start each point at the end of the platfrom we last spawned */
public class MapSectionGenerator : MonoBehaviour {
	//public GameObject mapSection;			//this will store the prefab map section to place as map generates
	private Transform generationPoint;		//the point ahead of camera that we use to know if we need to add more Sections for the approaching player
	private int gap;						//how much to leave between platforms
	[Range(0,5)] public int minGap;			//the minimum bounds of the gap
	[Range(0,5)] public int maxGap;			//the upper bounds of the gap
	private int sectionSelector;			//variable for the random selection of map section to place in (used as an array index)
	//private float[] sectionWidths;		//an array that stores the widths of the different objects so our gap generation still works correctly.
	public ObjectPooler sectionPools;		//create an object pool that can house 1 or more prefabs of map sections
	private float minSpawnHeight;				//the highest y point a platform can be spawned (this is read from the platfrom object)
	private float maxSpawnHeight;				//the lowest y point a platform can be spawned (this is read from the platfrom object)
	public float maxHeightChange;			//the biggest the height change can be (- or +) between platforms
	private float newYCoord;				//variable to store the current height change between the last and new platform 
	private CoinGenerator theCoinGenerator;	//reference to the coin generator so we can tell a platform to spawn coins on it
	public ObjectPooler spikePool;			//reference to the spike pool in order to spawn spike objects
	public float randomSpikeChance;			//set up a frequency for how often spikes should spawn
	public float powerupHeight;				//how high above a platform should a powerup be spawned
	////public ObjectPooler powerupPool;		//reference to the powerup pool in order to spawn powerup objects
	public float powerupChance;				//set up a frequency for how often powerups should spawn
	[HideInInspector] public bool endOfLevel;					//boolean to state whether the game has reached the end of level phase, and to stop randomly generating the level
	[HideInInspector] public bool stopGeneration;				//boolean to stop any further generation of level once exit has been activated
	private float theWidth;										//temp variable to store the new platforms widths
    public float exitHeight = 3.5f;                                    //user passes in this value to specify how high the exit should be spawned


	void Start () {	
		generationPoint = GameObject.Find ("GenerationPoint").transform;		//get our reference to the generation point for future use
		theCoinGenerator = FindObjectOfType<CoinGenerator> ();					//get reference to the coin generator
		ResetLevelExit (); 														//sets coordinates of levelexit	

		//getting an array of all the lengths of objects to use. 		<<BETTER IF ALL OBJECTS HAVE CLASS THAT EXPRESSES WIDTH
//		sectionWidths = new float[sectionPools.pooledObjects.Count];		//create an array that's going to store the legnths of each of the map sections in the sectionPools array
//		for (int i = 0; i < sectionPools.pooledObjects.Count; i++) {		//a loop in order to set up our array of sections widths		
//			sectionWidths[i] = sectionPools[i].pooledObjects[0].GetComponent<BoxCollider2D>().size.x;		//we want the box collider of the objects in the pool, so have to access the .pooledObject
//		}
										
	}

	void Update () {
		/* The jist of this is you have a generation point and a generator. The generation point is set just ahead of the camera, and moves with it.
		 * the generator is used to place down platforms, so sits at the end of the last platform placed. If the generation point passes 
		 * the genrator (meaning the point ahead of camera passes the last platform made), we shift the platform generator ahead and place a 
		 * platform. This way, the platforms generate a few steps ahead of the player, so you never see them built but also we don't have to
		 * build the entire map before the player can start. */
		if (sectionPools != null) {			//incase we don't need map generation
			if (!endOfLevel) {													//if we haven't reached the amount of points to trigger the end of level section to spawn
				if (transform.position.x < generationPoint.position.x) {		//check if we need to create a platform yet (read above for more info)
					gap = Random.Range (minGap, maxGap+1);	//!!					//calc the gap between sections

					float currentPosY = transform.position.y;

					float minRangePlayer = currentPosY - maxHeightChange;
					float maxRangePlayer = currentPosY + maxHeightChange;
					float minRangePlatform = 0f;
					float maxRangePlatform = 0f;

					float bestCommonHigh = 0f;
					float bestCommonLow = 0f;

					float newPosY = 0f;

					GameObject newSection = null;

					bool foundObject = false;
					while(!foundObject) {
						newSection = sectionPools.GetRandomPooledObject (); 
						maxRangePlatform = newSection.GetComponent<ObjectSpawnBounds> ().sectionMaxHeight;	//set the sections max height bounds
						minRangePlatform = newSection.GetComponent<ObjectSpawnBounds> ().sectionMinHeight;
					
						bestCommonHigh = Mathf.Min (maxRangePlayer, maxRangePlatform);
						bestCommonLow = Mathf.Max (minRangePlayer, minRangePlatform);

						if (bestCommonHigh >= bestCommonLow) {
							newPosY = Random.Range (bestCommonLow, bestCommonHigh);
							foundObject = true;
						} else {
							Debug.Log ("No fair spawn points, pooling another object...");
						}
					}



					//setting the two bools to false ensures that we enter the loop once, and repeat if a non suitable map section is pooled
//					bool canJump = false;
//					bool canDrop = false;
//					float upperBounds = 0f;
//					float lowerBounds = 0f;
//					float currentY = 0f;
//					float jumpAmount = 0f;
//					float dropAmount = 0f;
//					GameObject newSection = null;
//
//					if (!canJump && !canDrop) {			//if you can't do either, you need to pool a new object
//						Debug.Log("Pooling new map section at Time "+Time.time);
//						//Get a platform
//						newSection = sectionPools.GetRandomPooledObject (); 
//						//get it's upper and lower bounds
//						upperBounds = newSection.GetComponent<ObjectSpawnBounds> ().sectionMaxHeight;	//set the sections max height bounds
//						lowerBounds = newSection.GetComponent<ObjectSpawnBounds> ().sectionMinHeight;
//						//transform.position.y is our "currentY"
//						currentY = transform.position.y;
//						//maxHeightChange is our jumpAmount
//						jumpAmount = maxHeightChange;
//						dropAmount = jumpAmount;
//
//						if (
//							((currentY + jumpAmount) >= lowerBounds) &&
//							((currentY + jumpAmount) <= upperBounds)) {
//							canJump = true;
//						}
//						if (
//							((currentY - dropAmount) <= upperBounds) &&
//							((currentY - dropAmount) <= lowerBounds)) {
//							canDrop = true;
//						}
//					}
//
//					float newY;
//
//					if (canJump && canDrop) {
//						newY = Random.Range (lowerBounds, upperBounds);
//					} else if (canJump && (currentY > lowerBounds)) {
//						newY = Random.Range (currentY - dropAmount, upperBounds);
//					} else if (canJump && (currentY < lowerBounds)) {
//						newY = Random.Range (lowerBounds, currentY + jumpAmount);
//					} else if (canDrop && (currentY < upperBounds)) {
//						newY = Random.Range (lowerBounds, currentY + jumpAmount);
//					} else if (canDrop && (currentY > upperBounds)) {
//						newY = Random.Range (currentY - dropAmount, upperBounds);
//					} else {
//						
//						Debug.Log ("CanJump: " + canJump +
//						", CanDrop: " + canDrop +
//						", CurrentY: " + currentY +
//						", LowerBounds: " + lowerBounds +
//						", UpperBounds: " + upperBounds +
//						", JumpAmount: "+ jumpAmount +
//						", Section Name: " + newSection.name);
//						newY = 0f;
//						Debug.LogError ("Not one of the newY formulas matched, setting Y to 0");
//					}
//
//					Debug.Log ( "Here are the new platform details: ENJOY!!! <3");
//					Debug.Log ("CanJump: " + canJump +
//						", CanDrop: " + canDrop +
//						", CurrentY: " + currentY +
//						", LowerBounds: " + lowerBounds +
//						", UpperBounds: " + upperBounds +
//						", Section Name: " + newSection.name+
//						", New Y: "+newY);
					
					theWidth = newSection.GetComponent<ObjectSpawnBounds> ().width;

					//POWERUPS >> Chance to spawn
					//as we want powerups to spawn between platforms, we want to do the code at this point before the platform generator is moved on. 
					if (Random.Range (0f, 100f) < powerupChance) {				//decide whether to spawn a powerup or not based on the chance variable
						PowerupGenerator.Instance.CreateRandomAt
						(transform.position + new Vector3 (gap / 2f, powerupHeight, 0f));	//get a powerup from the pool
					}

					//our generator lives on the END of the last generated section. So we need to move on the gap amount, then half the way into the next generated section (as platform origin is in it's center).
					//At the end of the update method, we move along another half, so we are sitting back at the end of the platform ready for the next update call.
					transform.position = new Vector3 (transform.position.x + gap + (theWidth / 2) , newPosY, transform.position.z); 
					newSection.transform.position = transform.position;			//set the new sections co-ordinates to be equal to that of the Map Section Generator
					newSection.transform.rotation = transform.rotation;			//think this is needed, despite nothing rotating in the game.
					newSection.SetActive (true);                                //BRING IT TO LIFE!

                    ////COINS >> Chance to spawn
                    /// Now we are in center of our new platform, we will spawn coins. See spawnCoinGroup for spawn details
                    if (newSection.GetComponent<ObjectSpawnBounds>().canSpawnCoins)
                    {
                        //IMPROVEMENT: Increase Y co-ordinate if the newSection is one of Chunks thud components!
                        theCoinGenerator.spawnCoinGroup(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), theWidth);
                    }

                    //SPIKES >> Chance to spawn
                    //Before we move on our generation point, we will spawn spikes if the object can spawn spikes (property of platform) and based on it's spawn chance
                    if ((Random.Range (0f, 100f) < randomSpikeChance) && newSection.GetComponent<ObjectSpawnBounds> ().canSpawnSpikes) {
						GameObject newSpike = spikePool.Get (obj => obj.name.Contains ("Spike"));
						//this is creating a vector to add to our existing transform. It chooses a random x point on the platform, exlcuding the edges of platforms for fairness. 1.2 is suitable height for spike in relation to platform
						Vector3 spikePosition = new Vector3 ((Random.Range (-(theWidth / 2) + 2, (theWidth / 2) - 2)), 1.2f, 0f);	//-2 and +2 here are an offset to allow non spiked edges to platforms			
						newSpike.transform.position = transform.position + spikePosition;	//will combine the 2 vector3ds to get new co-ordinates
						newSpike.transform.rotation = transform.rotation;
						newSpike.SetActive (true);
					}

					//the y offset is used to take into account if the current platform's exit y co-ordinate, as it's important we know where the player will jumping from.
					float yOffset = newSection.GetComponent<ObjectSpawnBounds> ().endPointOffSet;	//if the last piece end point was lower than the entry point, adjust the y transform too
					//update the section generators end position, ready for the next time it calls. It sits at the exit point of the last section that was generated.
					transform.position = new Vector3 (transform.position.x + (theWidth / 2), transform.position.y + yOffset, transform.position.z); 

					//variable for how many different types we have - BUT we will have a random pooled object function to do this bit
					//sectionSelector = Random.Range (0, sectionPools.Length);	//choose a random section to place in
				//OPTION INEFFECTIVE BUT SHOULD WORK
					//get a random object

					//we want to check its min, max, width, offset

					//choose a random number between characterJumpLimit - / +

					//see if that number is within the bounds of the min and max spawns heights of the generated platfrom

					//if it is, we carry on, if it isn't, we go back to step 1.

				//OPTION SLIGHT BETTER AND STILL WORKS
					//get a random object

					//we want to check its min, max, width, offset
					//----------- min = 2, max = 8, ourcurrenty = 1, jumpLimit = 1
					//check if character jump limit +/- the last platfrom y end point is within the min and max of the new platform
					//----------- is our current Y+jump limit >= the min && currentY+jump < the max == WE CAN REACH JUMP UP TO IT. 
					//----------- ELSE check if we could spawn below
					//----------- if our currentY 
					//if neither are, we choose a new one object and go back to step 1

					//if one of them is within range, we 
					//if it is, we carry on, if it isn't, we go back to step 1.

					//GameObject newSection = sectionPools [sectionSelector].GetPooledObject ();	//call the method that looks in our pool, and gets the first inactive one
					//GameObject newSection = sectionPools.GetRandomPooledObject ();
					//theWidth = newSection.GetComponent<ObjectSpawnBounds> ().width;
					//maxSpawnHeight = newSection.GetComponent<ObjectSpawnBounds> ().sectionMaxHeight;	//set the sections max height bounds
					//minSpawnHeight = newSection.GetComponent<ObjectSpawnBounds> ().sectionMinHeight;	//set the sections min height bounds

					//LAST SECTION FINISHED ON 7 of 2. RANDOM NUMBER BETWEEN -1 and 1; 0.5 chosen. NewYCords = 2.5
					// the change on Y axis for next platform = (the last platforms y co-ordiante) + random number between -max change and +max change
					//newYCoord = (transform.position.y) + Random.Range (-maxHeightChange, maxHeightChange);	//store the new y co-ordinate to place the next platform

					//CHOSEN PLATFROM HAS MIN OF 5 MAX OF 9. (2, 5, 9). So would get clamped at 5. SOMEHOW we now need to jump from 2 up 5, even though max height 1.
					// change height change if the y co ord is too high or low for the chosen platform
					//newYCoord = Mathf.Clamp (newYCoord, minSpawnHeight, maxSpawnHeight);							//now need to ensure it fits within our top and bottom y co-ords. Platform bounds can vary per platform.


//					//our generator lives on the END of the last generated section. So we need to move on the gap amount, then half the way into the next generated section (as platform origin is in it's center).
//					//At the end of the update method, we move along another half, so we are sitting back at the end of the platform ready for the next update call.
//					transform.position = new Vector3 (transform.position.x + (theWidth / 2) + gap, newYCoord, transform.position.z); 
//					newSection.transform.position = transform.position;			//set the new sections co-ordinates to be equal to that of the Map Section Generator
//					newSection.transform.rotation = transform.rotation;			//think this is needed, despite nothing rotating in the game.
//					newSection.SetActive (true);								//BRING IT TO LIFE!

				}
			
			} else {							//if player score has hit threshold that will stop random generation and start the home straight where the exit will spawn
				if (!stopGeneration) {			//this boolean ensures the following code is executed only once, as it is set to be true within this if branch.		
					for (int i = 0; i < 10; i++) {											//load the platforms leading to the exit - "the home straight" of 10 x 5 wide platforms
						GameObject finalSection = sectionPools.Get (obj => obj.name.Contains ("Platform5"));
						finalSection.transform.position = transform.position + new Vector3 ((finalSection.GetComponent<ObjectSpawnBounds> ().width / 2), 0, 0);
						finalSection.transform.rotation = transform.rotation;
						finalSection.SetActive (true);
						float yOffset = finalSection.GetComponent<ObjectSpawnBounds> ().endPointOffSet;		//if home straight is made of platforms that end on different y axis to their start point
						transform.position = new Vector3 (transform.position.x + (finalSection.GetComponent<ObjectSpawnBounds> ().width / 2), transform.position.y + yOffset, transform.position.z); 															
					}
					GameObject theExit = GameObject.Find ("LevelExit");										//get a reference to the level exit object and move it to the end of the "Home straight".
					theExit.transform.position = new Vector3 (transform.position.x, transform.position.y + exitHeight, transform.position.z);
					theExit.transform.rotation = transform.rotation;
					theExit.SetActive (true);
					stopGeneration = true;																	//ensures we only generate the home straight once
				}
			}
		}
	}

	public void ResetLevelExit(){
		GameObject theExit = GameObject.Find("LevelExit");			//get a reference to the game object that acts as the level exit
		theExit.transform.position = new Vector3 (-100, 0f, 0f);		//set exit offscreen (as it needs to remain active in order to find it later)
		theExit.transform.rotation = transform.rotation;			//have to set rotation whenever you set new object, I think?
		theExit.SetActive (true);
	}
}
