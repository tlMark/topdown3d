using UnityEngine;
// Zombie Health 및 Damage 로직이 있다고 가정합니다.

public class ScenarioZombie : MonoBehaviour
{
    // 이 좀비가 죽었는지 여부
    private bool isDead = false;
    
    // 이 좀비를 비활성화하여 숨겨둡니다. (DoorTrigger에서 활성화)
    void Awake()
    {
        gameObject.SetActive(false); 
    }

    // 외부에서 데미지를 받는 함수 (Player Weapon.cs와 연동될 부분)
    public void TakeDamage(int damage)
    {
        // TODO: 실제 체력 감소 로직 구현... 
        
        // 시나리오 단순화를 위해 한 번의 공격으로 죽는다고 가정
        if (!isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("<color=red>ScenarioZombie: 좀비 처치! 문 개방 신호를 보냅니다.</color>");
        
        // 1. 글로벌 이벤트 발생 (DoorTrigger가 이 신호를 기다리고 있음)
        ScenarioEvents.OnScenarioZombieKilled.Invoke();
        
        // 2. 오브젝트 처리 (사망 애니메이션 후 제거 또는 비활성화)
        gameObject.SetActive(false);
    }
    
    // 플레이어가 총을 쐈을 때 Raycast가 이 함수를 호출하도록 구현해야 합니다.
}