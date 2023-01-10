using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Core.Dish
{
    public class DishRequesterSlotUI : MonoBehaviour
    {
        public DishItemSO dish;

        public Image image;

        private void OnEnable()
        {
            RefreshUI();
        }

        public void SetDish(DishItemSO dish)
        {
            this.dish = dish;
            RefreshUI();
        }

        public void ClearDish()
        {
            this.dish = null;
            RefreshUI();
        }

        [Button]
        public void RefreshUI()
        {
            if (dish == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
                if (image)
                {
                    image.sprite = dish.GetSprite();
                }
            }
        }
    }
}