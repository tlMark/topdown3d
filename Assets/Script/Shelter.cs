using UnityEngine;

public class Shelter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
        {
            // 1. 충돌한 오브젝트가 플레이어인지 확인
            if (other.CompareTag("Player"))
            {
                // 2. GameManager 인스턴스가 존재하는지 확인
                if (GameManager.instance != null)
                {
                    // 3. 게임 클리어 함수 호출
                    GameManager.instance.GameClear();
                    gameObject.SetActive(false); 
                }
            }
        }
}
