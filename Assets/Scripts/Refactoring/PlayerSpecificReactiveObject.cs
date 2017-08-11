using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecificReactiveObject : DynamicEnvironmentObject {
    public string playerName;               //this is player that we want the dynamic object to react to

    //question: is this virtual incase subclasses want to further define the method?
    protected override bool CheckPlayerCollision(PlayerCollisionEvent other)
    {
        if (other.player.CompareTag("Player") && other.player.name == playerName)            //as collision2d is event, not a component, we must use gameobject.comparetag
        {
            return true;
        }
        return false;                       //won't reach here if it's true, as return exits the function
    }

}
