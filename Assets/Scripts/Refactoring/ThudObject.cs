using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThudObject : DynamicInteractiveObject {

    //on collision with player, set the body type to dynamic (from it's original state of kinematic). We also add downwards velocity to the rigid body
    //we may need to also disbale the collider, as it may allow the player to jump off of a falling block (which isn't the intention)
    protected override void TriggerPhysics(Rigidbody2D rigidBody)
    {
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        myRigidbody.AddForce(Vector2.down * 20f, ForceMode2D.Impulse);
        myRigidbody.gameObject.GetComponent<Collider2D>().enabled = false;
    }

    //when object is re-enabled (reset), we need to reset the object to be kinetic, and remove any downwards velocity we had set. If we have disabled the collider, this should be enabled too. 
    protected override void OnReset()
    {
        myRigidbody.bodyType = RigidbodyType2D.Kinematic;
        myRigidbody.velocity = Vector2.zero;
        myRigidbody.gameObject.GetComponent<Collider2D>().enabled = true;

    }
}
