using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityAtoms.BaseAtoms;

public class RestaurantManager : MonoBehaviour
{
    public Transform door;

    public WaveManager waveManager;

    private void Awake()
    {
        GlobalRef.I.CleanUpWords();
    }

    private void Start()
    {
        GlobalRef.I.totalSales.Value = 0;
        GlobalRef.I.totalCustomerServed.Value = 0;
        GlobalRef.I.PlayBGM_Gameplay();
        waveManager.StartWave();
    }

    private void Update()
    {

    }
}
