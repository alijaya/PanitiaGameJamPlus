using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dish
{
    public class DishRequesterUI : MonoBehaviour
    {
        public DishRequester dishRequester;

        public List<Image> slots;

        private void OnEnable()
        {
            dishRequester.OnDishesChanged.AddListener(RefreshUI);
        }

        private void OnDisable()
        {
            dishRequester.OnDishesChanged.RemoveListener(RefreshUI);
        }

        public void RefreshUI()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                var slot = slots[i];
                slot.gameObject.SetActive(false);
                if (i < dishRequester.requestedDishes.Count)
                {
                    var dish = dishRequester.requestedDishes[i];
                    var complete = dishRequester.completes[i];
                    if (!complete)
                    {
                        slot.gameObject.SetActive(true);
                        slot.sprite = dish.GetSprite();
                    }
                }
            }
        }
    }
}