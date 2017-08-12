using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpecificReactiveObject : DynamicEnvironmentObject {
    public string playerName;               //this is player that we want the dynamic object to react to

    //question: this overrides the behaviour in Environmental Object, which is the if statement condition to whether OnPlayerCollision() is called. 
    protected override bool CheckPlayerCollision(PlayerCollisionEvent other)
    {
        if (other.player.CompareTag("Player") && other.player.name == playerName)          
        {
            return true;
        }
        return false;                      
    }

    
    //we don't need this at present, because through using WakeUp() to apply the physics, we no longer to adjust the rigidbody settings upon collision
    private void OnPlayerSpecificAction(PlayerCollisionEvent other)
    {

    }

}
