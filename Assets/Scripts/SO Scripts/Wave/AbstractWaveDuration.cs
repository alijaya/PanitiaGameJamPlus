using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractWaveDuration : IWave
{
    public float startTime;
    public float endTime;

    public bool passStartTime { get; private set; } = false;
    public bool passEndTime { get; private set; } = false;

    public virtual void Setup()
    {
        passStartTime = false;
        passEndTime = false;
    }

    public void Tick(float currentTime)
    {
        // make sure startTime <= endTime
        if (startTime > endTime)
        {
            var temp = startTime;
            startTime = endTime;
            endTime = temp;
        }

        if (!passStartTime && currentTime >= startTime)
        {
            passStartTime = true;
            TriggerStart();
        }

        if (startTime <= currentTime && currentTime <= endTime)
        {
            InDuration(currentTime);
        }


        if (!passEndTime && currentTime >= endTime)
        {
            passEndTime = true;
            TriggerEnd();
        }

    }

    public abstract void TriggerStart();
    public abstract void InDuration(float currentTime);
    public abstract void TriggerEnd();
}
