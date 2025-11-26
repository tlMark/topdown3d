using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]

public class ItemData : ScriptableObject
{
    public enum ItemType
    { Melee, Range, Glove, Shoe, Heal }
    
    [Header("# Main Info")]
    public ItemType itemType;
    public int itemID;
    public string itemName;
    [TextArea]
    public string itemDescription;
    public Sprite itemIcon;

    [Header("# Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;

    [Header("# Weapon Data")]
    public GameObject projectile;
    public Sprite hand;
}