using UnityEngine;
using UnityEngine.InputSystem; // Input System 네임스페이스 사용

/// <summary>
/// 플레이어의 Raw 입력을 처리하는 컴포넌트입니다.
/// PlayerInput 컴포넌트의 메시지 시스템을 통해 입력 이벤트를 받습니다.
/// </summary>
public class PlayerInputController : MonoBehaviour
{
    // 외부에서 접근 가능한 입력 벡터 (Vector2 -> Vector3로 변환하여 노출)
    public Vector3 moveInput { get; private set; }
    
    // 상호작용 입력 (버튼을 눌렀을 때만 true가 됨)
    public bool isInteractPressed { get; private set; }
    
    // 손전등 토글 입력 (버튼을 눌렀을 때만 true가 됨)
    public bool isFlashlightPressed { get; private set; }
    
    // 대화 진행 입력 (버튼을 눌렀을 때만 true가 됨)
    public bool isDialogueNextPressed { get; private set; }

    /// <summary>
    /// Input System의 'Move' 액션에 의해 호출됩니다.
    /// </summary>
    /// <param name="value">Move 액션의 Vector2 값</param>
    public void OnMove(InputValue value)
    {
        if (GameManager.instance != null && !GameManager.instance.isLive)
        {
            moveInput = Vector3.zero;
            return;
        }

        // Vector2 입력을 Vector3로 변환 (Y축은 0으로 고정, Z축이 전후 이동)
        Vector2 inputVec2 = value.Get<Vector2>();
        moveInput = new Vector3(inputVec2.x, 0, inputVec2.y).normalized;
    }
    
    /// <summary>
    /// Input System의 'Interact' 액션에 의해 호출됩니다.
    /// </summary>
    public void OnInteract(InputValue value)
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        
        // 버튼을 눌렀을 때(Press)만 true로 설정 (누르는 순간 한 번만 true가 됨)
        isInteractPressed = value.isPressed;
    }
    
    /// <summary>
    /// Input System의 'Flashlight' 액션에 의해 호출됩니다.
    /// </summary>
    public void OnFlashlight(InputValue value)
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        
        // 버튼을 눌렀을 때(Press)만 true로 설정 (누르는 순간 한 번만 true가 됨)
        isFlashlightPressed = value.isPressed;
    }
    
    /// <summary>
    /// Input System의 'DialogueNext' 액션에 의해 호출됩니다.
    /// </summary>
    public void OnDialogueNext(InputValue value)
    {
        if (GameManager.instance != null && !GameManager.instance.isLive) return;
        
        // 버튼이 눌렸고, 대화창이 활성화되어 있을 때만 대화 진행
        if (value.isPressed && DialogueManager.instance != null && DialogueManager.instance.dialoguePanel.activeSelf)
        {
            DialogueManager.instance.DisplayNextDialogue();
        }
    }
    
    // =========================================================================
    // [추가] Player.cs에서 입력을 소비(Consume)할 수 있도록 공개 메서드 추가
    // =========================================================================

    /// <summary>
    /// 상호작용 입력을 소비하고 false로 리셋합니다.
    /// </summary>
    public void ConsumeInteract()
    {
        isInteractPressed = false;
    }

    /// <summary>
    /// 손전등 입력을 소비하고 false로 리셋합니다.
    /// </summary>
    public void ConsumeFlashlight()
    {
        isFlashlightPressed = false;
    }
}