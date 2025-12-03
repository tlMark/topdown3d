using UnityEngine;

// 이 스크립트는 시나리오 열쇠 오브젝트에 부착됩니다.
[RequireComponent(typeof(Collider))] 
public class ShelterKeyItem : MonoBehaviour
{
    [Header("--- 자석 시스템 속성 (ScrapCollectable 로직 차용) ---")]
    private Transform playerTransform;
    public float magnetRange = 5f;      // 자석 효과가 발동되는 범위 (기존 스크랩보다 넓게 설정)
    public float attractionSpeed = 15f; // 플레이어에게 끌어당겨지는 속도
    public float collectionRange = 1.5f; // 아이템이 수집되는 최종 거리

    [Header("--- 시나리오 연동 속성 ---")]
    [Tooltip("키 획득 시 활성화될 Shelter Trigger Zone 오브젝트입니다.")]
    public GameObject shelterTriggerZone;
    
    void Start()
    {
        // GameManager를 통해 Player의 Transform을 가져옵니다.
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            playerTransform = GameManager.instance.player.transform;
        }
        else
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다. ShelterKeyItem.cs 오류!");
            enabled = false; 
            return;
        }
        
        // 시나리오 흐름상 이 키는 DoorTrigger에서 활성화되므로, Start에서 비활성화하지 않습니다.
        // 대신 Inspector에서 수동으로 비활성화하거나, DoorTrigger가 스폰하도록 합니다.
    }

    void Update()
    {
        // GameManager가 Live 상태일 때만 자석 로직 실행
        if (playerTransform == null || !GameManager.instance.isLive) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 1. 자석 범위 확인 (magnetRange)
        if (distance <= magnetRange)
        {
            // 2. 플레이어 방향 계산 및 Lerp를 사용한 부드러운 이동 (자석 효과)
            // Lerp 계산 시 거리가 0이 되는 것을 방지하기 위해 Mathf.Max(distance, 0.1f) 사용
            transform.position = Vector3.Lerp(
                transform.position, 
                playerTransform.position, 
                attractionSpeed * Time.deltaTime / Mathf.Max(distance, 0.1f) 
            );

            // 3. 수집 범위 확인 (collectionRange)
            if (distance <= collectionRange)
            {
                CollectItem();
            }
        }
    }

    private void CollectItem()
    {
        Debug.Log("<color=yellow>ShelterKeyItem: Shelter 열쇠 획득! Shelter Trigger Zone을 활성화합니다.</color>");
        
        // 1. Shelter Trigger Zone 활성화 (시나리오 목표 달성)
        if (shelterTriggerZone != null)
        {
            shelterTriggerZone.SetActive(true);
        }
        else
        {
            Debug.LogError("ShelterKeyItem: Shelter Trigger Zone 오브젝트가 Inspector에 연결되지 않았습니다.");
        }
        
        // 2. 자기 자신 제거 (아이템 획득 완료)
        Destroy(gameObject);
    }
}