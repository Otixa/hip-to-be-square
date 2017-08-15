using System;
using UnityEngine;

public enum TimerType
{
    OneShot,
    Repeating
}

public class Timer
{
    #region Fields
    protected float duration;
    protected float elapsed = 0f;
    protected TimerType Type;
    protected Action<Timer> Callback;
    public bool IsPaused = true;
    #endregion

    #region Properties
    public bool IsExpired
    {
        get
        {
            if(Type == TimerType.OneShot && RemainingTime > 0)
            {
                return true;
            }
            return false;
        }
    }

    public float RemainingTime
    {
        get
        {
            return Mathf.Clamp(duration - elapsed, 0, duration);
        }
    }

    public float Duration
    {
        get
        {
            return duration;
        }
    }
    #endregion

    /// <summary>
    /// Creates a new Timer instance
    /// </summary>
    /// <param name="duration">Timer duration.</param>
    /// <param name="type">Timer type.</param>
    /// <param name="callback">Optional. Callback to call when Timer expires.</param>
    public Timer(float duration, TimerType type = TimerType.OneShot, Action<Timer> callback = null) {
        this.duration = duration;
        Type = type;
        Callback = callback;
        TimerManager.Add(this);
    }

    /// <summary>
    /// Method which gets called whenever a Timer expires.
    /// </summary>
    protected virtual void OnExpire()
    {
        if (Callback != null)
        {
            Callback.Invoke(this);
        }
    }

    /// <summary>
    /// Method which calculates the time passed between update cycles.
    /// </summary>
    protected virtual void CalculateDelta()
    {
        elapsed += Time.deltaTime;
    }

    public virtual void Pause()
    {
        IsPaused = true;
    }

    public virtual void Resume()
    {
        IsPaused = false;
    }

    public virtual void Start()
    {
        IsPaused = false;
    }

    public virtual void Cancel()
    {
        IsPaused = true;
        Callback = null;
        TimerManager.Remove(this);
    }

    /// <summary>
    /// Timer update cycle.
    /// </summary>
    public virtual void Update()
    {
        if(IsPaused)
        {
            return;
        }

        CalculateDelta();
        if(elapsed > duration)
        {
            elapsed = duration;
            OnExpire();
            if (Type == TimerType.Repeating)
            {
                elapsed = 0f;
            } else
            {
                TimerManager.Remove(this);
            }
        }
    }

}
