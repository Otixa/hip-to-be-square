using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupGenerator : Singleton<PowerupGenerator>{
	public ObjectPooler powerupPool;
	private PowerupGenerator (){
	}

	//call this from the map generator, passing it the map generation point as the vector 3d
	public Pickup CreateRandomAt(Vector3 thePosition){
		GameObject thePowerup = powerupPool.GetRandomPooledObject ();		//choose a random pickup: PointBuff, FocusBuff, FocusPickup
		thePowerup.transform.position = thePosition;						//set it coordinates
		thePowerup.SetActive(true);											//set it active
		return thePowerup.GetComponent<Pickup>();			//CURRENTLY UNUSED BUT MAY BE USEFUL
	}
}
