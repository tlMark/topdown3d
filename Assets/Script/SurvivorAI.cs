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

    [Header("--- AI State & Settings ---")]
    public SurvivorState currentState = SurvivorState.Coward;
    public float moveSpeed = 3.5f;          // 이동 속도
    public float followDistance = 3f;       // 플레이어와 유지할 최소 거리
    public float stopDistance = 1.5f;       // 플레이어에게 가까워지면 멈출 거리
    public float attackRange = 1.0f;        // 적 공격을 위한 거리
    public float itemCollectRange = 5f;     // 아이템을 감지하고 이동할 범위
    public LayerMask itemLayer;             // 아이템 LayerMask (Inspector에서 설정)
    
    [Header("--- Scenario Target ---")]
    private Transform scenarioTarget;       // 외부에서 지정하는 시나리오 목표
    private Transform nearestItem;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        scanner = GetComponent<Scanner>();
        player = GameObject.FindGameObjectWithTag("Player");

        agent.speed = moveSpeed;
        agent.stoppingDistance = stopDistance;
    }

    void Update()
    {
        if (player == null) return;

        // Coward 상태일 때는 아무것도 하지 않음
        if (currentState == SurvivorState.Coward)
        {
            agent.isStopped = true;
            UpdateAnimation(0);
            return;
        }

        // 행동 우선순위에 따라 상태 결정
        DecideNextState();

        // 현재 상태에 따라 행동 실행
        ExecuteCurrentState();
        
        UpdateAnimation(agent.velocity.magnitude);
    }

    /// <summary>
    /// 우선순위에 따라 AI의 다음 상태를 결정합니다.
    /// </summary>
    private void DecideNextState()
    {
        // 1순위: 아이템 수집
        nearestItem = FindNearestItem();
        if (nearestItem != null)
        {
            currentState = SurvivorState.CollectItem;
            return;
        }

        // 2순위: 적 공격
        if (scanner.nearestTarget != null)
        {
            currentState = SurvivorState.AttackEnemy;
            return;
        }

        // 3순위: 시나리오 목표
        if (scenarioTarget != null)
        {
            currentState = SurvivorState.MoveToTarget;
            return;
        }

        // 4순위: 플레이어 추적
        currentState = SurvivorState.FollowPlayer;
    }

    /// <summary>
    /// 현재 상태에 맞는 행동 로직을 실행합니다.
    /// </summary>
    private void ExecuteCurrentState()
    {
        agent.isStopped = false; // 기본적으로 이동 상태로 설정

        switch (currentState)
        {
            case SurvivorState.CollectItem:
                agent.SetDestination(nearestItem.position);
                break;

            case SurvivorState.AttackEnemy:
                HandleAttack(scanner.nearestTarget);
                break;

            case SurvivorState.MoveToTarget:
                agent.SetDestination(scenarioTarget.position);
                break;

            case SurvivorState.FollowPlayer:
                HandleFollowPlayer();
                break;

            case SurvivorState.Idle:
                agent.isStopped = true;
                break;
        }
    }

    private void HandleAttack(Transform enemyTarget)
    {
        // 1. 적을 바라보도록 회전
        Vector3 lookDirection = enemyTarget.position - transform.position;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        float distanceToEnemy = Vector3.Distance(transform.position, enemyTarget.position);

        if (distanceToEnemy <= attackRange)
        {
            agent.isStopped = true; // 공격 범위 내에서는 이동 멈춤
            Debug.Log("Survivor: 적 감지! 공격 실행!");
        }
        else
        {
            agent.SetDestination(enemyTarget.position); // 적을 향해 이동
        }
    }

    private void HandleFollowPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > followDistance)
        {
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // stopDistance에 의해 자동으로 멈춤
        }
    }

    private Transform FindNearestItem()
    {
        Collider[] items = Physics.OverlapSphere(transform.position, itemCollectRange, itemLayer);
        Transform result = null;
        float diff = float.MaxValue;

        foreach (Collider item in items)
        {
            float curDiff = Vector3.Distance(transform.position, item.transform.position);
            if (curDiff < diff) { diff = curDiff; result = item.transform; }
        }
        return result;
    }

    public void SetScenarioTarget(Transform target)
    {
        scenarioTarget = target;
        Debug.Log("Survivor AI: 새로운 목표지점 " + target.name + " 추적 시작!");
    }

    public void StartFollowing()
    {
        currentState = SurvivorState.FollowPlayer; // Coward 상태에서 벗어나 바로 플레이어 추적 시작
        Debug.Log("SurvivorAI: Coward -> FollowPlayer 상태로 전환. 플레이어를 추적합니다.");
    }

    private void UpdateAnimation(float velocity)
    {
        // 애니메이션 로직 구현
    }
}