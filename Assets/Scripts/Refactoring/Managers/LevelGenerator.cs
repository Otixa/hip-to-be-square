using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* To do list:
        * Make sure level exit implements ResettableObject so it resets it's position offscreen when the game is reset / player dies
        * */

public class LevelGenerator : MonoBehaviour {
    private Transform generationTriggerPoint;                   //the point ahead of camera that we use to know if we need to add more Sections for the approaching player                         
    public ObjectPooler sectionPools;                           //create an object pool that can house 1 or more prefabs of map sections
    public ObjectPooler coinPool;                               //this will be fed into the script prior to execution
    public ObjectPooler spikePool;                              //reference to the spike pool in order to spawn spike objects

    private int gapBetweenPlatforms;                            //how much to leave between the last platform and the newly generated platform
    [Range(0, 5)] public int minGapBetweenPlatform;             //the minimum bounds of the gap
    [Range(0, 5)] public int maxGapBetweenPlatform;             //the upper bounds of the gap
    private float minPlatformSpawnHeight;                       //the highest y point a platform can be spawned (this is read from the platfrom object)
    private float maxPlatformSpawnHeight;                       //the lowest y point a platform can be spawned (this is read from the platfrom object)
    public float maxPlatformYChange;                            //the biggest the Y (height) change can be (- or +) between platforms
    private float newPlatformYChange;                           //variable to store the current Y (height) change between the last and new platform 

    public float spikeSpawnChance;                              //set up a frequency for how often spikes should spawn
    public float powerupSpawnChance;                            //set up a frequency for how often powerups should spawn
    public float powerupHeight;                                 //how high above a platform should a powerup be spawned

    private Vector3 levelGeneratorStartPos;                     //store starting co-ords in order to restart game
    private bool endOfLevel;                                    //boolean to state whether the game has reached the end of level phase, and to stop randomly generating the level
    //private bool haveSpawnedExit;                               //boolean to stop any further generation of level once exit has been activated (DONT NEED CAUSE HANDLED WITH EVENT NOW)
    public float exitSpawnHeight = 3.5f;                        //user passes in this value to specify how high the exit should be spawned
    public bool randomLevelGenerationEnabled = true;            //flag to turn off when you don't want this map generator to create generated content
    
    void Start () {
        generationTriggerPoint = GameObject.Find("GenerationTriggerPoint").transform;          //get our reference to the generation point for future use
        levelGeneratorStartPos = transform.position;
    }

    private void Awake()
    {
        GameManager2.OnLevelFinish += GenerateEndOfLevel;
        GameManager2.OnGameReset += ResetLevel;
    }

