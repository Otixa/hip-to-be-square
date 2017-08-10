using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	This component defines many parameters that governs how the player moves, and also handles collision with objects that would cause the player to
 *  die, or to complete the level. */

public class PlayerController : MonoBehaviour {
	public string playerName;			//name for the player (as there will be three playable chars)
	public float playerSpeed;			//to control the velocity of the player
	public float jumpForce;				//to control the y-axis velocity when the player jumps
	public float jumpTime;				//specifies how long the players jump lasts
	private float jumpTimeCounter;		//this tracks the jump time and counts down as the user is holding the jump key
	private bool stoppedJumping;		//boolean to stop you from being able to jump after falling off platform.
	private Rigidbody2D myRigidbody;	//variables to allow us to use velocity of rigidbody, and the IsTouchingLayers on Collider.
	private bool grounded;				//boolean that is set when the collider on the feet of the player is in contact with something tagged as ground
	public LayerMask whatIsGround;		//specify the type of layer that we will use to detect if we are on the ground and able to perform a jump (used to determined the grounded boolean)
	public Transform feetCheck;			//get a reference to the circle collider placed on our character feet
	public Transform headCheck;			//get a reference to the circle collider placed on our character head
	public float groundCheckRadius;		//allows us to specify the radius of the feet sensor (large may cause hovering)
	public float headCheckRadius;		//allows us to specify the radius of the feet sensor (large may cause hovering)
	private GameManager theGameManager;	//need a reference to the game manager, so that when our player collides with something	that would cause him to die, we need to tell the game manager to restart the game
	//private CameraController theCameraController;	//reference needed so we can stop following the player at end of level!
	public float slowSpeedPointsMax;	//maximum slow speed points
	public float slowSpeedPoints {get; set;}		//current slow speed points
	public float slowDownModifier;		//the rate of which slow down when using the slow speed ability
	public float originalTimeScale;		//need this value to calculate correctly what true half speed is
	private ScoreManager theScoreManager;//need this to stop scoring if we have hit the level exit
	public EndLevelPopup winScreen;		//reference to the canvas object to display upon level completion
	private Animator myAnimator;		//reference to animator so we can set animator parameters
	public string endLevelText;			//the message to display upon completing the current level
	public string nextLevel;			//the name of the next level to load
	public bool freeFocus;				//while true, focus can be used for free! hurrah
	public float prevVelocity;			//this players velocity, BUT LAST FRAME


	void Start () {
		myRigidbody = GetComponent<Rigidbody2D> ();						//we need this to change velocity of our player
		theGameManager = FindObjectOfType<GameManager>();				//we need this to call restart / death screen method upon death
		//theCameraController = FindObjectOfType<CameraController> ();	//get reference to the camera controller
		theScoreManager = FindObjectOfType<ScoreManager>();				//get reference to the score manager
		jumpTimeCounter = jumpTime;				//initiate jumpTimeCounter based on the maximum jump time 
		stoppedJumping = true;					//if this is false, it will not allow us to jump, so need to set as true
		slowSpeedPoints = slowSpeedPointsMax;	//initiate the slow down points to maximum 
		originalTimeScale = 1f;					//reset the time scale to default
		myAnimator = GetComponent<Animator> ();	//get a reference to this players animator component
		myAnimator.SetBool("isMoving", true);
	}
		
