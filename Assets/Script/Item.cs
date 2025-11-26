using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public int level;
    public Weapon weapon;
    public Gear gear;

    Image icon;
    Text textLevel;
    Text textName;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = itemData.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2]; //GetComponent는 계층 순서를 따라감
        textName.text = itemData.itemName;
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level);

        switch (itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                textDesc.text = string.Format(itemData.itemDescription, itemData.damages[level] * 100, itemData.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                textDesc.text = string.Format(itemData.itemDescription, itemData.damages[level] * 100);
                break;
            default:
                textDesc.text = string.Format(itemData.itemDescription);
                break;
        }
    }

    public void OnClick()
    {
        switch (itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(itemData);
                }
                else
                {
                    float nextDamage = itemData.baseDamage;
                    int nextCount = 0;

                    nextDamage += itemData.baseDamage * itemData.damages[level];
                    nextCount += itemData.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(itemData);
                }
                else
                {
                    float nextRate = itemData.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
                
            case ItemData.ItemType.Heal:
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        if (level == itemData.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }        
}
