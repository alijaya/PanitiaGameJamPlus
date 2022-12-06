using System.Collections.Generic;
using System.Linq;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class TestWaveManager : MonoBehaviour {
    [SerializeField] private ItemSO[] items;
    [SerializeField] private float shiftDuration = 90f;
    [SerializeField] private float closingTime = 10f; // waktu sebelum tutup ( tidak spawn customer lagi)
    [SerializeField] private int customerPerShift = 10;

    public WaveSO[] waves;

    private float[] _customerSequence;
    private int _customerCounter;
    
    [SerializeField] private float _warmupDuration = 5f; // minimal delay to spawn other customer
    private float _timerCounter;
    private bool _timerRunning;

    private int timesec;

    private void Awake() {
        StartWave();
    }

    public void StartWave() {
        // SetCustomerSequence();
        print(waves.Length);
        print(waves[0].time);

        _customerCounter = 0;
        _timerCounter = shiftDuration;
        _timerRunning = true;
        timesec = Mathf.RoundToInt(shiftDuration);
    }

    private void Update() {
        if (!_timerRunning) return;
        if (_timerCounter > 0) {
            _timerCounter -= Time.deltaTime;
            if (_timerCounter < closingTime) return;

            if(timesec > Mathf.RoundToInt(_timerCounter)) timesec -= 1;

            for(var i = 0; i < waves.Length; i++ ){
                // print(_timerCounter+" x "+waves[i].time);
                if(timesec == waves[i].time){
                    print("time "+timesec);
                    if(waves[i].isMouse) print("mouse");
                    else {
                        for(var j = 0; j < waves[i].customers.Count; j++){
                            print(waves[i].customers[j].ToString());
                        }
                    }
                }
            }
            
        }
        else {
            _timerRunning = false;
            _timerCounter = 0;
            EndShift();
        }
    }

    private void EndShift() {
        // onShiftEnd?.Invoke();
        print("shift end");
    }

    private void SetCustomerSequence() {
        var customerSeqDelay = shiftDuration / customerPerShift;
        
        var sequence = new List<float> { shiftDuration - _warmupDuration };

        for (var i = 0; i < customerPerShift - 1; i++) {
            var prev = sequence[i];
            var clampedSeqDelay = Mathf.Clamp(
                customerSeqDelay-i , _warmupDuration, customerSeqDelay);
            sequence.Add(prev-clampedSeqDelay);
        }

        _customerSequence = sequence.ToArray();
        foreach( var x in _customerSequence) {
         Debug.Log( x.ToString());
        }
    }

    
}
