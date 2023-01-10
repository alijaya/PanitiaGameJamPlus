using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MB_Scripts {
    public class WaitChefHere : MonoBehaviour {
        [SerializeField] private float waitTime;
        private Chef _chef;

        private void Awake()
        {
            _chef = FindObjectOfType<Chef>();
        }

        public async UniTask Wait(CancellationToken ct = default)
        {
            await _chef.WaitForSeconds(waitTime, ct);
        }
    }
}