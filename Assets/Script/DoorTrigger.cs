using UnityEngine;
using UnityEngine.Events;
// DialogueSequence 구조체 사용을 위해 DialogueData.cs의 내용이 프로젝트에 포함되어 있어야 합니다.

public class DoorTrigger : MonoBehaviour
{
    [Header("--- 1. 시나리오 데이터 ---")]
    [Tooltip("트리거 진입 시 시작할 대화 시퀀스입니다. 인스펙터에서 DialogueSequence를 연결하세요.")]
    public DialogueSequence initialDialogue; 

    [Header("--- 2. 타겟 오브젝트 ---")]
    [Tooltip("대화 종료 후 사라지거나 열릴 문 오브젝트입니다.")]
    public GameObject doorObject;

    [Tooltip("문이 열린 뒤 활성화 및 추적이 시작될 Survivor 오브젝트입니다. (SurvivorAI 컴포넌트 필수)")]
    public GameObject survivorObject;

    private bool isTriggered = false;

    void Start()
    {
        // 씬 로드 시 DialogueManager의 이벤트에 함수를 자동 연결합니다.
        if (DialogueManager.instance != null)
        {
            // [핵심] DialogueManager의 onDialogueEnd 이벤트에 OpenDoorAndActivateAI 함수를 리스너로 추가합니다.
            DialogueManager.instance.onDialogueEnd.AddListener(OpenDoorAndActivateAI);
            Debug.Log("DoorTrigger: DialogueManager의 onDialogueEnd 이벤트에 성공적으로 연결되었습니다.");
        }
        else
        {
            Debug.LogError("DoorTrigger: 씬에 DialogueManager 인스턴스가 존재하지 않습니다! 이벤트 연결 실패.");
        }
    }
    
    private void OnDestroy()
    {
        // 메모리 누수 방지: 오브젝트가 파괴될 때 이벤트 리스너도 제거합니다.
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.onDialogueEnd.RemoveListener(OpenDoorAndActivateAI);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // 이미 작동했거나, 플레이어가 아니면 무시
        if (isTriggered || !other.CompareTag("Player")) return;

        isTriggered = true;
        Debug.Log("DoorTrigger: 플레이어 진입. DialogueManager.StartDialogue() 호출.");
        
        // 1. 대화 시작
        if (DialogueManager.instance != null && initialDialogue.dialogues.Length > 0)
        {
            DialogueManager.instance.StartDialogue(initialDialogue);
        }
        else
        {
            Debug.LogError("DoorTrigger: 대화 데이터가 없거나 DialogueManager가 없습니다. 바로 문을 엽니다.");
            OpenDoorAndActivateAI();
        }
    }

    /// <summary>
    /// DialogueManager의 대화 종료 신호를 받아 문을 열고 SurvivorAI의 추적을 시작시킵니다.
    /// (9, 10, 11 단계 통합 로직)
    /// </summary>
    public void OpenDoorAndActivateAI()
    {
        Debug.Log("<color=lime>DoorTrigger: [이벤트 수신] 대화 종료 신호 수신. 문을 열고 AI를 추적 상태로 전환합니다.</color>");

        // 1. 문 처리 (비활성화)
        if (doorObject != null)
        {
            // RPG/FPS Industrial Set에서 문 모델을 찾아 비활성화하여 문이 열린 것처럼 보이게 합니다.
            doorObject.SetActive(false); 
        }

        // 2. 생존자 AI 활성화 및 추적 시작
        if (survivorObject != null)
        {
            // Survivor 오브젝트가 비활성화 상태였다면 켜줍니다.
            if (!survivorObject.activeSelf)
            {
                survivorObject.SetActive(true);
            }

            // AI 상태를 Follow로 변경
            SurvivorAI survivorAI = survivorObject.GetComponent<SurvivorAI>();
            if (survivorAI != null)
            {
                // SurvivorAI의 StartFollowing 함수 호출 (10단계 목표 달성)
                survivorAI.StartFollowing(); 
            }
            else
            {
                Debug.LogError("DoorTrigger: Survivor Object에 SurvivorAI 컴포넌트가 없습니다! 추적 시작 실패.");
            }
        }
    }
}