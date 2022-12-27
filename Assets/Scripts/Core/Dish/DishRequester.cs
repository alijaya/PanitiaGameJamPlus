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
            for (var i = 0; i < requestedDishes.Length; i++) {
                text.text += requestedDishes[i].name;

                if (i != requestedDishes.Length -1) {
                    text.text += " + ";
                }
            }
        }

        public void AttemptToFill() {
            var usedTraySlots = new List<TraySlot>();
            // TODO: Refactor
            //foreach (var dish in requestedDishes) {
            //    Tray.I.HasDish(dish, out var traySlots);
            //    traySlots = traySlots.Except(usedTraySlots).ToArray();

            //    if (!traySlots.Any()) return;
                
            //    usedTraySlots.Add(traySlots[0]);
            //}

            foreach (var traySlot in usedTraySlots) {
                Tray.I.RemoveDish(traySlot.SlotIndex);
            }
            onRequestFullFilled?.Invoke();
        }
    }
}