	void Update () {
		/* The update method here will constantly update the x-axis velocity of the player. It will also need to
		 * handle the jumping mechanics, allowing the user to jump as desired. This includes not starting a jump
		 * in midair and ensuring you can only jump when your feet are on the ground (rather than face against a wall)*/
		myRigidbody.velocity = new Vector2(playerSpeed, myRigidbody.velocity.y);					//updates the x-axis velocity of the player based on his playerSpeed variable
		grounded = Physics2D.OverlapCircle (feetCheck.position, groundCheckRadius, whatIsGround);	//create a sphere collider at feet, detect if it comes into contact with the layer type associated with "ground".

		if (Physics2D.OverlapCircle (headCheck.position, headCheckRadius, whatIsGround)) {			//stop jumping when HEAD collides with underside of platform
			jumpTimeCounter = 0;																	//expend all of the jump time
			stoppedJumping = true;	
		}
		//AS THE KEY IS PRESSED IN, CHECK IF WE ARE GROUND (GROUND LAYER TOUCHING FEET SENSOR), IF SO ADD JUMP FORCE, SET STOPPED JUMPING TO TRUE
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetMouseButtonDown (0)) {						//check when they key is first pressed to begin the jump
			if (grounded) {																			//only allow the following if grounded is true, meaning player is on a layer defined as "Ground".
				myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);				//add upwards velocity to the character
				stoppedJumping = false;																//this indicates that we have started jumping and are able to hold the key to increase the size of the jump
			}
		}
		//WHILE THE KEY IS HELD AND WE HAVEN'T STOPPED JUMPING, CHECK WE HAVE JUMP TIME LEFT, IF SO ADD JUMP FORCE AND DECREASE JUMP TIME
		if ((Input.GetKey (KeyCode.Space) || Input.GetMouseButton (0)) && !stoppedJumping) {		//check if button is being held down, if so - continue with upwards velocity until jump time is 0
			if (jumpTimeCounter > 0) {																//while jump time still remains
				myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, jumpForce);				//continue applying the jump force on the y axis
				jumpTimeCounter -= Time.deltaTime;													//reduce the timer
			} else {
				stoppedJumping = true;	//MAYBE
			}
		}
		//WHEN THE KEY IS RELEASED, RESET JUMP COUNTER, STOPPED JUMPING = TRUE;
		if (Input.GetKeyUp (KeyCode.Space) || Input.GetMouseButtonUp (0)) {							//when the user comes off the key, stop them being able to expend anymore of the jumpTime (rocketpack)
			jumpTimeCounter = 0;																	//expend all of the jump time
			stoppedJumping = true;																	//set variable that will stop player from entering if statement that allows continued jump velocity
		}
		if (Input.GetKey (KeyCode.LeftShift) || Input.GetMouseButton (1)) {							//check if shift being pressed in order to use up slow down points and half the speed of the game
			if (freeFocus || slowSpeedPoints > 0f ) {																//ensure slow down points remain
				if (!freeFocus) {
					slowSpeedPoints -= Time.deltaTime * slowDownModifier;							//reduce the slow down points (we * by slowDownModifier in order to counter the effect of slowing time on a counter)
				} else {
					//Debug.Log("FREE FOCUS ON: Slowdown modifier is: "+slowDownModifier);
				}

				Time.timeScale = originalTimeScale / slowDownModifier;								//set the game speed based on the slow down modifier (example: 2 would mean speed is halved)
				myAnimator.SetBool("isFocusing", true);												//boolean that trigger the animation to start
				//myAnimator.SetBool("isMoving", false);
			}
		}
		if ((Input.GetKeyUp (KeyCode.LeftShift) || Input.GetMouseButtonUp (1)) || slowSpeedPoints <= 0) {	//to track when player stops expending slow down speed points
			Time.timeScale = originalTimeScale;																//reset the speed to the original speed (1f)
			if (slowSpeedPoints < 0) {																		//to ensure that the lowest slowSpeedPoints possible is 0.00. 
				slowSpeedPoints = 0;
			}
			myAnimator.SetBool("isFocusing", false);														//boolean that trigger the animation to stop
			//myAnimator.SetBool("isMoving", true);
		}
		if (grounded) {						//when the player lands, reset the timer so the player can again jump
			jumpTimeCounter = jumpTime;
			myAnimator.SetBool("isJumping", false);

		} else {
			myAnimator.SetBool("isJumping", true);
			//if (myRigidbody.velocity.y < -11) {
			//	myRigidbody.velocity = new Vector2 (myRigidbody.velocity.x, -11f);
			//}
			//Debug.Log (myRigidbody.velocity.y);
		}


	}
		
	void FixedUpdate(){				//this update will execute prior to collision code calculations
		prevVelocity = myRigidbody.velocity.y;
	}

	void OnCollisionEnter2D(Collision2D other){			//other refers to the other object with a 2D collider that we've bumped into
		if (other.gameObject.tag == "triggerdeath") {	//if we have hit an object with the tag triggerdeath (used defined tag)
			theGameManager.RestartGame ();				//call the restart game method within the game manager, which brings up menu asking user if they want to restart
		}
	}

	void OnTriggerEnter2D(Collider2D other){			//TriggerEnter is used for colliders which are set as triggers, allowing objects to pass through them
		if (other.gameObject.name == "LevelExit") {		//if we have gone through a trigger with the name LevelExit			
			//theCameraController.endOfLevel = true;		//tell the camera that we've reached the end of level (so it can stop following us)
			theScoreManager.scoringEnabled = false;		//stops the points from accumulating once we have hit the end level block
			playerSpeed = 0;							//stop motion of player on x						
			jumpForce = 0;								//stop motion of player on y
			myAnimator.SetBool("isMoving", false);		//variable to control the animation of the character
			winScreen.gameObject.SetActive(true);					//display the win screen           
			winScreen.setupPopup(endLevelText, nextLevel);			//tell the popup that appears when you collide with exit what it should display
		} 
	}
}