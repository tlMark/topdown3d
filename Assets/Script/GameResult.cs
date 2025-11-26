using UnityEngine;

public class GameResult : MonoBehaviour
{
    public GameObject[] resultsTitles;

    public void ShowResultLose()
    {
        resultsTitles[0].SetActive(true);
    }

    public void ShowResultWin()
    {
        resultsTitles[1].SetActive(true);
    }
}
