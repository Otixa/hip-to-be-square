using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicInteractiveObject : DynamicEnvironmentObject {
    protected override void OnEnable()
    {
        
    }


    //this type of object is not static and responds to interaction with the player (such as triggering a block to drop / pushing or pulling)
    protected override void OnPlayerCollision(Collider2D other)
    {
        TriggerPhysics(myRigidbody);
    }

    
}
