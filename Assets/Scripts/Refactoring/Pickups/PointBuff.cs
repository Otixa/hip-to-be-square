using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class is a buff that multiplies the points per second a player gets
 * by a set amount for a set duration. */
public class PointBuff : BuffPickup {
	//private ScoreManager theScoreManager;						    //need this as score manager controls the scoring
	public float pointsModifier = 2f;									//how strong the strong the buff is (modifier), 2.0 is double for example

	protected override void Awake(){
        base.Awake();
		//theScoreManager = FindObjectOfType<ScoreManager>();		    //we need this so we can call the add points function within it
	}

	protected override void OnApply ()
	{
		GameManager2.Instance.pointsPerSecond *= pointsModifier;        //tell score manager to multiply the points per second by buffAmount
        UIManager.Instance.SetBuff(buffDuration);                       //sorting the UI to display the buff
    }

	protected override void OnExpire ()
	{
        GameManager2.Instance.pointsPerSecond /= pointsModifier;            //tell score manager to divide the points per second by theBuffAmount
        UIManager.Instance.HideBuff();
	}

}
