using System.Collections.Generic;
using System.Linq;
using Core;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class WaveManager : MonoBehaviour {
    public bool autoStart = false;

    public WaveSequenceSO waveSequence;

    public List<CustomerTypeSO> customerTypes;
    public Transform door;

    [SerializeField] private FloatVariable timeLeftVariable;
    [SerializeField] private float shiftDuration = 90f;

    [SerializeField] private UnityEvent onShiftEnd;

    private float _timerCounter;
    private bool _timerRunning;

    private void Start()
    {
        if (autoStart) StartWave();
    }

    public void StartWave() {
        //ItemTray.Instance.ClearTray();

        waveSequence.Setup();

        _timerCounter = shiftDuration;
        _timerRunning = true;
    }

    private void Update() {
        if (!_timerRunning) return;
        if (_timerCounter > 0) {
            _timerCounter -= Time.deltaTime;
            timeLeftVariable.Value = _timerCounter;

            waveSequence.Tick(shiftDuration - _timerCounter);
        }
        else {
            _timerRunning = false;
            _timerCounter = 0;
            timeLeftVariable.Value = _timerCounter;
            EndShift();
        }
    }

    private void EndShift() {
        onShiftEnd?.Invoke();
    }

    private int GetRandomWeight(IEnumerable<int> weights) {
        var currentWeight = weights.ToArray();
        
        var rnd = new Random();
        var totalWeight = new List<int>();
        foreach (var t in currentWeight) {
            var last = totalWeight.Count > 0 ? totalWeight[^1] : 0;
            var weight = checked(last + t);
            totalWeight.Add(weight);
        }

        var totalRandom = rnd.Next(totalWeight[^1]);
        for (var i = 0; i < currentWeight.Length; i++) {
            if (currentWeight[i] > totalRandom) {
                return i + 1;
            }
            totalRandom -= currentWeight[i];
        }

        return 0;
    }
}
