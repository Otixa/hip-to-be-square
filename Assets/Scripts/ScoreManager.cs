using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* This script will set up and update the current values to be drawn on the UI
 * along with handling requests to add a certain amount of points to the currentpoints*/
public class ScoreManager : MonoBehaviour {
	public Text PPStest;	//for testing
	public Text playerName;			//get reference to the UI text object that displays the player name
	public Text currentScore;		//get reference to the UI text object that displays the current score
	public Text highScore;			//get reference to the UI text object that displays the highest score
	public Text slowSpeedPoints;	//get reference to the UI text object that displays the slow speed points that remain
	public Text buffDurationText;	//get reference to the text UI object that will show the current duration of buff
	public Image buffImage;			//reference to the image component that will display the larger buff type image
	private float buffDurationCounter;
	public float scoreCounter;		//this is used to keep track of the current score of this game (from start until death)
	public float highScoreCounter;	//this is used to store the highest achieves score within this copy of the game
	public string nameText;			//variable used to store the name to display in the UI
	public float pointsPerSecond;	//this specifies how many points we should add to the score every second that passes
	public bool scoringEnabled;		//this boolean controls whether or not points should be added to the score counter or not (when you die, points should stop increasing).
	private PlayerController thePlayerController;			//reference needed so we can access character name and speed bar value
	private MapSectionGenerator theMapGen;					//reference needed so we can stop generating map when we accumulate enough points
	public GameObject speedBar;								//reference to the UI image that represents how much time slow down points we have remaining (the blue bar)
	public bool spawnedEnd;								//boolean that ensures the endoflevel code is only executed once
	public float scoreCap;								//the limit of points needed to be earned to unlock the level exit
	public GameObject pointsBar;						//reference to the image that represents the numbers of points gained
	public Sprite[] buffImages;							//to store the images for the buff UI#
	public Text VelTest;		//display characters velocity

	void Start () {
		//Debug.Log ("I AM A SCORE MANAGER, I COME FROM ROUND YOUR WAY, WHAT CAN I PLAY!?");
		thePlayerController = FindObjectOfType<PlayerController> ();		//get hold of the player
		theMapGen = FindObjectOfType<MapSectionGenerator>();				//get hold of the MapGenerator
		nameText = thePlayerController.playerName;							//grab the name field from the player
		playerName.text = nameText;											//set the text to show that name
		scoreCounter = 0;													//set scores at 0 at the beginning
		highScoreCounter = 0;												//this sets highscore to be 0, but will be updated if PlayerPrefs holds an existing high score
		if (PlayerPrefs.HasKey ("HighScore")) {								//check if a high score has been set previously in player prefs
			highScoreCounter = PlayerPrefs.GetFloat ("HighScore");			//look up in the PlayerPrefs a key pair value with the key "HighScore" and returns it's corresponding value
		}
		highScore.text = "High Score: " + Mathf.Round(highScoreCounter);	//show the current highscore, we round it so we don't have decimal points in the score
		scoringEnabled = true;												//at start of game scoring should be enabled
		slowSpeedPoints.text = "Slow Time: "+thePlayerController.slowSpeedPoints.ToString("F2")+" / "+thePlayerController.slowSpeedPointsMax;	//display our speed points text (under the blue bar)
		//speedBar.transform.localScale = new Vector3 ((thePlayerController.slowSpeedPoints / thePlayerController.slowSpeedPointsMax), speedBar.transform.localScale.y, speedBar.transform.localScale.z);
		buffImage.color = new Color(0.5f,0.5f,0.5f, 0.2f);
		buffDurationText.color = new Color (1f, 1f, 1f, 0f);
	}

	void Update () {
		if (scoringEnabled) {												//if we are alive and not finished the level
			scoreCounter = Mathf.Clamp(scoreCounter + (pointsPerSecond * Time.deltaTime), 0, scoreCap);				//calculate score per frame and add it to our counter
			currentScore.text = "Score: " +scoreCounter.ToString("F0");		//update the text on the UI to show the score
			playerName.text = nameText;										//update the text on the UI to show the name (this may change mid game)
			if (scoreCounter >= highScoreCounter) {							//check if there has been a new highscore, if so, update it to be the current score and display it
				highScoreCounter = scoreCounter;							//set the high score counter
				highScore.text = "High Score: " + Mathf.Round(highScoreCounter);	//round it and display it on the UI
				PlayerPrefs.SetFloat ("HighScore", highScoreCounter);				//update player prefs with the high score
			}
			pointsBar.transform.localScale = new Vector3 ((scoreCounter / scoreCap), pointsBar.transform.localScale.y, pointsBar.transform.localScale.z);
		}
		slowSpeedPoints.text = "Focus: "+Mathf.Round(thePlayerController.slowSpeedPoints)/*.ToString("F2")*/+" / "+thePlayerController.slowSpeedPointsMax;		//update the text that shows how much slow time points that remain
		//adjusts the scale of the coloured portion of the HP bar. scale of 1 = full, so divide amount left by max amount to get a normalised number to pass into this
		speedBar.transform.localScale = new Vector3 (thePlayerController.slowSpeedPoints / thePlayerController.slowSpeedPointsMax, speedBar.transform.localScale.y, speedBar.transform.localScale.z);
	
		if (scoreCounter >= scoreCap && spawnedEnd == false) {				//check the score to see if it is ready to end the level
			theMapGen.endOfLevel = true;									//tell the map generator to stop generating random map sections
			spawnedEnd = true;												//to ensure this if statement only executes once
			scoringEnabled = false;
		}
			
		//display the buff duration
		if (buffDurationCounter > 0) {
			buffDurationCounter -= Time.deltaTime; 
		}
		buffDurationText.text = ""+Mathf.Round(buffDurationCounter);
		//PPStest.text = ""+pointsPerSecond;
		//VelTest.text = thePlayerController.gameObject.GetComponent<Rigidbody2D> ().velocity.y.ToString();
	}
		
	public void AddScore(int amount){										//a way for other scripts to affect the score
		scoreCounter = Mathf.Clamp(scoreCounter + amount, 0, scoreCap);		//add the amount passed in to the current score
	}

	public void setBuff(BuffPickup buff, float duration){
		if (buff.gameObject.GetComponent<FocusBuff> () != null) {		//if buff of component type focusBuff, then set image to be blue one
			//buffImage = buff.GetComponent<SpriteRenderer> ().sprite;
			buffImage.GetComponent<Image>().overrideSprite = buffImages[0];
			buffImage.color = new Color (1f, 1f, 1f, 1f);
		}else if(buff.gameObject.GetComponent<PointBuff> () != null) {
			buffImage.GetComponent<Image>().overrideSprite = buffImages[1];
			buffImage.color = new Color (1f, 1f, 1f, 1f);
		} else {
			Debug.Log ("Wasnt the right buff bro");
		}
		buffDurationText.color = new Color (1f, 1f, 1f, 1f);
		buffDurationCounter = duration;
		//else if it is a points buff, set it to be the green one
		//else debug log
		//set buffduration as duration


	}

	public void hideBuff(){
		buffImage.color = new Color (0.2f, 0.2f, 0.2f, 0.5f);
		buffDurationText.color = new Color (1f, 1f, 1f, 0f);
	}
}