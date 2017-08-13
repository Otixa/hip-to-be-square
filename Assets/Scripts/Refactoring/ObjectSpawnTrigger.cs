using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnTrigger : PersistentPlayerTrigger {
    public GameObject objectToSpawn;                //this HAS to be done through inspector. Drag in the prefab you want to spawn.
    public float offsetX;                           //where to place this object in relation to the trigger on X axis
    public float offsetY;                           //where to place this object in relation to the trigger on X axis  

    public override void WhenTriggered(PlayerCollisionEvent other)
    {
        objectToSpawn.transform.position = new Vector3(transform.position.x + offsetX, transform.position.y + offsetY, transform.position.z);
        objectToSpawn.SetActive(true);
    }

    // Use this for initialization
    void Start () {
		//Implement way of finding the object here, but will be easier to drag it in via the UI to make this multipurpose (instead of just for exits).
	}
	
}
