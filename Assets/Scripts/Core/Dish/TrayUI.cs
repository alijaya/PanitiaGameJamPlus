using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dish
{
    public class TrayUI : MonoBehaviour
    {
        public List<Image> slots;

        private void OnEnable()
        {
            Tray.I.OnSlotChanged.AddListener(UpdateDisplay);
        }

        private void OnDisable()
        {
            Tray.I.OnSlotChanged.RemoveListener(UpdateDisplay);
        }

        public void UpdateDisplay()
        {
            for (var i = 0; i < slots.Count; i++)
            {
                if (i < Tray.I.slots.Count)
                {
                    slots[i].enabled = true;
                    slots[i].sprite = Tray.I.slots[i].GetSprite();
                } else
                {
                    slots[i].enabled = false;
                    slots[i].sprite = null;
                }
            }
        }
    }
}