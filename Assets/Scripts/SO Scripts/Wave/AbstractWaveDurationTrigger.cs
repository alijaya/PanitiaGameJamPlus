using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractWaveDurationTrigger : AbstractWaveDuration
{
    public int count = 1;
    public bool triggerOnStart = true;
    public bool triggerOnEnd = true;

    public bool random = false;

    private List<float> triggers = new ();
    private int triggerIndex = 0;

    public override void Setup()
    {
        base.Setup();

        var restCount = count;
        if (triggerOnStart) restCount--;
        if (triggerOnEnd) restCount--;
        restCount = Mathf.Max(restCount, 0); // clamp

        var span = (endTime - startTime) / (restCount + 1);

        triggers = new();
        if (triggerOnStart) triggers.Add(startTime);
        for (var i = 0; i < restCount; i++)
        {
            if (!random)
            {
                triggers.Add(startTime + span * (i + 1));
            } else
            {
                triggers.Add(Random.Range(startTime, endTime));
            }
        }
        if (triggerOnEnd) triggers.Add(endTime);

        triggers.Sort();
        triggerIndex = 0;
    }

    public override void TriggerStart()
    {
        InDuration(startTime);
    }

    public override void InDuration(float currentTime)
    {
        while (triggerIndex < triggers.Count && currentTime >= triggers[triggerIndex])
        {
            Trigger();
            triggerIndex++;
        }
    }

    public override void TriggerEnd()
    {
        InDuration(endTime);
    }

    public abstract void Trigger();
}
