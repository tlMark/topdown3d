using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public enum InfoType { EXP, Level, Kills, Time, Health }

    public InfoType type;

    Text tExt;
    Slider sLider;

    void Awake()
    {
        tExt = GetComponent<Text>();
        sLider = GetComponent<Slider>();
    }

    void LateUpdate()
    {
        switch (type)
        {
            case InfoType.EXP:
                float curexp = GameManager.instance.exp;
                float maxexp = GameManager.instance.nextExp[Mathf.Min(GameManager.instance.level, GameManager.instance.nextExp.Length)];
                sLider.value = curexp / maxexp;
                break;
            case InfoType.Level:
                tExt.text = string.Format("Lv.{0:F0}", GameManager.instance.level);
                break;
            case InfoType.Kills:
                tExt.text = string.Format("{0:F0}", GameManager.instance.kill);
                break;
            case InfoType.Time:
                float reaminTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                int min = Mathf.FloorToInt(reaminTime / 60);
                int sec = Mathf.FloorToInt(reaminTime % 60);
                tExt.text = string.Format("{0:D2}:{1:D2}", min, sec);
                break;
            case InfoType.Health:
                float curHealth = GameManager.instance.health;
                float maxHealth = GameManager.instance.maxHealth;
                sLider.value = curHealth / maxHealth;
                break;
        }
    }
}
