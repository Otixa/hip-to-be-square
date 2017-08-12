using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*This component contains a method that can be called, which will spawn a series of coins onto a platform. 
 * You should pass into the function the position where the platform is, along with it's widths. This is 
 * currently designed for use with simple flat platforms */
public class CoinGenerator : MonoBehaviour {
	
	public ObjectPooler coinPool;		//this will be fed into the script prior to execution
	public float gapBetweenCoins;		//when placing coins, this variable can set how much of a gap to place

	void Start () {}

	void Update () {}

	/*COMMENTARY: This was more complicated than planned, because a platfrom of length 7 can actually fit 
	* 8 coins along it's length, as the coins spawn on the grid lines, rather than within the grid cells.*/
	public void spawnCoinGroup(Vector3 spawnPosition, float width){
		int amountOfCoins = (int)Random.Range(1, width+1);					//choose a random amount of coins from 1 to width+1
		int gapFromEdge = 1;												//set the gap on either side of platform you want before coins are spawned
			//calculate an even amount of gap to put between each of the coins
												//this variable keeps track of the x-cordinate as we loop and place amountOfCoins along the platform
		if (amountOfCoins == 1) {											//there is no gap between 1 coin, so instead we just spawn the coin in the center of the platform
			GameObject theCoin = coinPool.Get(obj=>obj.GetComponent<PointPickup>() != null);
			theCoin.transform.position = spawnPosition;
			theCoin.SetActive (true);
		} else {
            float currentXoffset = 0;
            gapBetweenCoins = (width - (2 * gapFromEdge)) / (amountOfCoins - 1);//if there are more than 1 coin to be spawn
            spawnPosition = spawnPosition - new Vector3(width/2, 0f, 0f);		//go back to start of platform
			spawnPosition = spawnPosition + new Vector3(gapFromEdge, 0f, 0f);	//add the gapFromEdge to inset the spawnPosition;
			for (int i = 0; i < amountOfCoins; i++) {							//loop that many times, create a coin, place it and move along
				GameObject theCoin = coinPool.Get(obj=>obj.GetComponent<PointPickup>() != null);
				theCoin.transform.position = spawnPosition + new Vector3 (currentXoffset, 0f, 0f);
                //theCoin.GetComponent<ResettableObject>().resetPosition = theCoin.transform.position;
                theCoin.SetActive (true);
				currentXoffset += gapBetweenCoins;								//update our current X offset so the next coin is placed gapBetweenCoins ahead of this one
			}
		}
	}
}
