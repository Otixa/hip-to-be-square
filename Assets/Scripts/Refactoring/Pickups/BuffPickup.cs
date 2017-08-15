using UnityEngine;

/// <summary>
/// An extension of the Pickup class used to accomodate timed effect Pickups.
/// </summary>
public abstract class BuffPickup: Pickup
{
    #region Properties
    /// <summary>
    /// Duration of the Buff.
    /// </summary>
    [Range(1, 10)]
    public float buffDuration = 5f;

    private Timer _timer;

    /// <summary>
    /// The currently active BuffPickup.
    /// </summary>
    private static BuffPickup _activeBuff;

    /// <summary>
    /// Returns the remaining time the BuffPickup will be active.
    /// </summary>
    public float RemainingTime
    {
        get
        {
            if (_timer == null)
            {
                return 0f;
            }
            return _timer.RemainingTime;
        }
    }

    /// <summary>
    /// Returns the total duration of the BuffPickup.
    /// </summary>
    public float Duration
    {
        get
        {
            return buffDuration;
        }
    }
    #endregion

    #region Abstractions
    /// <summary>
    /// Internal method for managing buff disabling and reset of timer.
    /// </summary>
    internal void _disableBuffInternal()
    {
        GameManager2.OnBuffExpire.Invoke(_activeBuff);
        _activeBuff = null;
        OnExpire();
    }

    /// <summary>
    /// Disables and cancels the timer for the currently active BuffPickup.
    /// Leads to an OnCancel call.
    /// </summary>
    internal void _cancelBuffInternal()
    {
        if (_timer != null)
        {
            _timer.Cancel();
        }
        OnCancel();
        _disableBuffInternal();
    }

    /// <summary>
    /// Method called when the BuffPickup is picked up.
    /// </summary>
    /// <param name="other">The Player GameObject collision information.</param>
	public override sealed void OnPickup(PlayerCollisionEvent other)
    {
        //Debug.Log("Pickup Event on Buffpickup");
        if (_activeBuff != null)  //if a buff is already active
        {
            //Debug.Log("On pickup, buff was already active");
            _activeBuff._cancelBuffInternal();      //get that buff object, and cancel any invokation it's scheduled
        }
        _activeBuff = this;                         //assign active buff to be the current buffPickup that's just been collected
        GameManager2.OnBuffPickup.Invoke(_activeBuff);
        OnApply();                                 //do whatever actions that specific buff needs you to do, defined by the specific buffs

        // Set up Timer
        _timer = new UnscaledTimer(buffDuration, TimerType.OneShot, (_t) => _disableBuffInternal());
        _timer.Start();
    }
    #endregion

    /// <summary>
    /// Gets the globally active BuffPickup instance.
    /// </summary>
    /// <returns>Globally active BuffPickup</returns>
    public static BuffPickup GetActive()
    {
        return _activeBuff;
    }

    /// <summary>
    /// Cancels the buff.
    /// Leads to an OnCancel, and then OnExpire call.
    /// </summary>
    public void Cancel()
    {
        _cancelBuffInternal();
    }

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
    protected virtual void OnCancel() { }

}

//wisdom
//can't override private methods
//shift and del makes a whole line disappeasr <3
//alt up/down switches lines <3
//protected is like private, but allows children (classes that extend this class or it subclasses) to access them still.