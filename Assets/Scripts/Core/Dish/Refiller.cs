using System;
using System.Collections;
using Core.Words;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Dish {
    public class Refiller : SerializedMonoBehaviour {
        [SerializeField] private float refillTime;
        [SerializeField] private Func<bool> tryRefill;

        private bool _isRefilling;

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

        IEnumerator RefillProcess() {
            while (_isRefilling) {
                yield return new WaitForSeconds(refillTime);
                _isRefilling = tryRefill.Invoke();
            }   
        }

        public void StartRefill() {
            _isRefilling = true;
            StartCoroutine(RefillProcess());
        }
    }
}   