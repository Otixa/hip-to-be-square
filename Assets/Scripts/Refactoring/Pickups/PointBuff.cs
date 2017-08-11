using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is a buff that multiplies the points per second a player gets
 * by a set amount for a set duration. */
public class PointBuff : BuffPickup {
	private ScoreManager theScoreManager;						//need this as score manager controls the scoring
	public float buffAmount;									//how strong the strong the buff is (modifier)

	void Awake(){
		theScoreManager = FindObjectOfType<ScoreManager>();		//we need this so we can call the add points function within it
	}

	protected override void OnApply ()
	{
		//Debug.Log("On Apply in Buff Points (Green Gem)");
		//Debug.Log("PPS was: "+theScoreManager.pointsPerSecond);
		theScoreManager.pointsPerSecond *= buffAmount;			//tell score manager to multiply the points per second by buffAmount
		//Debug.Log("PPS is now: "+theScoreManager.pointsPerSecond);
		theScoreManager.setBuff (this, buffDuration);
	}

	protected override void OnExpire ()
	{
		//Debug.Log ("I've hit disableBuff POINTS");
		theScoreManager.pointsPerSecond /= buffAmount;			//tell score manager to divide the points per second by theBuffAmount
		theScoreManager.hideBuff();
	}

}
