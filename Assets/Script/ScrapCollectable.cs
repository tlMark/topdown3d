// ScrapCollectable.cs (새 C# 파일)
using UnityEngine;

// 이 스크립트는 실제로 월드에 드롭되어 플레이어가 수집하는 아이템 오브젝트에 부착됩니다.
[RequireComponent(typeof(Collider))] 
public class ScrapCollectable : MonoBehaviour
{
    // [자석 시스템 속성]
    private Transform playerTransform;
    public float magnetRange = 3f;      // 자석 효과가 발동되는 범위
    public float attractionSpeed = 10f; // 플레이어에게 끌어당겨지는 속도
    public float collectionRange = 1.5f; // 아이템이 수집되는 최종 거리
    
    // 이 스크랩이 어떤 아이템을 대표하는지에 대한 ID (옵션)
    public int itemID = 0; 
    
    void Start()
    {
        // GameManager를 통해 Player의 Transform을 가져옵니다.
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            playerTransform = GameManager.instance.player.transform;
        }
        else
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다. ScrapCollectable.cs 오류!");
            enabled = false; 
        }
    }

    void Update()
    {
        // GameManager가 Live 상태일 때만 자석 로직 실행
        if (playerTransform == null || !GameManager.instance.isLive) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 1. 자석 범위 확인 (magnetRange)
        if (distance <= magnetRange)
        {
            // 2. 플레이어 방향 계산 및 Lerp를 사용한 부드러운 이동
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
        // GameManager의 GetExp() 함수를 호출하여 경험치 획득 로직을 실행합니다.
        if (GameManager.instance != null)
        {
            GameManager.instance.GetExp(); 
        }
        
        Debug.Log("아이템 수집 완료: " + gameObject.name + " (XP 획득)");
        
        // 아이템 오브젝트 파괴
        Destroy(gameObject); 
    }
}