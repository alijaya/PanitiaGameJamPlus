using System.Collections.Generic;
using System.Linq;
using Core;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;
using Sirenix.OdinInspector;

public class WaveManager : SingletonSceneMB<WaveManager> {
    public bool autoStart = false;

    [InlineEditor]
    public WaveSequenceSO waveSequence;

    [SerializeField] private UnityEvent onShiftEnd;

    private float shiftDuration;
    private bool _timerRunning;

    protected override void SingletonStarted()
    {
        if (autoStart) StartWave();
    }

    public void StartWave(WaveSequenceSO waveSequence)
    {
        this.waveSequence = waveSequence;
        StartWave();
    }

    public void StartWave() {
        waveSequence.Setup();

        shiftDuration = waveSequence.GetEndTime();
        GlobalRef.I.timeElapsed.Value = 0;
        GlobalRef.I.shiftDuration.Value = shiftDuration;
        _timerRunning = true;
    }

    private void Update() {
        if (!_timerRunning) return;
        if (GlobalRef.I.timeElapsed.Value < shiftDuration) {
            GlobalRef.I.timeElapsed.Value += Time.deltaTime;

            waveSequence.Tick(GlobalRef.I.timeElapsed.Value);
        }
        else {
            GlobalRef.I.timeElapsed.Value = shiftDuration;

            // wait until there's no CustomerGroup
            // ga performant, bodo amat
            if (FindObjectOfType<CustomerGroup>() == null)
            {
                _timerRunning = false;
                EndShift();
            }
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
