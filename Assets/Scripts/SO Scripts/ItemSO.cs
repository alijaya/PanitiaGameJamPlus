using UnityEngine;

[CreateAssetMenu(fileName = "ItemObjective", menuName = "Objective/Item", order = 0)]
public class ItemSO : ScriptableObject {
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private int itemCost;

    public Sprite GetItemIcon() => itemIcon;
    public int GetItemCost() => itemCost;
}