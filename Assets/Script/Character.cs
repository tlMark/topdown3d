using UnityEngine;

public class Character : MonoBehaviour
{
    public static float SpeedRate
    {
        get
        {
            return GameManager.instance.playerId == 0 ? 1.1f : 1f;
        }
    }
    public static float WeaponSpeedRate
    {
        get
        {
            return GameManager.instance.playerId == 1 ? 1.1f : 1f;
        }
    }
    public static float WeaponRate
    {
        get
        {
            return GameManager.instance.playerId == 1 ? 0.9f : 1f;
        }
    }
    public static float WeaponDamage
    {
        get
        {
            return GameManager.instance.playerId == 2 ? 1.2f : 1f;
        }
    }
    public static int WeaponCount
    { 
        get
        {
            return GameManager.instance.playerId == 3 ? 1 : 0;
        }
    }
}
