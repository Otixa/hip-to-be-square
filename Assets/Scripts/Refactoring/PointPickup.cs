﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* this class extends the pickup class, and defines the behaviour of a coin pick up */
public class PointPickup : Pickup {
	[SerializeField] private int amountOfPoints;
	private ScoreManager theScoreManager;

	void Awake(){
		theScoreManager = FindObjectOfType<ScoreManager>();			//we need this so we can call the add points function within it
	}

	public override void OnPickup (Collider2D other)
	{	//define behaviour of our coins
		theScoreManager.AddScore(amountOfPoints);					//call it's method which allows us to add points, passing to it the amount of points this coin is worth
	}		
}