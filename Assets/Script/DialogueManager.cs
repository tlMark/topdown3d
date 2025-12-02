// DialogueManager.cs
using UnityEngine;
using TMPro; // TMP_Text를 사용한다면 추가
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    // UI 연결 변수
    public GameObject dialoguePanel; // 대화창 UI 전체 패널
    public TextMeshProUGUI nameText; // 화자 이름 표시용 Text
    public TextMeshProUGUI dialogueText; // 대사 내용 표시용 Text
    public float typingSpeed = 0.05f; // 글자가 출력되는 속도

    private Dialogue[] currentDialogues;
    private int dialogueIndex;
    private bool isTyping = false;

    void Awake()
    {
        instance = this;
        // 초기에는 대화창을 닫아둡니다.
        dialoguePanel.SetActive(false);
    }

    // 대화 시작 함수
    public void StartDialogue(DialogueSequence sequence)
    {
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
            dialogueText.text = currentDialogues[dialogueIndex].dialogueText;
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

    // 대화 종료 함수
    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        // [중요] 시나리오 흐름 제어를 위해 TutorialManager에 이벤트나 신호를 보낼 수 있습니다.
        // 예: TutorialManager.instance.NextStep(); 
    }
}