using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This component acts as a list of fields / parameters for the map section objects, that affect their behaviour
 * when generating the map dynamically. */
public class ObjectSpawnBounds : MonoBehaviour {
	/* These set the bounds on where a section can spawn when 
     * , ensuring it isn't too low or high.
	 * This is important because some sections require a drop for example, so can't be too low. */
	public float sectionMaxHeight = 8;	//8 leaves one space to run at top of screen, so the default max.
	public float sectionMinHeight = 0;	//0 is spawned in the lava, the literal lowest point you can run.
	/* This is to handle sections the finish on a different y position compared to where they started.
	 * as a result, the next platform spawned should still be reachable based on the variable max height change */
	public float endPointOffSet = 0;
	//variable to say whether or not a platform can get coins spawned on it in a healthy manner (spawn coin method only works on straight sections at present)
	public bool canSpawnCoins = false;
	//variable to say whether or not a platform can have spikes spawned on it (as spikes on some platforms would be unfair)
	public bool canSpawnSpikes = false;

	public float width = 8;

	//NEED TO IMPLEMENT <<
	//public bool isDestroyable = true;
}