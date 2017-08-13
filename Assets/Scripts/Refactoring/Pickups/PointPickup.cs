using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this class extends the pickup class, and defines the behaviour of a coin pick up */
public class PointPickup : Pickup {
	public int amountToGive = 10;

	public override void OnPickup (PlayerCollisionEvent other)
	{	//define behaviour of our coins
		GameManager2.Instance.AddPoints(amountToGive);					    //call it's method which allows us to add points, passing to it the amount of points this coin is worth
	}		
}