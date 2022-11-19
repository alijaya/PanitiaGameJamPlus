using UnityEngine;

[CreateAssetMenu(fileName = "ItemObjective", menuName = "Objective/Item", order = 0)]
public class ItemSO : ScriptableObject {
    [SerializeField] private Sprite itemIcon;

    public Sprite GetItemIcon() => itemIcon;
}