    // Update is called once per frame
    void Update () {
        /* The jist of this is you have a generationTriggerPoint and a generator. The generationTriggerpoint is set just ahead of the camera, and moves with it.
		 * the generator is used to place down platforms, so sits at the end of the last platform placed. If the generationTriggerPoint passes 
		 * the level genrator (meaning the point ahead of camera passes the last platform created), we shift the platform generator ahead and place a 
		 * platform. This way, the platforms generate a few steps ahead of the player, so you never see them built but also we don't have to
		 * build the entire map before the player can start. */
        if (sectionPools != null && randomLevelGenerationEnabled)                                               //incase we don't need map generation, sectionPool would be empty...!
        {           
            if (!endOfLevel)
            {                                                                   //if we haven't reached the amount of points to trigger the end of level section to spawn
                if (transform.position.x < generationTriggerPoint.position.x)
                {                                                               //check if we need to create a platform yet (read above for more info) 
                    //POWERUPS >> Chance to spawn
                    //as we want powerups to spawn between platforms, we want to do the code at this point before the platform generator is moved on. 
                    gapBetweenPlatforms = UnityEngine.Random.Range(minGapBetweenPlatform, maxGapBetweenPlatform + 1); 
                    if (UnityEngine.Random.Range(0f, 100f) < powerupSpawnChance)
                    {
                        spawnRandomPowerup(transform.position + new Vector3(gapBetweenPlatforms / 2f, powerupHeight, 0f));
                    }                

                    /* define local variables needed in the algorithm for generating and placing a valid platform section prefab */
                    float currentPosY = transform.position.y;                       //the y co-ordinate of the exit point of the last generated platform          
                    float minRangePlayer = currentPosY - maxPlatformYChange;        //the highest Y coord on Y axis the player can reach
                    float maxRangePlayer = currentPosY + maxPlatformYChange;        //the lowest Y coord on Y axis the player can reach
                    float minRangePlatform = 0f;                                    //the highest Y coord on which the new platform can safely spawn
                    float maxRangePlatform = 0f;                                    //the lowerst Y coord on which the new platform can safely spawn
                    float bestCommonHigh = 0f;                                      //the highest Y coord that suits both player and platform
                    float bestCommonLow = 0f;                                       //the lowest Y coord that suits both player and platform
                    float newPosY = 0f;                                             //the new Y coordinate for the level generator to use to spawn the new platform
                    GameObject newSection = null;                                   //variable to be used to store the pooled platform prefab
                    bool foundObject = false;                                       //boolean used to continue pooling platform prefabs until we found one that can spawn in a valid position

                    while (!foundObject)
                    {
                        newSection = sectionPools.GetRandomPooledObject();                                  //pool a prefab from the pool
                        maxRangePlatform = newSection.GetComponent<ObjectSpawnBounds>().sectionMaxHeight;   //get hold of its highest valid spawn point
                        minRangePlatform = newSection.GetComponent<ObjectSpawnBounds>().sectionMinHeight;   //get hold of its lowest valid spawn point

                        bestCommonHigh = Mathf.Min(maxRangePlayer, maxRangePlatform);                       //get the minimum out of the two highest possible Y co-ords between player and platform
                        bestCommonLow = Mathf.Max(minRangePlayer, minRangePlatform);                        //get the maximum out of the two lowest possible Y co-ords between player and platform

                        if (bestCommonHigh >= bestCommonLow)            //if the highest isn't greater than the lowest, it means there were no common Y co-ordinates that suit both platform and player
                        {
                            newPosY = UnityEngine.Random.Range(bestCommonLow, bestCommonHigh);                          //choose a random position between the lowest and highest valid spawn points
                            foundObject = true;                                                             //set escape flag
                        }
                        else
                        {
                            Debug.Log("No fair spawn points, pooling another object...");   //due to last platform position and pooled platform parameters, there are no valid places to spawn this platform
                        }
                    }
                    //MOVE GENERATOR INTO SPAWN POINT OF THE NEW PLATFORM
                    //our generator lives on the END of the last generated section. So we need to move on the gap amount, then half the way into the next generated section (as platform origin
                    // is in it's center). At the end of the update method, we move along another half, so we are sitting back at the end of the platform ready for the next update call.
                    float theWidth = newSection.GetComponent<ObjectSpawnBounds>().width;    //variable to store the new platforms width thats used to calc center point when spawning platform/coins etc
                    transform.position = new Vector3(transform.position.x + gapBetweenPlatforms + (theWidth / 2), newPosY, transform.position.z);
                    newSection.transform.position = transform.position;         //set the new sections co-ordinates to be equal to that of the Map Section Generator
                    newSection.transform.rotation = transform.rotation;         //think this is needed, despite nothing rotating in the game.
                    newSection.SetActive(true);                                //BRING IT TO LIFE!

                    // COINS >> Chance to spawn
                    // Now we are in center of our new platform, we will spawn coins. See spawnCoinGroup for spawn details
                    if (newSection.GetComponent<ObjectSpawnBounds>().canSpawnCoins)
                    {
                        //IMPROVEMENT: Increase Y co-ordinate if the newSection is one of Chunks thud components!
                        spawnCoins(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), theWidth);
                    }

                    // SPIKES >> Chance to spawn
                    // Before we move on our generation point, we will spawn spikes if the object can spawn spikes (property of platform) and based on it's spawn chance
                    if ((UnityEngine.Random.Range(0f, 100f) < spikeSpawnChance) && newSection.GetComponent<ObjectSpawnBounds>().canSpawnSpikes)
                    {
                        GameObject newSpike = spikePool.Get(obj => obj.name.Contains("Spike"));
                        //this is creating a vector to add to our existing transform. It chooses a random x point on the platform, exlcuding the edges of platforms for fairness. 1.2 is suitable height for spike in relation to platform
                        Vector3 spikePosition = new Vector3((UnityEngine.Random.Range(-(theWidth / 2) + 2, (theWidth / 2) - 2)), 1.2f, 0f); //-2 and +2 here are an offset to allow non spiked edges to platforms			
                        newSpike.transform.position = transform.position + spikePosition;   //will combine the 2 vector3ds to get new co-ordinates
                        newSpike.transform.rotation = transform.rotation;
                        newSpike.SetActive(true);
                    }

                    // MOVE ON GENERATOR >> update the section generators end position, ready for the next time it calls. It sits at the exit point of the last section that was generated.
                    //the y offset is used to take into account if the current platform's exit y co-ordinate, as it's important we know where the player will jumping from when generating the next platform
                    float yOffset = newSection.GetComponent<ObjectSpawnBounds>().endPointOffSet;                                                                                                 
                    transform.position = new Vector3(transform.position.x + (theWidth / 2), transform.position.y + yOffset, transform.position.z);                    
                }
            }
        }
    }

    // WIN CONDITION >> if player score has hit threshold that will stop random generation and start the home straight where the exit will spawn
    private void GenerateEndOfLevel()
    {
        endOfLevel = true;
            // SPAWN HOME STRAIGHT >> load the platforms leading to the exit - "the home straight" of 10 x 5 wide platforms
            for (int i = 0; i < 10; i++)
            {
                GameObject finalSection = sectionPools.Get(obj => obj.name.Contains("Platform5"));
                finalSection.transform.position = transform.position + new Vector3((finalSection.GetComponent<ObjectSpawnBounds>().width / 2), 0, 0);
                finalSection.transform.rotation = transform.rotation;
                finalSection.SetActive(true);
                transform.position = new Vector3(transform.position.x + (finalSection.GetComponent<ObjectSpawnBounds>().width / 2), transform.position.y, transform.position.z);
            }
            // SPAWN EXIT
            GameObject theExit = GameObject.Find("LevelExit");                                      //get a reference to the level exit object and move it to the end of the "Home straight".
            theExit.transform.position = new Vector3(transform.position.x, transform.position.y + exitSpawnHeight, transform.position.z);
            theExit.transform.rotation = transform.rotation;
            theExit.SetActive(true);
            //haveSpawnedExit = true;                                                                  //ensures we only generate the home straight once
    }

    public void ResetLevel()
    {
        if (randomLevelGenerationEnabled)                   //in order to avoid unnecessary computation on tutorial levels
        {
            transform.position = levelGeneratorStartPos;
            //haveSpawnedExit = false;
            endOfLevel = false;

            //loop to remove any generated platforms/coins/pickups/spikes, ready for a new game to begin
            GeneratedContent[] activeGeneratedContent = FindObjectsOfType<GeneratedContent>();
            for (int i = 0; i < activeGeneratedContent.Length; i++)
            {
                activeGeneratedContent[i].gameObject.SetActive(false);
            }
        } 
    }

    private void spawnRandomPowerup(Vector3 where)
    {            
        PowerupGenerator.Instance.CreateRandomAt(where);    //get a powerup from the pool
    }

    /*COMMENTARY: This was more complicated than planned, because a platfrom of length 7 can actually fit 
	* 8 coins along it's length, as the coins spawn on the grid lines, rather than within the grid cells.*/
    public void spawnCoins(Vector3 spawnPosition, float width)
    {
        int amountOfCoins = (int)UnityEngine.Random.Range(1, width + 1);                    //choose a random amount of coins from 1 to width+1
        int gapFromEdge = 1;                                                                //set the gap on either side of platform you want before coins are spawned
        float gapBetweenCoins;                      //when placing coins, this variable can set how much of a gap to place                                        
        if (amountOfCoins == 1)                     //there is no gap between 1 coin, so instead we just spawn the coin in the center of the platform        
        {                                           
            GameObject theCoin = coinPool.Get(obj => obj.GetComponent<PointPickup>() != null);
            theCoin.transform.position = spawnPosition;
            theCoin.SetActive(true);
        }
        else
        {
            float currentXoffset = 0;            //this variable keeps track of the x-cordinate as we loop and place amountOfCoins along the platform
            gapBetweenCoins = (width - (2 * gapFromEdge)) / (amountOfCoins - 1);    //calculate an even amount of gap to put between each of the coins
            spawnPosition = spawnPosition - new Vector3(width / 2, 0f, 0f);         //go back to start of platform
            spawnPosition = spawnPosition + new Vector3(gapFromEdge, 0f, 0f);       //add the gapFromEdge to inset the spawnPosition;
            for (int i = 0; i < amountOfCoins; i++)                                 //loop that many times, create a coin, place it and move along
            {                           
                GameObject theCoin = coinPool.Get(obj => obj.GetComponent<PointPickup>() != null);
                theCoin.transform.position = spawnPosition + new Vector3(currentXoffset, 0f, 0f);
                theCoin.SetActive(true);
                currentXoffset += gapBetweenCoins;                                  //update our current X offset so the next coin is placed gapBetweenCoins ahead of this one
            }
        }
    }
}