using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This component controls how the camera should behave during the game. It checks the position of the player, and moves with the player. However, 
 * we do not want the camera to always follow the players Y co-ordinate, though we do allow some movement when the platforms spawn close to the top
 * of the screen. */
public class CameraController : MonoBehaviour {
	
	private GenericPlayer thePlayerController;	//so we can get hold of our player in order to follow his X co-ordinate through the game
	private Vector3 previousPlayerPosition;			//variable to store the players position, we will use this to update the cameras position relative to the player
	private float distanceMoved;					//to store gap between camera and player
	private float originalCameraHeight;				//store the starting height of camera, to return to when camera pans up at top of map.
	private float maxCameraHeight;					//store the maximum y coordinate the camera is allowed to pan to when above the heightToStartPan
	public float heightToStartPan;					//we set this value which says at which y co-ordinate should the camera start following the y movement
	public bool endOfLevel;							//a boolean that is turned on to tell the player end of level is reached, and no need to continue running indefintely!
	public float viewOffset = 6;

    void Start () {
		thePlayerController = FindObjectOfType<GenericPlayer> ();		    //instantiate the playerController Object
		previousPlayerPosition = thePlayerController.transform.position;	////we use a vector 3 as we will need to use 3D coords for the camera object, even if only using 2D
		originalCameraHeight = transform.position.y;						//set the original camera height to a variable, so we can refer back to it later when moving camera up and down
		maxCameraHeight = originalCameraHeight * 1.25f;						//define how much we want the camera to be able to scroll upwards when you're near top of screen (so if original height was 10, it can go to 12.5)
		transform.position = new Vector3(thePlayerController.transform.position.x + viewOffset, transform.position.y, transform.position.z);
	}
		
	void Update () {	
		if (!endOfLevel) {																			//how we want the camera to behave whilst the game is running
			distanceMoved = thePlayerController.transform.position.x - previousPlayerPosition.x;	//work out how far camera needs to move, being how far the character has moved since last frame
			if (thePlayerController.transform.position.y > heightToStartPan) { 						//if you are towards top of screen, meaning the camera should start scrolling
				if (transform.position.y < maxCameraHeight) {										//make sure we haven't panned higher than the limit of our camera pan on the y axis
					transform.position = new Vector3 (transform.position.x + distanceMoved, transform.position.y + 0.1f, transform.position.z); //adjust y co-ordinate of camera (pan it up) by 0.1f

					//GameObject tempGenPoint = transform.Find ("GenerationPoint").gameObject;		//get a reference to the generation point, then move it inversely so that it doesn't follow the camera as we pan.
					//tempGenPoint.transform.position = new Vector3 (tempGenPoint.transform.position.x, tempGenPoint.transform.position.y - 0.1f,	tempGenPoint.transform.position.z);
				} else {																			//if we are at top of the screen, but have reached our upper limit of how high to scroll
					transform.position = new Vector3 (transform.position.x + distanceMoved, transform.position.y, transform.position.z);	//move regularly based on how much the player has moved
				}
			} else {																				//if the player is no longer above the pan scroll point
				if (transform.position.y > originalCameraHeight) {									//check if you've not scrolled down back to the original height
					transform.position = new Vector3 (transform.position.x + distanceMoved, transform.position.y - 0.1f, transform.position.z); 	//scroll the camera down (as well as the usual forwards);
					//GameObject tempGenPoint = transform.Find ("GenerationPoint").gameObject;		//get a reference to the generation point, then move it inversely so that it doesn't follow the camera as we pan back down
					//tempGenPoint.transform.position = new Vector3 (tempGenPoint.transform.position.x, tempGenPoint.transform.position.y + 0.1f,	tempGenPoint.transform.position.z);
				} else {																			//if the camera has scrolled back to the original camera height
					transform.position = new Vector3 (transform.position.x + distanceMoved, transform.position.y, transform.position.z);	//move just along the x plane by the amount the player has moved
				}
			}
			previousPlayerPosition = thePlayerController.transform.position;						// this variable stores the current position, so you can compare next frame to calculate how far player has moved (and then move the camera that much)
		} else {
			//the end of level has been reached, and player has collided with the level exit. We no longer need to follow the player. IMPROVEMENT: We may not need this, as if we just stop moving the player, the camera will stop too.
		}
	}
}