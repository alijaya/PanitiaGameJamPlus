using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WaveSequenceSO : ScriptableObject {
    [SerializeReference]
    public  List<IWave> waves;

    public void Setup()
    {
        foreach (var wave in waves)
        {
            wave.Setup();
        }
    }

    public void Tick(float currentTime)
    {
        foreach (var wave in waves)
        {
            wave.Tick(currentTime);
        }
    }

    public float GetEndTime()
    {
        return waves.Max(w => w.GetEndTime());
    }
}