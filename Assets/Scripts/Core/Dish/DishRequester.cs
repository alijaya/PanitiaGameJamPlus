using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Core.Dish {
    public class DishRequester : MonoBehaviour {
        public DishItemSO[] requestedDishes;
        public TextMeshProUGUI text;

        public UnityEvent onRequestFullFilled;

        private void Start() {
            foreach (var dish in requestedDishes) {
                text.text += dish.name;

                if (dish != requestedDishes[^1]) {
                    text.text += " + ";
                }
            }
        }

        public void AttemptToFill() {
            var usedTraySlots = new List<TraySlot>();
            foreach (var dish in requestedDishes) {
                ItemTray.I.HasDish(dish, out var traySlots);
                traySlots = traySlots.Except(usedTraySlots).ToArray();

                if (!traySlots.Any()) return;
                
                usedTraySlots.Add(traySlots[0]);
            }

            foreach (var traySlot in usedTraySlots) {
                ItemTray.I.TakeDish(traySlot.SlotIndex);
            }
            onRequestFullFilled?.Invoke();
        }
    }
}