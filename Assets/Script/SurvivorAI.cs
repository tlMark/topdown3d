// SurvivorAI.cs
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SurvivorAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Scanner scanner;

    
    [Header("AI 설정")]
    public float followDistance = 3f; 
    public float stopDistance = 1.5f; 
    public float moveSpeed = 3.5f;
    public float attackRange = 1.0f; // 적 공격을 위한 거리


    [Header("아이템 감지 및 수집")]
    public float itemCollectRange = 5f; // 아이템을 감지하고 이동할 범위
    public LayerMask itemLayer;         // 아이템 LayerMask (Hierarchy에서 설정)
    private Transform nearestItem;

    [Header("시나리오 목표")]
    private Transform scenarioTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        scanner = GetComponent<Scanner>(); // Scanner 컴포넌트 가져오기
        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance; 
        
        // GameManager를 통해 플레이어 Transform 가져오기 (기존 로직 유지)
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            playerTransform = GameManager.instance.player.transform;
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        // =================================
        // 1. 🚀 1순위: 아이템 수집 (최우선)
        // =================================
        nearestItem = FindNearestItem();
        if (nearestItem != null)
        {
            agent.SetDestination(nearestItem.position);
            UpdateAnimation(agent.velocity.magnitude);
            // 아이템에 도달하면 (ScrapCollectable.cs의 로직에 의해) 아이템이 파괴될 것이므로 별도 처리 불필요
            return; 
        }

        // =================================
        // 2. 🔫 2순위: 적 공격/추적
        // =================================
        if (scanner.nearestTarget != null) // Scanner가 적을 감지했다면
        {
            Transform enemyTarget = scanner.nearestTarget;

            // 1. 적을 바라보도록 회전
            Vector3 lookDirection = enemyTarget.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);

            float distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.position);
            
            if (distanceToEnemy <= attackRange)
            {
                agent.isStopped = true; // 이동 멈춤
                // ❗ 여기에 공격 로직 (예: 애니메이션, 투사체 발사)을 구현
                Debug.Log("Survivor: 적 감지! 공격 실행!");
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(enemyTarget.position); // 적을 향해 이동
            }
            UpdateAnimation(agent.velocity.magnitude);
            return; // 아이템이나 적을 쫓는 중에는 다른 로직을 수행하지 않음
        }

        // =================================
        // 3. 🗺️ 3순위: 시나리오/플레이어 추적 (기존 로직)
        // =================================
        if (scenarioTarget != null)
        {
            agent.SetDestination(scenarioTarget.position);
        }
        else // 시나리오 목표가 없을 때만 플레이어 추적
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            
            if (distanceToPlayer > followDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(playerTransform.position);
            }
            else // 플레이어에게 가까워지면 멈춤
            {
                // stoppingDistance 설정에 의해 자동으로 멈추므로 isStopped = true는 불필요
            }
        }
        
        UpdateAnimation(agent.velocity.magnitude);
    }

    // 아이템 탐지 함수 (아이템 수집 로직)
    Transform FindNearestItem()
    {
        // itemCollectRange 내의 모든 아이템 콜라이더 탐색
        Collider[] items = Physics.OverlapSphere(transform.position, itemCollectRange, itemLayer);

        Transform result = null;
        float diff = float.MaxValue;

        foreach (Collider item in items)
        {
            float curDiff = Vector3.Distance(transform.position, item.transform.position);
            if (curDiff < diff)
            {
                diff = curDiff;
                result = item.transform;
            }
        }
        return result;
    }

    // 외부에서 시나리오 목표를 설정하는 함수 (기존 로직 유지)
    public void SetScenarioTarget(Transform target)
    {
        scenarioTarget = target;
        Debug.Log("Survivor AI: 새로운 목표지점 " + target.name + " 추적 시작!");
    }
    
    void UpdateAnimation(float velocity)
    {
        // 애니메이션 로직 구현
    }
}