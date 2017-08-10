using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is a script to attach to things that give you points. We give each object an amount of points */
public class PickupPoints : MonoBehaviour {
	public int amount;							//number of points to add
	private ScoreManager theScoreManager;		//need score manager so we can change the score when there is an interaction with this object
	public bool respawnOnReset;

	void Start () {
		theScoreManager = FindObjectOfType<ScoreManager>();		//get the reference to the score manager
	}

	void Update () { }

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player") {		//if the thing that collides with this object is the player
			theScoreManager.AddScore(amount);			//add score
			gameObject.SetActive (false);				//deactivate the object "pickup"
		}
	}
}
