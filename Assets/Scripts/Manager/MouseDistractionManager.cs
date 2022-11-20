using System;
using System.Collections.Generic;
using System.Linq;
using RS.Typing.Core;
using UnityEngine;

public class MouseDistractionManager : MonoBehaviour {
    [SerializeField] private Mouse mousePrefab;
    [SerializeField] private Transform doorTransform;

    private int _itemUpdated;
    private void Start() {
        ItemTray.Instance.ItemTrayUpdated.AddListener(ObserveItemTray);
    }

    private void OnDestroy() {
        ItemTray.Instance.ItemTrayUpdated.RemoveListener(ObserveItemTray);
    }

    private void ObserveItemTray(Dictionary<ItemSO, Item> itemTray) {
        var itemCount = itemTray.Values.Sum(item => item.StackSize);
        if (itemCount > 3) _itemUpdated++;
        
        if (_itemUpdated > 2) {
            SpawnMouse();
            _itemUpdated = 0;
        }
    }

    private void SpawnMouse() {
        var mouse = Instantiate(mousePrefab, doorTransform.position, Quaternion.identity);
        mouse.doorTransform = doorTransform;
    }
}