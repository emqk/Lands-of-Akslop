using System;
using System.Collections.Generic;
using UnityEngine;

public class TimerUnscaled : Timer
{
    public TimerUnscaled(Action _action, float _unscaledDelay) : base(_action, _unscaledDelay) {}

    public override void Update()
    {
        delay -= Time.unscaledDeltaTime;
    }
}

public class Timer
{
    public Timer(Action _action, float _delay)
    {
        action = _action;
        delay = _delay;
    }

    public virtual void Update()
    {
        delay -= Time.deltaTime;
    }

    public bool ShouldBeInvoked()
    {
        return delay <= 0;
    }

    public void Invoke()
    {
        action();
    }

    protected Action action;
    protected float delay;
}


public class TimerManager : MonoBehaviour
{
    List<Timer> timers = new List<Timer>();

    public static TimerManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        List<int> timersToRemove = new List<int>();

        for (int i = 0; i < timers.Count; i++)
        {
            Timer currTimer = timers[i];

            currTimer.Update();

            if (currTimer.ShouldBeInvoked())
            {
                InvokeTimer(currTimer);
                timersToRemove.Add(i);
            }
        }

        //Remove already invoked timers
        for (int i = timersToRemove.Count-1; i >= 0; i--)
        {
            timers.RemoveAt(timersToRemove[i]);
        }
    }

    public void AddTimer(Timer timer)
    {
        timers.Add(timer);
    }

    void InvokeTimer(Timer timer)
    {
        Debug.Log("Invoking timer action!");
        timer.Invoke();
    }
}