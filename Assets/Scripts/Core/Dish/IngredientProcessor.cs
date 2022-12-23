using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Util;

namespace Core.Dish {
    public class IngredientProcessor : MonoBehaviour,IIngredientReceiver {
        [SerializeField] private DishProcessSO[] processes;
        [SerializeField] private DishPicker[] collectorSlots;
        
        [Space(10)]
        [SerializeField] private DishItemSO burnedDish;
        [SerializeField] private UnityEvent<bool> isSlotAvailable;

        [SerializeField] private UnityEvent<TrayItemSO> onOutputPicked;

        private readonly Dictionary<DishPicker, KeyValuePair<DishProcessSO,CustomTimer>> _runningProcesses = new ();

        private readonly Queue<DishPicker> _pendingProcesses = new();

        private void Update() {
            var keys = new List<DishPicker>(_runningProcesses.Keys);
            foreach (var collector in keys) {
                var process = _runningProcesses[collector].Key;
                var timer = _runningProcesses[collector].Value;
                
                // cooking timer will stop when chef attempt to pick the food or the dish is burned 
                if (timer.IsStopped()) continue;
                timer.DeltaTick(Time.deltaTime, out var isStageChanged, out var currentStage);
                
                if (isStageChanged) {
                    var output = process.GetStages()[currentStage].output;
                    // TODO: maybe will adding Ingredient picker in the future?
                    if (output is DishItemSO dish) collector.SetupDish(dish == burnedDish ? null: dish);
                }
            }
        }
        private void StartProcess(DishPicker collector,DishProcessSO dishProcessSo) {
            var stagesTime = dishProcessSo.GetStages().Select(x => x.timeToProcess);
            collector.gameObject.SetActive(true);
            
            _runningProcesses[collector] = new KeyValuePair<DishProcessSO, CustomTimer>
                (dishProcessSo, new CustomTimer(stagesTime.ToArray()));
            _runningProcesses[collector].Value.Stop();
            _pendingProcesses.Enqueue(collector);
            
            collector.SetupDish(null);
            collector.GetComponentInChildren<TrayItemUI>().Setup(dishProcessSo.GetInput());
        }

        public void ContinueProcess() {
            if (_pendingProcesses.TryDequeue(out var collector)) {
                _runningProcesses[collector].Value.Continue();
            }
        }
        
        public void AddIngredient(IngredientItemSO ingredientItem) {
            var freeSlot = collectorSlots.FirstOrDefault(x => !_runningProcesses.ContainsKey(x));
            if (!freeSlot) {
                Debug.LogWarning("No slot for this ingredient");
                return;
            }
            
            // find process 
            var process = processes.FirstOrDefault(x => x.GetInput() == ingredientItem);
            if (!process) return;
            
            StartProcess(freeSlot,process);
            isSlotAvailable?.Invoke(IsSlotAvailable());
        }
        public void AddIngredient(TrayItemSO trayItem) {
            if (trayItem is IngredientItemSO ingredientItem) {
                AddIngredient(ingredientItem);
            }
        }
        public void PickOutput(DishPicker collector) {
            _runningProcesses[collector].Value.Stop();
            _runningProcesses.Remove(collector);
            isSlotAvailable?.Invoke(IsSlotAvailable());
        }
        private bool IsSlotAvailable() {
            return !collectorSlots.All(x => _runningProcesses.ContainsKey(x));
        }
        public bool IsBaseIngredient(IngredientItemSO ingredientItem) {
            return processes.Any(x => x.GetInput() == ingredientItem);
        }

    }
}