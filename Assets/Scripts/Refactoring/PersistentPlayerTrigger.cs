using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentPlayerTrigger : EnvironmentObject {

    protected override void OnPlayerCollision(PlayerCollisionEvent other)
    {
        WhenTriggered(other);        
    }

    public abstract void WhenTriggered(PlayerCollisionEvent other);             //does this need to be public?

    //protected override void OnReset()                     //
    //{
        

    //}
}
