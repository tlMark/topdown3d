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
    
    // 플레이어가 트리거 영역에 진입했을 때
    private void OnTriggerEnter(Collider other)
    {
        // 이미 트리거되었거나 충돌한 오브젝트가 플레이어가 아니면 리턴
        if (isTriggered || !other.CompareTag("Player"))
        {
            return;
        }

        // 1. 딱 한 번만 실행되도록 플래그 설정
        isTriggered = true;

        // 2. 대화 시작
        StartDialogueSequence();
    }
    
    // 대화 시퀀스 시작
    private void StartDialogueSequence()
    {
        // 대화 데이터가 유효하고 DialogueManager가 준비되었는지 확인
        if (DialogueManager.instance != null && initialDialogue.dialogues != null && initialDialogue.dialogues.Length > 0)
        {
            DialogueManager.instance.StartDialogue(initialDialogue);
        }
        else
        {
            // 대화가 없거나 오류가 있으면, 대화 없이 바로 문을 엽니다.
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
                survivorAI.StartFollowing(); // Coward 상태를 해제하고 FollowPlayer로 전환하는 함수
            }
            else
            {
                Debug.LogError("DoorTrigger: Survivor 오브젝트에 SurvivorAI 컴포넌트가 없습니다!");
            }
        }
        
        // 문이 열리고 생존자가 구출되었으므로, 트리거 자체는 제거할 필요가 있습니다.
        // 이 스크립트가 부착된 오브젝트를 비활성화합니다.
        gameObject.SetActive(false); 
    }
    
    // 씬이 파괴될 때 이벤트 리스너를 제거하여 메모리 누수를 방지합니다.
    private void OnDestroy()
    {
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.onDialogueEnd.RemoveListener(OpenDoorAndActivateAI);
            Debug.Log("DoorTrigger: onDialogueEnd 이벤트 리스너 제거 완료.");
        }
    }
}