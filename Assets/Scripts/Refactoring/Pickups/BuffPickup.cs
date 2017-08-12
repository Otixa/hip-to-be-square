using UnityEngine;

/// <summary>
/// An extension of the Pickup class used to accomodate timed effect Pickups.
/// </summary>
public abstract class BuffPickup : Pickup {
    #region Properties
    /// <summary>
    /// Duration of the Buff.
    /// </summary>
    [Range(1, 10)]
    public float buffDuration = 5f;

    /// <summary>
    /// The currently active BuffPickup.
    /// </summary>
    private static BuffPickup _activeBuff;
    #endregion

    #region Abstractions
    /// <summary>
    /// Internal method for managing buff disabling and reset of timer.
    /// </summary>
    internal void _disableBuffInternal()                    //THIS ONLY GETS CALLED WHEN A BUFF COMES TO ITS NATURAL END (NOT PREMATURELY CANCELLED)
    {
        _activeBuff = null;
        OnExpire();
    }

    /// <summary>
    /// Disables and cancels the timer for the currently active BuffPickup.
    /// Also calls CancelBuff.
    /// </summary>
    internal void _cancelBuffInternal() {                     //THIS WILL GET CALLED WHEN A NEW PICKUP IS COLLECTED, AND AN OLD PICKUP IS ACTIVE. GETS CALLED ON THE OLD ONE. 
		CancelInvoke ("_disableBuffInternal");  //(WONT HAVE AN INVOKATION OF THIS))      //stop this scheduled one, because we're gonna do it right now instead.
        OnCancel();           //if you wanna do something different if it is cancelled early,
                                //rather than it coming to it's naturally end.
        //DisableBuff ();        //(THIS WOULD GET CALLED A SECOND TIME ON THE SAME OBJECT) //invert all the things the buff changed
        _disableBuffInternal();
	}

    /// <summary>
    /// Method called when the BuffPickup is picked up.
    /// </summary>
    /// <param name="other">The Player GameObject collision information.</param>
	public override sealed void OnPickup (PlayerCollisionEvent other)
	{
        //Debug.Log("Pickup Event on Buffpickup");
		if (_activeBuff != null)  //if a buff is already active
        {
            //Debug.Log("On pickup, buff was already active");
            _activeBuff._cancelBuffInternal ();      //get that buff object, and cancel any invokation it's scheduled
		}
		_activeBuff = this;              //assign active buff to be the current buffPickup that's just been collected
		OnApply ();                    //do whatever actions that specific buff needs you to do, defined by the specific buffs
        if(_activeBuff is FocusBuff){ 
		    Invoke ("_disableBuffInternal", (buffDuration*other.player.GetComponent<GenericPlayer>().slowDownModifier));        //set the disableBuffInternal function to be called in Buff Duration seconds
        }
        else
        {
            Invoke("_disableBuffInternal", buffDuration);        //set the disableBuffInternal function to be called in Buff Duration seconds
        }
    }
    #endregion

    /// <summary>
    /// Method executed when BuffPickup is activated.
    /// </summary>
    protected abstract void OnApply();

    /// <summary>
    /// Method activated when BuffPickup is disabled or expires.
    /// </summary>
    protected abstract void OnExpire();

    /// <summary>
    /// Method called when the BuffPickup is cancelled.
    /// Internally called before DisableBuff
    /// </summary>
    protected virtual void OnCancel() {}
       
    
    public void Cancel()
    {
        _cancelBuffInternal();
    }
}

//wisdom
//can't override private methods
//shift and del makes a whole line disappeasr <3
//alt up/down switches lines <3
//protected is like private, but allows children (classes that extend this class or it subclasses) to access them still.