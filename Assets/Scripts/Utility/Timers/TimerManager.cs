using System.Collections.Generic;

class TimerManager : Singleton<TimerManager>
{
    #region Fields
    private static List<Timer> _timers = new List<Timer>();
    #endregion

    /// <summary>
    /// Private constructor to enforce Singleton
    /// </summary>
    private TimerManager() {}

    public static void Add(Timer t)
    {
        UnityEngine.Assertions.Assert.IsNotNull(Instance);
        if (!_timers.Contains(t))
        {
            _timers.Add(t);
        }
    }

    public static void Remove(Timer t)
    {
        UnityEngine.Assertions.Assert.IsNotNull(Instance);
        if (!_timers.Contains(t))
        {
            _timers.Remove(t);
            t = null;
        }
    }

    private void Update()
    {
        _timers.ForEach(t => t.Update());
    }

}
