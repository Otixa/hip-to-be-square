using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class will be the base class for objects that the player can collect, which will have some form of
 * effect on the game state. This includes changing points modifiers, awarding poitns and restoring 
 * focus points. This will be extended by other sub-classes */

[RequireComponent(typeof(Collider2D))]
public abstract class Pickup : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){				//we want every pickup to check for trigger collisions with the player
		if (other.gameObject.tag == "Player") {
			OnPickup (other);								//pass the collided player to the OnPickup method which will have several different implementations
			gameObject.SetActive(false);
		}
	}

	public abstract void OnPickup (Collider2D other);

}
