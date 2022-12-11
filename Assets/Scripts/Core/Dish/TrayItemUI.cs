using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Dish {
    public class TrayItemUI : MonoBehaviour {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI stackText;

        private void Start() {
            stackText.enabled = false;
        }
    }
}   