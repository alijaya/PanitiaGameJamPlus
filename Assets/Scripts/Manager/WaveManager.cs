using System.Collections.Generic;
using System.Linq;
using RS.Typing.Core;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using Random = System.Random;

public class WaveManager : MonoBehaviour {
    [SerializeField] private ItemSO[] items;
    [SerializeField] private FloatVariable timeLeftVariable;
    [SerializeField] private float shiftDuration = 90f;
    [SerializeField] private float closingTime = 10f; // waktu sebelum tutup ( tidak spawn customer lagi)
    [SerializeField] private int customerPerShift = 10;

    [SerializeField] private UnityEvent onShiftEnd;

    private RestaurantManager _manager;

    private float[] _customerSequence;
    private int _customerCounter;
    
    private float _warmupDuration = 5f; // minimal delay to spawn other customer
    private float _timerCounter;
    private bool _timerRunning;

    private void Awake() {
        _manager = GetComponentInParent<RestaurantManager>();
    }

    public void StartWave() {
        ItemTray.Instance.ClearTray();
        SetCustomerSequence();

        _customerCounter = 0;
        _timerCounter = shiftDuration;
        _timerRunning = true;
    }

    private void Update() {
        if (!_timerRunning) return;
        if (_timerCounter > 0) {
            _timerCounter -= Time.deltaTime;
            timeLeftVariable.Value = _timerCounter;
            if (_timerCounter < closingTime) return;

            var seq = _customerCounter < customerPerShift?
                _customerSequence[_customerCounter] : _customerSequence[customerPerShift-1] - _warmupDuration *
                (_customerCounter - customerPerShift);

            if (_timerCounter < seq) {
                var orderWeight = new[] {7, 3}; // 70% chance spawn 1 customer, 30% chance spawn 2 customer
                var spawnCount = GetRandomWeight(orderWeight);
                SpawnCustomer(spawnCount);
                _customerCounter += spawnCount;
            }
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
    }

    private void SpawnCustomer(int amount = 1) {
        var orderWeight = new[] { 4, 6}; // 40% chance 1 item order, 60% chance 2 item order
        for (var j = 0; j < amount; j++) {
            var order = new List<ItemSO>();

            for (var i = 0; i < GetRandomWeight(orderWeight); i++) {
                order.Add(items.GetRandom());
            }
            _manager.Spawn(order);    
        }
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
