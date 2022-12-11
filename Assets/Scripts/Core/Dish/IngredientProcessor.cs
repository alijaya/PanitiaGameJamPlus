using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class IngredientProcessor : MonoBehaviour {
        [SerializeField] private DishProcessSO[] processes;
        [SerializeField] private DishItemSO burnedDish;
        [SerializeField] private UnityEvent<TrayItemSO> onStageChanged;
        [SerializeField] private UnityEvent<TrayItemSO> onOutputPicked;

        private TrayItemSO _output;
        private CancellationTokenSource _tokenSource;

        private async void StartProcess(DishProcessSO dishProcess) {
            _tokenSource = new CancellationTokenSource();
            var currentStages = 0;
            var stages = dishProcess.GetStages();
            while (currentStages < stages.Length) {
                var end = Time.time + stages[currentStages].timeToProcess;
                while (Time.time < end) {
                    if (_tokenSource.Token.IsCancellationRequested) {
                        _tokenSource.Dispose();
                        return;
                    }
                    await Task.Yield();
                }

                _output = stages[currentStages].output;
                onStageChanged?.Invoke(_output);
                currentStages++;
            }
        }

        public void AddIngredient(IngredientItemSO ingredientItem) {
            var process = processes.First(x => x.GetInput() == ingredientItem);
            if (process) {
                StartProcess(process);
            }
            onStageChanged?.Invoke(ingredientItem);
        }

        public void PickOutput() {
            _tokenSource?.Cancel();
            if (_output || _output != burnedDish) {
                onOutputPicked?.Invoke(_output);

                if (_output is DishItemSO dish) {
                    ItemTray.I.TryAddItemToTray(dish);
                }
            }

            Reset();
        }

        public bool IsIngredientValid(IngredientItemSO ingredientItem) {
            return processes.Any(x => x.GetInput() == ingredientItem);
        }

        private void Reset() {
            _output = null;
            onStageChanged?.Invoke(_output);
        }
    }
}