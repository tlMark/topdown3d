using UnityEngine;
using UnityEngine.InputSystem; // PlayerInput 컴포넌트를 사용하기 위해 필요

/// <summary>
/// 플레이어의 최상위 컨트롤러이자 상태 관리 및 컴포넌트 연결 허브입니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInputController))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInput))] // Input System 사용을 위해 추가
public class Player : MonoBehaviour
{
    // 연결된 컴포넌트
    private PlayerInputController inputController;
    private PlayerMovement movement;
    
    // [기존 기능 복원] 손전등 연결
    [Header("--- Functional Components ---")]
    [Tooltip("손전등 역할을 하는 Light 컴포넌트를 연결하세요.")]
    public Light flashlight; 
    
    // [회전 기능 복원] 마우스 회전을 위한 변수
    [Header("--- Rotation Settings ---")]
    [Tooltip("플레이어 회전을 위한 바닥 레이어 (Terrain, Floor 등)")]
    public LayerMask groundLayer; 
    
    // 상호작용 관련 변수
    private Collider[] interactionOverlap = new Collider[5]; 
    private int interactionLayerMask; 

    void Awake()
    {
        // 컴포넌트 참조
        inputController = GetComponent<PlayerInputController>();
        movement = GetComponent<PlayerMovement>();
        
        // 상호작용 레이어 마스크 설정 
        interactionLayerMask = LayerMask.GetMask("Interactable"); 
        
        // 손전등 초기 상태 설정 (시작 시 꺼짐)
        if (flashlight != null)
        {
            flashlight.enabled = false;
        }
        
        // 마우스 커서를 숨기고 잠급니다 (탑다운 슈터의 일반적인 설정)
        Cursor.lockState = CursorLockMode.Confined; // 화면을 벗어나지 않게 유지
        Cursor.visible = true; // 커서 보이게 설정 (조준용)
    }

    void Update()
    {
        // 게임 상태 체크
        if (GameManager.instance == null || !GameManager.instance.isLive) return;

        // 1. 이동 및 회전 로직 (마우스 커서 방향 회전)
        HandleMovementAndRotation();
        
        // 2. 상호작용 로직
        if (inputController.isInteractPressed)
        {
            HandleInteraction(); 
            // [수정] 입력을 처리했으므로 명시적으로 소비하여 다음 프레임에 중복 실행 방지
            inputController.ConsumeInteract();
        }
        
        // 3. 손전등 토글 로직
        if (inputController.isFlashlightPressed)
        {
            ToggleFlashlight();
            // [수정] 입력을 처리했으므로 명시적으로 소비하여 다음 프레임에 중복 실행 방지
            inputController.ConsumeFlashlight();
        }
    }

    private void HandleMovementAndRotation()
    {
        Vector3 moveInput = inputController.moveInput;
        
        // [핵심] 마우스 커서 위치를 향하는 방향 계산
        Vector3 lookDirection = GetMouseLookDirection();

        // 이동 및 회전 실행
        movement.MoveAndRotate(moveInput, lookDirection);
    }
    
    /// <summary>
    /// 마우스 커서 위치를 Raycast하여 플레이어와의 방향 벡터를 반환합니다.
    /// (마우스 커서 기반 회전 로직)
    /// </summary>
    private Vector3 GetMouseLookDirection()
    {
        if (Camera.main == null) return Vector3.zero;

        // 마우스 위치에서 카메라를 통해 씬으로 Ray를 발사
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        // Raycast를 바닥 레이어(groundLayer)에만 실행
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            Vector3 direction = targetPosition - transform.position;
            
            // Y축 회전만 사용하기 위해 Y를 0으로 고정하고 정규화
            direction.y = 0;
            return direction.normalized;
        }
        
        return Vector3.zero; 
    }

    /// <summary>
    /// 손전등을 켜거나 끕니다.
    /// </summary>
    private void ToggleFlashlight()
    {
        if (flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
            Debug.Log($"손전등 토글: {(flashlight.enabled ? "ON" : "OFF")}");
        }
        else
        {
            Debug.LogWarning("손전등 Light 컴포넌트가 연결되지 않았습니다! 인스펙터에서 Light 컴포넌트를 연결해주세요.");
        }
    }

    /// <summary>
    /// 상호작용 로직 (HandleInteraction은 Door, Key 등 IInteractable 구현체를 실행합니다.)
    /// </summary>
    private void HandleInteraction()
    {
        // 플레이어 근처의 상호작용 가능한 오브젝트를 검색
        int numFound = Physics.OverlapSphereNonAlloc(transform.position, 2.0f, interactionOverlap, interactionLayerMask);

        if (numFound > 0)
        {
            IInteractable nearestInteractable = null;
            float shortestDistance = float.MaxValue;

            for (int i = 0; i < numFound; i++)
            {
                IInteractable interactable = interactionOverlap[i].GetComponent<IInteractable>();
                
                if (interactable != null)
                {
                    float distance = Vector3.Distance(transform.position, interactionOverlap[i].transform.position);
                    
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestInteractable = interactable;
                    }
                }
            }
            
            // 상호작용 실행
            if (nearestInteractable != null)
            {
                nearestInteractable.Interact(gameObject); 
                Debug.Log($"상호작용 실행: {((MonoBehaviour)nearestInteractable).gameObject.name}");
            }
        }
    }
}