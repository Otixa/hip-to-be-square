using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerCollisionEvent
{
    public GameObject player;
    public Collision2D collisionInfo;
}

[RequireComponent(typeof(Collider2D))]
public abstract class EnvironmentObject : ResettableObject {   
    //TRIGGERS & COLLIDER2D
    void OnTriggerEnter2D(Collider2D other)
    {
        var playerCollisionEvent = new PlayerCollisionEvent();
        playerCollisionEvent.player = other.gameObject;
        playerCollisionEvent.collisionInfo = null;
        if (CheckPlayerCollision(playerCollisionEvent))
        {
            OnPlayerCollision(playerCollisionEvent); 
        }
    }

    //COLLISION & COLLISION2D
    void OnCollisionEnter2D(Collision2D other)
    {
        var playerCollisionEvent = new PlayerCollisionEvent();
        playerCollisionEvent.player = other.gameObject;
        playerCollisionEvent.collisionInfo = other;

        if (CheckPlayerCollision(playerCollisionEvent))
        {
            OnPlayerCollision(playerCollisionEvent);
        }
    }
   
    //this is for the triggers
    protected abstract void OnPlayerCollision(PlayerCollisionEvent other);

    //question: is this virtual incase subclasses want to further define the method?
    protected virtual bool CheckPlayerCollision(PlayerCollisionEvent other)
    {
        if (other.player.CompareTag("Player"))            //as collision2d is event, not a component, we must use gameobject.comparetag
        {
            return true;
        }
        return false;                       //won't reach here if it's true, as return exits the function
    }

 

}
