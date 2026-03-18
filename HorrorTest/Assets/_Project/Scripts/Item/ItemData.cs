using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject {
    public int itemID;
    public string itemName;
    public Sprite icon;
    public GameObject worldPrefab; // 落ちている時の見た目
}