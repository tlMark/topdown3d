using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData itemData)
    {
        //Basic set
        name = "Gear " + itemData.itemID;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        //Property set
        type = itemData.itemType;
        rate = itemData.damages[0];
        ApplyGear();
    }

    public void LevelUp(float nextRate)
    {
        this.rate = nextRate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    float speed = 150 * Character.WeaponSpeedRate;
                    weapon.speed = speed + (speed * rate);
                    break;
                case 1:
                    float baseSpeed = 0.5f * Character.WeaponRate;
                    weapon.speed = baseSpeed * (1f * rate);
                    break;
            }
        }
    }
    void SpeedUp()
    {
        float speed = 3 * Character.SpeedRate;

        GameManager.instance.player.speed = speed + speed * rate;
    }
}
