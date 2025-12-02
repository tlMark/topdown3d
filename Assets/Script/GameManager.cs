using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 2 * 10f;

    [Header("# Player Info")]
    public float health;
    public int playerId;
    public int maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 7, 14, 21, 28, 35, 42, 49, 56, 63, 70 };

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUp uiLevelUp;
    public GameResult uiResult;
    public GameObject enemyCleaner;
    public Transform uiJoystick;
    public GameObject shelterObject;

    void Awake()
    {
        instance = this;
    }
    
    void Update()
    {
        if (!isLive)
        {
            return;
        }

        gameTime += Time.deltaTime;

        if (gameTime >= maxGameTime)
        {
            gameTime = maxGameTime;
            if (shelterObject != null && !shelterObject.activeSelf)
            {
                shelterObject.SetActive(true);
                Debug.Log("쉘터 문이 열렸습니다! 탈출하세요!");
            }
        }
    }

    public void GameStart(int id)
    {
        health = maxHealth;
        level = 1;
        exp = 0;

        playerId = id;
        player.gameObject.SetActive(true);
        uiLevelUp.Select(playerId % 2);

        Resume();

        AudioManger.instance.PlayBgm(true);
        AudioManger.instance.PlaySfx(AudioManger.SFX.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        isLive = false;

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.ShowResultLose();
        Stop();

        AudioManger.instance.PlayBgm(false);
        AudioManger.instance.PlaySfx(AudioManger.SFX.Lose);
    }

    public void GameClear()
    {
        StartCoroutine(GameClearCoroutine());
    }

    IEnumerator GameClearCoroutine()
    {
        isLive = false;
        enemyCleaner.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        uiResult.gameObject.SetActive(true);
        uiResult.ShowResultWin();
        Stop();

        AudioManger.instance.PlayBgm(false);
        AudioManger.instance.PlaySfx(AudioManger.SFX.Win);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }


    public void GetExp()
    {
        if (!isLive)
        {
            return;
        }

        exp++;

        if (level < nextExp.Length)
        {
        // 다음 레벨에 도달했는지 확인 (현재 level을 인덱스로 사용)
            if (exp >= nextExp[level - 1])
            {
                level++;
                exp = 0;
                uiLevelUp.Show();
            }
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0f;
        uiJoystick.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1f;
        uiJoystick.localScale = Vector3.one;
    }
}
