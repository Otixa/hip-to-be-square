using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStats
{
    public string name;
    public Vector3 position;
    public float focus;
    public Vector3 velocity;
    public int attempts;
}

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]     //!!is this the correct way to require both?
public abstract class GenericPlayer : MonoBehaviour {
    public Vector3 resetPosition;                   //as will need to access this from object pooler when positions are changing as objects are spawned and destroyed
    public bool shouldReset = true;                 //for special situation where we want a resettableObject to behave differently 

    protected Collider2D _collider;
    protected Rigidbody2D _rigidbody;
    private Animator _animator;		                        //reference to animator so we can set animator parameters

    public PlayerStats playerStats;
    [Header("Control Parameters")]
    protected float moveVelocity;                           //the amount of velocity on the x-axis of the player
    protected float jumpVelocity;                           //the amount of y-axis velocity when the player jumps
    protected float jumpDuration;
    private float jumpDurationCurrent;
    private bool isGrounded;
    private bool stoppedJumping;
    [Range(0.05f, 0.25f)] public float feetSensorRadius = 0.1f;
    [Range(0.05f, 0.25f)] public float headSensorRadius = 0.1f;
    public LayerMask groundLayer; // = LayerMask.GetMask("Ground");//specify the type of layer that we will use to detect if we are on the ground and able to perform a jump (used to determined the grounded boolean)

    [Header("Focus Settings")]
    public bool freeFocus;                                           //!! change to game manager eventually
    public float slowDownModifier = 0.5f;                            //decimal point value
    public float maxFocus = 5f;

    [Header("Additional Settings")]
    public float lastFrameVelocity;			    //this players velocity, BUT LAST FRAME - used for some player collisions to detect the downwards velocity upon impact


    private void OnDrawGizmos()
    {
        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetFeetPosition(), 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(GetHeadPosition(), 0.1f);
    }

    // Use this for initialization
    protected void Awake () {
        resetPosition = transform.position;
        name = this.ToString();
        playerStats.focus = maxFocus;
        _collider = GetComponent<Collider2D>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        GameManager2.OnGameReset += ResetLevel;
        UIManager.OnDialogClose += DialogClose;
        UIManager.OnDialogOpen += DialogOpen;
    }

    private void DialogOpen()
    {
        //Debug.Log("Dialog open: player disabled");
        enabled = false;
    } 

    private void DialogClose()
    {
        //Debug.Log("Dialog closed: player enabled");
        enabled = true;
    }

    // Update is called once per frame
    void Update () {
        /* The update method here will constantly update the x-axis velocity of the player. It will also need to
		 * handle the jumping mechanics, allowing the user to jump as desired. This includes not starting a jump
		 * in midair and ensuring you can only jump when your feet are on the ground (rather than face against a wall)*/
        _rigidbody.velocity = new Vector2(moveVelocity, _rigidbody.velocity.y);                                         //updates the x-axis velocity of the player based on his playerSpeed variable
        isGrounded = Physics2D.OverlapCircle(GetFeetPosition(), feetSensorRadius, groundLayer);     //create a sphere collider at feet, detect if it comes into contact with the layer type associated with "ground".

        if (Physics2D.OverlapCircle(GetHeadPosition(), headSensorRadius, groundLayer))
        {                                                                   //stop jumping when HEAD collides with underside of platform
            jumpDurationCurrent = 0;                                        //expend all of the jump time
            stoppedJumping = true;
        }
        
        if (Input.GetButtonDown("Jump"))                                    //AS THE KEY IS PRESSED IN, CHECK IF WE ARE GROUND (GROUND LAYER TOUCHING FEET SENSOR), IF SO ADD JUMP FORCE, SET STOPPED JUMPING TO TRUE
        {                                                                   //check when they key is first pressed to begin the jump
            if (isGrounded)
            {                                                                                       //only allow the following if grounded is true, meaning player is on a layer defined as "Ground".
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpVelocity);             //add upwards velocity to the character
                stoppedJumping = false;                                                             //this indicates that we have started jumping and are able to hold the key to increase the size of the jump
            }
        }
        
