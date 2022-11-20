﻿using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace RS.Typing.Core {
    public class Order : MonoBehaviour {
        [SerializeField] private IntVariable totalSales;
        [SerializeField] private IntVariable totalCustomerServed;
        [Tooltip("Duration to fulfill the order")]
        [SerializeField] private float orderTimeout;
        [SerializeField] private OrderItem itemPrefab;
        [SerializeField] private Transform itemContainerTransform;
        
        [SerializeField] private UnityEvent orderCompleted;
        [SerializeField] private UnityEvent orderFailed;

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
            var stackSize = new Dictionary<ItemSO, int>();
            foreach (var itemSo in items) {
                stackSize[itemSo] = 0;
            }
            
            foreach (var itemSo in items) {
                var item = Instantiate(itemPrefab, itemContainerTransform);
                item.Setup(itemSo);
                if (ItemTray.Instance.IsItemInTray(itemSo)) {
                    stackSize[itemSo] += 1;
                } 
                item.SetHighlight(stackSize[itemSo] > 0);
                
                _orderedItems.Add(new KeyValuePair<ItemSO, OrderItem>(itemSo, item));
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
                orderCompleted?.Invoke();
                totalCustomerServed.Value++;
            }
        }

        private void StartTimer() {
            _timeRemaining = orderTimeout;
            _timerRunning = true;
        }

        private void Update() {
            if (!_timerRunning) return;
            
            if (_timeRemaining > 0) {
                _timeRemaining -= Time.deltaTime;    
            }
            else {
                _timeRemaining = 0;
                _timerRunning = false;
                orderFailed?.Invoke();
            }
        }
    }
}