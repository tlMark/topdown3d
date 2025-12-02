using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    private CharacterController characterController;
    private InputSystem_Actions inputActions;
    public Scanner scanner;
    private Camera mainCamera;
    
    [SerializeField] private float rotationSpeed = 720f; // degrees per second
    [SerializeField] private LayerMask groundLayer;

    // ======================================================
    // 통합된 기능 변수
    // ======================================================
    [Header("# Interaction")] // 새로운 섹션 추가
    public GameObject flashlightObject;      // Inspector에서 손전등 오브젝트를 연결해주세요.
    private Scanner scannerComponent;         // 손전등 오브젝트에 부착된 Scanner 컴포넌트 (감지 범위 제어용)
    public InputAction interactionAction;     // Inspector에서 상호작용/손전등 액션을 연결해주세요.
    [SerializeField] private float interactionRange = 1.5f; // 상호작용 감지 범위
    [SerializeField] private LayerMask interactableLayer; // 상호작용 오브젝트 레이어 (에디터에서 설정)
    public IInteractable nearestInteractable; // 현재 가장 가까운 상호작용 가능한 오브젝트

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        scanner = GetComponent<Scanner>();
        inputActions = new InputSystem_Actions();

        // 손전등 오브젝트에 Scanner 컴포넌트가 있다면 초기화합니다.
        if (flashlightObject != null)
        {
            scannerComponent = flashlightObject.GetComponent<Scanner>();
            if (scannerComponent != null)
            {
                // 손전등의 초기 활성화 상태에 따라 스캐너 컴포넌트를 활성화/비활성화합니다.
                scannerComponent.enabled = flashlightObject.activeSelf;
            }
        }

        // 상호작용 액션에 이벤트 핸들러를 연결합니다.
        // 이 부분이 '상호작용 입력 처리' 로직을 Input System으로 대체합니다.
        interactionAction.performed += OnInteractionPerformed;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        interactionAction.Enable(); // 상호작용 액션 활성화
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        interactionAction.Disable(); // 상호작용 액션 비활성화
    }

    // 상호작용 입력 처리 핸들러
    private void OnInteractionPerformed(InputAction.CallbackContext context)
    {
        if (GameManager.instance == null || !GameManager.instance.isLive)
        {
            return;
        }

        // 1. 대화 진행 (최고 우선순위)
        // DialogueManager가 존재하고 대화창이 활성화되어 있으면 대사 진행
        if (DialogueManager.instance != null && DialogueManager.instance.dialoguePanel.activeSelf)
        {
            DialogueManager.instance.DisplayNextDialogue();
            return; // 대화 중에는 다른 상호작용을 막습니다.
        }

        // 2. 오브젝트 상호작용 (대화 중이 아닐 때)
        if (nearestInteractable != null)
        {
            nearestInteractable.Interact(this.gameObject); // 상호작용 요청
            return; // 상호작용을 했으면 손전등 토글은 하지 않습니다.
        }

        // 3. 손전등 켜기/끄기 (모든 우선순위가 충족되지 않았을 때)
        ToggleFlashlight();
    }

    // 손전등 켜기/끄기 기능
    private void ToggleFlashlight()
    {
        if (flashlightObject != null)
        {
            // 손전등 상태 토글
            bool isActive = !flashlightObject.activeSelf;
            flashlightObject.SetActive(isActive);

            // 손전등에 부착된 스캐너 컴포넌트 활성화/비활성화
            if (scannerComponent != null)
            {
                scannerComponent.enabled = isActive;
            }
        }
    }

    private void Update()
    {
        if (GameManager.instance == null || !GameManager.instance.isLive) return;
        HandleMovement();
        HandleRotation();
        DetectInteractable(); // 매 프레임 상호작용 오브젝트 감지
    }

    private void HandleMovement()
    {
        Vector2 input = inputActions.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 normalizedMoveDirection = moveDirection.normalized;
            characterController.Move(normalizedMoveDirection * speed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, groundLayer))
        {
            Vector3 lookDirection = hit.point - transform.position;
            lookDirection.y = 0; // 캐릭터가 위아래로 기울지 않도록 y축을 고정합니다.
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // 1. 상호작용 가능한 오브젝트 감지
    private void DetectInteractable()
    {
        // 플레이어 주변 범위 내의 Collider를 모두 탐색
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            interactionRange,
            interactableLayer
        );

        nearestInteractable = null;

        // 감지된 오브젝트 중 IInteractable 인터페이스를 가진 오브젝트를 찾습니다.
        foreach (var hitCollider in hitColliders)
        {
            IInteractable interactable = hitCollider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // [단순화] 가장 먼저 찾은 오브젝트를 가장 가까운 것으로 간주합니다.
                // (필요 시 거리를 계산하여 가장 가까운 것을 선택하도록 개선 가능)
                nearestInteractable = interactable;
                break;
            }
        }
    }
}
