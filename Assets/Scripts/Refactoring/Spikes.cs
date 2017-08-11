using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : EnvironmentObject {
    //We want this one - as spikes will have Triggers.
    protected override void OnPlayerCollision(Collider2D other)
    {
        //trigger death
        //contact game manager
        //
    }

    //Do we just do nothing here? 
    protected override void OnPlayerCollision(Collision2D other)
    {
        throw new NotImplementedException();
    }

}
