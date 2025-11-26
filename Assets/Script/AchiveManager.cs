using System;
using System.Collections;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacters;
    public GameObject[] unlockCharacters;
    public GameObject uiNotice;

    enum AchiveType
    {
        Unlock1, Unlock2, Unlock3
    }

    AchiveType[] achives;

    WaitForSecondsRealtime waitTime;

    void Awake()
    {
        achives = (AchiveType[])Enum.GetValues(typeof(AchiveType));

        waitTime = new WaitForSecondsRealtime(5f);

        if (!PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    void Start()
    {
        UnlockCharacters();
    }

    void LateUpdate()
    {
        foreach (AchiveType achive in achives)
        {
            CheckAchive(achive);
        }
    }

    void Init()
    {
        PlayerPrefs.SetInt("MyData", 1);

        foreach (AchiveType achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 0);
        }
    }

    void UnlockCharacters()
    {
        for (int i = 0; i < lockCharacters.Length; i++)
        {
            string achiveName = achives[i].ToString();
            
            bool isUnlocked = PlayerPrefs.GetInt(achiveName) == 1;
            lockCharacters[i].SetActive(!isUnlocked);
            unlockCharacters[i].SetActive(isUnlocked);
        }
    }

    void CheckAchive(AchiveType achive)
    {
        bool isAchived = false;

        switch (achive)
        {
            case AchiveType.Unlock1:
                isAchived = GameManager.instance.kill >= 10;
                break;
            case AchiveType.Unlock2:
                isAchived = GameManager.instance.kill >= 20;
                break;
            case AchiveType.Unlock3:
                isAchived = GameManager.instance.kill >= 30;
                break;
        }
        //isAchived = GameManager.instance.gameTime == GameManager.instance.maxGameTime;
        //이건 시간으로 업적 달성했을 때 해금된다는 의미의 코드

        if (isAchived && PlayerPrefs.GetInt(achive.ToString()) == 0)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);
            UnlockCharacters();

            for (int i = 0; i < uiNotice.transform.childCount; i++)
            {
                bool isAchivedNotice = i == (int)achive;

                uiNotice.transform.GetChild(i).gameObject.SetActive(isAchivedNotice);
            }

            StartCoroutine(ShowNotice());
        }
    }

    IEnumerator ShowNotice()
    {
        uiNotice.SetActive(true);
        AudioManger.instance.PlaySfx(AudioManger.SFX.LevelUp);

        yield return waitTime;

        uiNotice.SetActive(false);
    }
}
