using System;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rectTransform;
    Item[] items;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        Next();

        rectTransform.localScale = Vector3.one;

        GameManager.instance.Stop();
        AudioManger.instance.PlaySfx(AudioManger.SFX.LevelUp);
        AudioManger.instance.HighPassBgm(true);
    }

    public void Hide()
    {
        rectTransform.localScale = Vector3.zero;

        GameManager.instance.Resume();
        AudioManger.instance.PlaySfx(AudioManger.SFX.Select);
        AudioManger.instance.HighPassBgm(false);
    }

    public void Select(int index)
    {
        items[index].OnClick();
    }

    void Next()
    {
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        int[] select = new int[3];
        
        while (true)
        {
            select[0] = UnityEngine.Random.Range(0, items.Length);
            select[1] = UnityEngine.Random.Range(0, items.Length);
            select[2] = UnityEngine.Random.Range(0, items.Length);

            if (select[0] != select[1] && select[1] != select[2] && select[2] != select[0])
            {
                break;
            }
        }

        for (int i = 0; i < select.Length; i++)
        {
            Item selectItem = items[select[i]];

            if (selectItem.level == selectItem.itemData.damages.Length)
            {
                items[4].gameObject.SetActive(true);
            }
            else
            {
             selectItem.gameObject.SetActive(true);   
            }
        }
    }
}
