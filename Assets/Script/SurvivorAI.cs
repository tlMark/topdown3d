// SurvivorAI.cs
using UnityEngine;
using UnityEngine.AI;

// 생존자 AI의 주요 상태 정의
public enum SurvivorState
{
    Coward,         // 1. 겁먹고 제자리에 서 있음 (시나리오 시작 시)
    Idle,           // 2. 특별한 목표 없이 대기 (플레이어 추적 전)
    FollowPlayer,   // 3. 플레이어 추적
    AttackEnemy,    // 4. 적 공격
    CollectItem,    // 5. 아이템 수집
    MoveToTarget    // 6. 지정된 목표로 이동
}

[RequireComponent(typeof(NavMeshAgent))]
public class SurvivorAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private Scanner scanner;
    
    // 회전 속도 (추가)
    [Tooltip("공격 시 적을 바라보는 회전 속도")]
    public float rotationSpeed = 10f;
    
    [Header("--- AI State & Settings ---")]
    public SurvivorState currentState = SurvivorState.Coward; // 시나리오 시작 상태
    public float moveSpeed = 3.5f;          // 이동 속도
    public float followDistance = 3f;       // 플레이어와 유지할 최소 거리
    public float stopDistance = 1.5f;       // 플레이어에게 가까워지면 멈출 거리
    public float attackRange = 1.0f;        // 적 공격을 위한 거리
    public float itemCollectRange = 5f;     // 아이템을 감지하고 이동할 범위
    public LayerMask itemLayer;             // 아이템 LayerMask (Inspector에서 설정)
    
    [Header("--- Scenario Target ---")]
    private Transform scenarioTarget;       // 시나리오 이동 목표 (현재 튜토리얼에서는 미사용)
    private Transform enemyTarget;          // 스캐너로 감지된 적
    private Transform itemTarget;           // 스캐너로 감지된 아이템

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        scanner = GetComponent<Scanner>();
        
        // GameManager를 통해 플레이어 Transform 가져오기 (기존 로직 유지)
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player.gameObject;
        }
        else
        {
            Debug.LogError("Survivor AI: GameManager 또는 Player를 찾을 수 없습니다. (테스트 환경 오류)");
        }

        agent.speed = moveSpeed;
        // NavMeshAgent의 stoppingDistance는 플레이어 추적과 공격 대상 추적 모두에 사용됩니다.
        // 플레이어 추적 시 멈추는 거리(stopDistance)를 기본으로 설정합니다.
        agent.stoppingDistance = stopDistance; 
    }
    
    void FixedUpdate()
    {
        // 게임 오버 상태이거나 플레이어 참조가 없으면 AI 로직을 실행하지 않습니다.
        if (player == null || GameManager.instance == null || !GameManager.instance.isLive) return;

        // [핵심 로직] 상태에 따른 행동 결정
        switch (currentState)
        {
            case SurvivorState.Coward:
                HandleCoward(); // 꼼짝 않고 대기
                break;

            case SurvivorState.FollowPlayer:
                // 1. 최우선 순위: 적 감지 및 공격
                // Scanner는 자동으로 가장 가까운 적을 찾습니다.
                enemyTarget = scanner.nearestTarget; 
                if (enemyTarget != null)
                {
                    currentState = SurvivorState.AttackEnemy;
                    agent.stoppingDistance = attackRange; // 적 공격 거리를 정지 거리로 설정
                    break;
                }
                
                // 2. 두 번째 우선순위: 아이템 감지 및 수집
                itemTarget = FindNearestItem();
                if (itemTarget != null)
                {
                    currentState = SurvivorState.CollectItem;
                    // 아이템 수집 시에는 아이템에 근접해야 하므로 정지 거리를 0으로 설정하거나 매우 작게 설정합니다.
                    agent.stoppingDistance = 0.5f; 
                    break;
                }
                
                // 3. 기본 행동: 플레이어 추적
                HandleFollowPlayer();
                agent.stoppingDistance = stopDistance; // 플레이어 추적 거리를 정지 거리로 설정
                break;
            
            case SurvivorState.AttackEnemy:
                // 적이 사라지면(처치되면) 다시 추적 상태로 복귀
                if (enemyTarget == null)
                {
                    currentState = SurvivorState.FollowPlayer;
                    break;
                }
                HandleAttackEnemy();
                break;
                
            case SurvivorState.CollectItem:
                // 아이템이 사라지면(수집되면) 다시 추적 상태로 복귀
                if (itemTarget == null)
                {
                    currentState = SurvivorState.FollowPlayer;
                    break;
                }
                HandleCollectItem();
                break;

            // 기타 상태는 현재 시나리오에서 미사용
            case SurvivorState.Idle:
            case SurvivorState.MoveToTarget:
                agent.isStopped = true;
                break;
        }
        
        UpdateAnimation(agent.velocity.magnitude);
    }
    
    // ------------------------------------------------------------------
    // 상태별 행동 함수
    // ------------------------------------------------------------------

    private void HandleCoward()
    {
        // Coward 상태에서는 꼼짝하지 않습니다.
        agent.isStopped = true;
    }

    private void HandleCollectItem()
    {
        // 아이템이 유효한지 다시 확인
        if (itemTarget == null)
        {
            currentState = SurvivorState.FollowPlayer;
            return;
        }

        // 아이템으로 이동
        agent.isStopped = false;
        agent.SetDestination(itemTarget.position);
    }

    private void HandleAttackEnemy()
    {
        if (enemyTarget == null)
        {
            currentState = SurvivorState.FollowPlayer;
            return;
        }

        float distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.position);

        // 1. 공격 범위 안에 있으면 멈추고 공격 실행
        if (distanceToEnemy <= attackRange)
        {
            agent.isStopped = true; // 적 공격 범위 내에서는 이동 멈춤
            
            // 적을 향해 부드럽게 회전 (추가 로직)
            Vector3 lookDirection = enemyTarget.position - transform.position;
            lookDirection.y = 0; // Y축 고정
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            
            Debug.Log("Survivor: 적 감지! 공격 실행! (12단계에서 Weapon 연동 예정)");
            
            // TODO: [12단계] Weapon.cs를 통한 실제 공격 로직 호출
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(enemyTarget.position); // 적을 향해 이동
        }
    }

    private void HandleFollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        agent.isStopped = false;

        // 플레이어와 followDistance 이상 떨어져 있으면 플레이어에게 이동합니다.
        if (distanceToPlayer > followDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // followDistance 내에 진입하면 agent.stoppingDistance(stopDistance)에 의해 자동으로 멈춥니다.
            // 추가적인 SetDestination이나 isStopped = true/false 로직은 필요 없습니다.
        }
    }

    // 아이템 탐지 함수 (아이템 수집 로직)
    private Transform FindNearestItem()
    {
        // itemCollectRange 내의 모든 아이템 콜라이더 탐색
        Collider[] items = Physics.OverlapSphere(transform.position, itemCollectRange, itemLayer);

        Transform result = null;
        float diff = float.MaxValue;

        foreach (Collider item in items)
        {
            // ScarpCollectable 또는 ShelterKeyItem과 같은 수집 가능한 아이템인지 확인 (개선된 로직)
            // 이를 통해 AI가 엉뚱한 오브젝트를 목표로 삼는 것을 방지합니다.
            if (item.GetComponent<ScrapCollectable>() || item.GetComponent<ShelterKeyItem>())
            {
                float curDiff = Vector3.Distance(transform.position, item.transform.position);
                if (curDiff < diff)
                {
                    diff = curDiff;
                    result = item.transform;
                }
            }
        }
        return result;
    }

    // 외부에서 시나리오 목표를 설정하는 함수 (현재 튜토리얼 시나리오에서는 미사용)
    public void SetScenarioTarget(Transform target)
    {
        scenarioTarget = target;
        currentState = SurvivorState.MoveToTarget;
        agent.stoppingDistance = 0f; // 목표 지점까지 완전히 이동하도록 설정
        Debug.Log("Survivor AI: 새로운 목표지점 " + target.name + " 추적 시작!");
    }

    /// <summary>
    /// [핵심] DoorTrigger.cs에서 좀비 처치 후 호출되어 AI 상태를 변경합니다.
    /// </summary>
    public void StartFollowing()
    {
        if (currentState == SurvivorState.Coward)
        {
            currentState = SurvivorState.FollowPlayer;
            agent.isStopped = false; // NavMeshAgent 다시 활성화
            Debug.Log("<color=lime>Survivor AI: 구출 완료! 상태를 FollowPlayer로 변경합니다.</color>");
        }
    }

    private void UpdateAnimation(float velocity)
    {
        // 애니메이션 로직 구현: Animator.SetFloat("Speed", velocity) 등
    }
}