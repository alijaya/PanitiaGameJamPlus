using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractWaveTrigger : IWave
{
    public float time;

    public bool triggered { get; private set; } = false;

    public void Setup()
    {
        triggered = false;
    }

    public void Tick(float currentTime)
    {
        if (!triggered && currentTime >= time)
        {
            triggered = true;
            Trigger();
        }
    }

    public float GetEndTime()
    {
        return time;
    }

    public abstract void Trigger();
}
