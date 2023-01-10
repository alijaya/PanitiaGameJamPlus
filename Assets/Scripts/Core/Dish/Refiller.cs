using System.Collections;
using System.Linq;
using System.Threading;
using Core.Words;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Dish {
    public class Refiller : SerializedMonoBehaviour {
        [SerializeField] private float refillTime;
        [SerializeField] private UniTaskFunc waitCheck;
        [SerializeField] private DishPickerRefillable[] refillable;

        private bool _isRefilling;
        
        private CancellationTokenSource _waitCheckCancel;

        private void Awake() {
            WordTracker.I.OnTextInput.AddListener(OnTextInput);
        }

        private void OnDestroy() {
            WordTracker.I.OnTextInput.RemoveListener(OnTextInput);
        }

        private void OnTextInput(char arg0) {
            _isRefilling = false;
            StopAllCoroutines();
        }

        private IEnumerator RefillProcess() {
            while (_isRefilling) {
                yield return new WaitForSeconds(refillTime);
                _isRefilling = refillable.Any(x => x.TryRefill());
            }   
        }

        public async void StartRefill() {
            _isRefilling = true;

            _waitCheckCancel ??= new CancellationTokenSource();
            var success = true;
            if (waitCheck.target != null) {
                success = ! await waitCheck.Invoke(_waitCheckCancel.Token).SuppressCancellationThrow();
            }
            
            _waitCheckCancel.Dispose();
            _waitCheckCancel = null;

            if (success) {
                foreach (var dishPicker in refillable) {
                    dishPicker.TryRefill();
                }
            }

            StartCoroutine(RefillProcess());
        }
    }
}   