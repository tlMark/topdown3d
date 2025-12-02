// Door.cs (새 C# 파일)
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    private bool isOpen = false;
    private Animator animator; // 문 열림 애니메이션 컴포넌트 (선택 사항)

    void Awake()
    {
        animator = GetComponent<Animator>();
        // 문 오브젝트에 Collider 컴포넌트가 필수적으로 있어야 합니다.
    }

    public void Interact(GameObject interactor)
    {
        // 문 열기/닫기 로직 실행
        if (!isOpen)
        {
            OpenDoor();
        }
        else
        {
            // CloseDoor(); // 필요하다면 닫는 로직도 추가
        }
    }

    private void OpenDoor()
    {
        isOpen = true;
        
        // 애니메이션이 있다면 재생 (Door 애니메이터에 "Open" 트리거 설정 가정)
        if (animator != null)
        {
            animator.SetTrigger("Open");
        }
        
        Debug.Log(gameObject.name + " 문이 열렸습니다!");
        
        // [시나리오 연동] 문을 열었으므로 TutorialManager의 다음 단계로 진행 신호를 보낼 수 있습니다.
        // 예: TutorialManager.instance.CompleteStep(TutorialStep.OpenDoor);
    }

    public string GetInteractionPrompt()
    {
        return isOpen ? "닫기 (E)" : "열기 (E)";
    }
}