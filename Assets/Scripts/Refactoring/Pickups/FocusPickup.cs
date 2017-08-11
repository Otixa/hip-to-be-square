using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusPickup : Pickup {
	[SerializeField] private int amountOfFocus;
	private PlayerController thePlayer;

	void Awake(){
		thePlayer = FindObjectOfType<PlayerController>();		//we need this reference to adjust the focus points
	}

	public override void OnPickup (PlayerCollisionEvent other)
	{	//define behaviour focus point pickups
		thePlayer.slowSpeedPoints = Mathf.Clamp(thePlayer.slowSpeedPoints + amountOfFocus, 0, thePlayer.slowSpeedPointsMax);		
			//call it's method which allows us to add points, passing to it the amount of points this coin is worth
	}	
}