        if (Input.GetButton("Jump") && !stoppedJumping)                     //WHILE THE KEY IS HELD AND WE HAVEN'T STOPPED JUMPING, CHECK WE HAVE JUMP TIME LEFT, IF SO ADD JUMP FORCE AND DECREASE JUMP TIME
        {                                                                   //check if button is being held down, if so - continue with upwards velocity until jump time is 0
            if (jumpDurationCurrent > 0)
            {                                                               //while jump time still remains
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, jumpVelocity);                 //continue applying the jump force on the y axis
                jumpDurationCurrent -= Time.deltaTime;                                                  //reduce the timer
            }
            else
            {
                stoppedJumping = true;  //MAYBE
            }
        }
        
        if (Input.GetButtonUp("Jump"))                                      //WHEN THE KEY IS RELEASED, RESET JUMP COUNTER, STOPPED JUMPING = TRUE;
        {                                                                   //when the user comes off the key, stop them being able to expend anymore of the jumpTime (rocketpack)
            jumpDurationCurrent = 0;                                        //expend all of the jump time
            stoppedJumping = true;                                          //set variable that will stop player from entering if statement that allows continued jump velocity
        }

        if (Input.GetButton("Focus"))
        {                                                                   //check if shift being pressed in order to use up slow down points and half the speed of the game
            if (freeFocus || playerStats.focus > 0f)
            {                                                               //ensure slow down points remain
                if (!freeFocus)
                {
                    //!! this doesn't worked as we hoped

                    // 1. To reduce speed we * 0.5, giving us newSpeed
                    // How can we use that 0.5 modifier, to get the newSpeed back to 1. 

                    // 1. To reduce speed we * 0.25, giving us newSpeed
                    // How can we use that 0.25 modifier, to get the newSpeed back to 1. 

                    // 1. To reduce speed we * 0.2, giving us newSpeed
                    // How can we use that 0.2 modifier, to get the newSpeed back to 1. 

                    // 1 / modifier = multiplication rate    
                    playerStats.focus -= Time.unscaledDeltaTime ;            //reduce the slow down points (we * by slowDownModifier in order to counter the effect of slowing time on a counter)
                    //playerStats.focus -= (Time.deltaTime * (1 / slowDownModifier));               //hahaha @ Otixa this is even worse than my old way!
                }
                else
                {
                    //Debug.Log("FREE FOCUS ON: Slowdown modifier is: "+slowDownModifier);
                }

                Time.timeScale = slowDownModifier;                      //set the game speed based on the slow down modifier (example: 2 would mean speed is halved)
                _animator.SetBool("isFocusing", true);                  //boolean that trigger the animation to start                                                                                                   //myAnimator.SetBool("isMoving", false);
            }
        }

        if (Input.GetButtonUp("Focus") || (playerStats.focus <= 0 && !freeFocus))
        {                                                               //to track when player stops expending slow down speed points
            Time.timeScale = 1f;                                        //reset the speed to the original speed
            if (playerStats.focus < 0)
            {                                                           //to ensure that the lowest playerStats.focus possible is 0.00. 
                playerStats.focus = 0;
            }
            _animator.SetBool("isFocusing", false);                    //boolean that trigger the animation to stop                                                                                                           
        }
        if (isGrounded)                                                 //when the player lands, reset the timer so the player can again jump
        {                       
            jumpDurationCurrent = jumpDuration;
            _animator.SetBool("isJumping", false);
        }
        else
        {
            _animator.SetBool("isJumping", true);
        }
    }

    void FixedUpdate()                                                  //this update will execute prior to collision code calculations
    {               
        lastFrameVelocity = _rigidbody.velocity.y;
    }

    private void UpdatePlayerStats()
    {
        playerStats.position = transform.position;
        playerStats.velocity = _rigidbody.velocity;
    }

    protected void ResetLevel()
    {
        transform.position = resetPosition;
        playerStats.focus = maxFocus;
    }

    private Vector3 GetFeetPosition()
    {
        Vector3 feetPosition = _collider.bounds.center;                 //get middle
        feetPosition.y -= _collider.bounds.extents.y;                   //subtract half of the y size
        return feetPosition;
    }

    private Vector3 GetHeadPosition()
    {
        Vector3 headPosition = _collider.bounds.center;                 //get middle
        headPosition.y += _collider.bounds.extents.y;                   //add half of the y size
        return headPosition;
    }

    public override string ToString() {
        return this.GetType().ToString();
    }

}
