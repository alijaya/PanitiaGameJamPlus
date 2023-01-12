using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWave
{
    public void Setup();
    public void Tick(float currentTime);
    public float GetEndTime();
}
