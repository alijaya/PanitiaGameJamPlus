using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;

public class RestaurantManager : SingletonSceneMB<RestaurantManager>
{
    public Transform door;

    public WaveManager waveManager;

    protected override void SingletonAwakened()
    {
        GlobalRef.I.CleanUpWords();
    }

    protected override void SingletonStarted()
    {
        GlobalRef.I.totalSales.Value = 0;
        GlobalRef.I.totalCustomerServed.Value = 0;
        GlobalRef.I.PlayBGM_Gameplay();
        waveManager.StartWave();
    }

}
