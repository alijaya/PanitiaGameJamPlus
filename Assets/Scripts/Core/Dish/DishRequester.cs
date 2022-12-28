using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Core.Dish {
    public class DishRequester : MonoBehaviour {

        public List<DishItemSO> requestedDishes;

        public List<bool> completes { get; private set; }

        public Core.Words.WordObject wordObject;

        public UnityEvent OnDishesChanged;
        public UnityEvent OnRequestCompleted;

        public void Setup(List<DishItemSO> requestedDishes)
        {
            this.requestedDishes = requestedDishes;
        }

        private void OnEnable()
        {
            completes = new();
            for (var i = 0; i < requestedDishes.Count; i++) completes.Add(false);
            OnDishesChanged?.Invoke();
        }

        public void AttemptToFill() {
            for (var i = 0; i < requestedDishes.Count; i++)
            {
                var dish = requestedDishes[i];
                if (!completes[i])
                {
                    if (Tray.I.RemoveDish(dish))
                    {
                        completes[i] = true;
                        OnDishesChanged?.Invoke();
                    }
                }
            }

            if (completes.All(complete => complete))
            {
                OnRequestCompleted?.Invoke();
            }
        }
    }
}