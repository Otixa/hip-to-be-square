using System;
using UnityEngine;

public class UnscaledTimer : Timer
{
    public UnscaledTimer(float duration, TimerType type = TimerType.OneShot, Action<Timer> callback = null) : base(duration, type, callback) {}

    protected override void CalculateDelta()
    {
        elapsed += Time.unscaledDeltaTime;
    }
}
