using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;

public class Timer : MonoBehaviour {
    [SerializeField] private FloatVariable timeLeftVariable;
    [SerializeField] private TextMeshProUGUI timerText;

    private void OnEnable() {
        timeLeftVariable.Changed.Register(UpdateText);
    }

    private void UpdateText(float value) {
        var minutes = Mathf.FloorToInt(value / 60);
        var seconds = Mathf.FloorToInt(value % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnDisable() {
        timeLeftVariable.Changed.Unregister(UpdateText);
    }
}
