using UnityEngine;

public interface IInteractable
{
    // 상호작용이 일어났을 때 실행될 함수
    void Interact(GameObject interactor);
    // 상호작용 가능 여부 (UI 표시 등을 위해 사용)
    string GetInteractionPrompt();
}
