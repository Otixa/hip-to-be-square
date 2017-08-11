using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : EnvironmentObject {
    //We want this one - as spikes will have Triggers.
    protected override void OnPlayerCollision(PlayerCollisionEvent other)
    {
        throw new NotImplementedException();
        //trigger death
        //contact game manager
        //
    }

}
