using System.Collections.Generic;
using DG.Tweening;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Core {
    public class Order : MonoBehaviour {
        [SerializeField] private IntVariable totalSales;
        [SerializeField] private IntVariable totalCustomerServed;
        [Tooltip("Duration to fulfill the order")]
        [SerializeField] private float orderTimeout;
        [SerializeField] private OrderItem itemPrefab;
        [SerializeField] private Transform itemContainerTransform;
        
        [SerializeField] private UnityEvent orderCompleted;
        [SerializeField] private UnityEvent orderFailed;

        public Image moodImage;
        public UICircle pie;
        public List<Sprite> moods;

        private readonly List<KeyValuePair<ItemSO, OrderItem>> _orderedItems = new List<KeyValuePair<ItemSO, OrderItem>>();

        private float _timeRemaining;
        private bool _timerRunning;

        private void OnEnable() {
            ItemTray.Instance.ItemTrayUpdated.AddListener(ItemTrayOnItemTrayUpdated);
        }

        private void OnDisable() {
            ItemTray.Instance.ItemTrayUpdated.RemoveListener(ItemTrayOnItemTrayUpdated);
        }

        private void ItemTrayOnItemTrayUpdated(Dictionary<ItemSO, Item> itemTray) {
            var stackSize = new Dictionary<ItemSO, int>();
            foreach (var item in itemTray) {
                stackSize[item.Key] = item.Value.StackSize;
            }
            foreach (var (itemData, itemObject) in _orderedItems) {
                itemObject.SetHighlight(itemTray.ContainsKey(itemData) && stackSize[itemData] > 0);
                stackSize[itemData]--;
            }
        }

        public void Setup(IEnumerable<ItemSO> items) {
            foreach (var itemData in items) {
                var item = Instantiate(itemPrefab, itemContainerTransform);
                item.Setup(itemData);
                _orderedItems.Add(new KeyValuePair<ItemSO, OrderItem>(itemData, item));
            }
            StartTimer();
        }

        public void AttemptCompleteOrder() {
            var temp = new List<KeyValuePair<ItemSO, OrderItem>>();
            foreach (var t in _orderedItems) {
                var item = t.Key;
                var itemUI = t.Value;
                if (!ItemTray.Instance.TryRemoveFromTray(item)) continue; // if tray doesnt have the ordered item continue
                temp.Add(t);
                itemUI.transform.DOScale(new Vector3(0,0,0), .2f).OnComplete(() => {
                    Destroy(itemUI.gameObject);
                });
            }

            foreach (var item in temp) {
                _orderedItems.Remove(item);
                var itemCost = item.Key.GetItemCost(); // TODO - Adding item cost to ItemSO
                totalSales.Value += itemCost;
            }

            if (_orderedItems.Count == 0) {
                orderCompleted.Invoke();
                totalCustomerServed.Value++;
            }
        }

        private void StartTimer() {
            _timeRemaining = orderTimeout;
            _timerRunning = true;
        }

        private void Update() {
            if (!_timerRunning) return;

            _timeRemaining -= Time.deltaTime;

            var timeElapsed = orderTimeout - _timeRemaining;
            var percentage = Mathf.Clamp01(timeElapsed / orderTimeout);
            moodImage.sprite = moods[Mathf.Clamp(Mathf.FloorToInt(percentage * moods.Count), 0, moods.Count-1)];
            pie.SetProgress((percentage * moods.Count) % 1);


            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                _timerRunning = false;
                orderFailed.Invoke();
            }
        }

        private void LateUpdate() {
            CheckItemReady();
        }

        private void CheckItemReady() {
            var stackSize = new Dictionary<ItemSO, int>();
            foreach (var item in ItemTray.Instance.GetItemTray()) {
                stackSize[item.Key] = item.Value.StackSize;
            }
            
            foreach (var (itemData, itemObject) in _orderedItems) {
                itemObject.SetHighlight(ItemTray.Instance.GetItemTray().ContainsKey(itemData) && stackSize[itemData] > 0);
                stackSize[itemData]--;
            }
        }

        public void SetTimerRunning(bool value) {
            _timerRunning = value;
        }

        public void ResetText() {
        }
    }
}