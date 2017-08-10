using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]

abstract class EnvironmentObject : ResettableObject {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CheckPlayerCollision(other))
        {
            OnPlayerCollision(other); 
        }
    }

    void OnCollisionEnter2D(Collider2D other)
    {
        if (CheckPlayerCollision(other))
        {
            OnPlayerCollision(other);
        }
    }

    // this will be defined in all extensions of this class
    protected abstract void OnPlayerCollision(Collider2D other);        

    protected virtual bool CheckPlayerCollision(Collider2D other)
    {
        if(gameObject.tag == "Player")
        {
            return true;
        }
        return false;                       //won't reach here if it's true, as return exits the function
    }

}
