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

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        scanner = GetComponent<Scanner>();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
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
}
