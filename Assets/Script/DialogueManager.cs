// DialogueManager.cs
using UnityEngine;
using TMPro; // TMP_Text를 사용한다면 추가
using System.Collections;
using UnityEngine.Events; // UnityEvent 사용을 위해 추가

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    // UI 연결 변수
    public GameObject dialoguePanel; // 대화창 UI 전체 패널
    public TextMeshProUGUI nameText; // 화자 이름 표시용 Text
    public TextMeshProUGUI dialogueText; // 대사 내용 표시용 Text
    public float typingSpeed = 0.05f; // 글자가 출력되는 속도

    // [핵심 추가] 대화가 완전히 종료되었음을 외부에 알리는 이벤트
    public UnityEvent onDialogueEnd = new UnityEvent();

    private Dialogue[] currentDialogues;
    private int dialogueIndex;
    private bool isTyping = false;

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (instance == null)
        {
            instance = this;
            // 씬이 전환되어도 유지하려면 주석 해제: DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 중복 인스턴스 방지
            Destroy(gameObject);
            return;
        }

        // 초기에는 대화창을 닫아둡니다.
        dialoguePanel.SetActive(false);
    }

    // 대화 시작 함수
    public void StartDialogue(DialogueSequence sequence)
    {
        // 대화 시작 시, GameManager에서 isLive를 잠시 false로 설정하여 게임 일시정지
        if (GameManager.instance != null) GameManager.instance.isLive = false;
        
        dialoguePanel.SetActive(true);
        currentDialogues = sequence.dialogues;
        dialogueIndex = 0;
        // 첫 번째 대사 출력 시작
        DisplayNextDialogue();
    }

    // 다음 대사 출력 함수
    public void DisplayNextDialogue()
    {
        if (isTyping)
        {
            // 타이핑 중이면, 타이핑을 즉시 완료
            StopAllCoroutines();
            // currentDialogues[dialogueIndex-1]를 사용해야 현재 출력 중인 대사를 완료할 수 있습니다.
            // 이미 DisplayNextDialogue 시작 시점에 dialogueIndex++가 되기 때문입니다.
            dialogueText.text = currentDialogues[dialogueIndex - 1].dialogueText; 
            isTyping = false;
            return;
        }

        if (dialogueIndex >= currentDialogues.Length)
        {
            EndDialogue();
            return;
        }

        Dialogue dialogue = currentDialogues[dialogueIndex];

        nameText.text = dialogue.speakerName;
        // 초상화 로직 추가 (필요하다면)

        StartCoroutine(TypeSentence(dialogue.dialogueText));
        
        dialogueIndex++;
    }

    // 대화 종료 함수 (추가 및 수정)
    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        
        // 대화 종료 시, GameManager에서 isLive를 다시 true로 설정하여 게임 재개
        if (GameManager.instance != null) GameManager.instance.isLive = true;

        // [핵심] 이벤트를 발생시켜 DoorTrigger와 같은 외부 스크립트에 알립니다.
        Debug.Log("<color=yellow>DialogueManager: 대화가 종료되었습니다. Event 발생!</color>");
        onDialogueEnd.Invoke();
    }

    // 대사를 한 글자씩 출력하는 코루틴
    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = ""; // 초기화

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}