using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class will be the base class for objects that the player can collect, which will have some form of
 * effect on the game state. This includes changing points modifiers, awarding poitns and restoring 
 * focus points. This will be extended by other sub-classes */

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : EnvironmentObject {

    protected override void OnPlayerCollision(PlayerCollisionEvent other)
    {
        OnPickup(other);
        gameObject.SetActive(false);
    }

	public abstract void OnPickup (PlayerCollisionEvent other);
}
