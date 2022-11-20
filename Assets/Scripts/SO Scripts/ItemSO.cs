using UnityEngine;

[CreateAssetMenu(fileName = "ItemObjective", menuName = "Objective/Item", order = 0)]
public class ItemSO : ScriptableObject {
    [SerializeField] private Sprite itemIcon;
    [SerializeField] private Sprite itemIconColor;
    [SerializeField] private int itemCost;

    public Sprite GetItemIcon() => itemIcon;
    public Sprite GetItemIconColor() => itemIconColor;
    public int GetItemCost() => itemCost;
}