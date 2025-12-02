// DialogueTest.cs
using UnityEngine;

// DialogueData.cs 파일의 구조체를 사용하기 위해 이 파일도 using UnityEngine; 를 사용합니다.
// Unity 에디터에서 DialogueSequence 구조체 전체를 설정할 수 있게 해줍니다.
public class DialogueTest : MonoBehaviour
{
    // [1] DialogueSequence 변수 선언: 여기에 대화 내용을 입력합니다.
    public DialogueSequence initialDialogue;

    // 테스트를 위해 임시로 Start()에서 호출합니다.
    void Start()
    {
        // GameManager에서 isLive가 true가 된 후 호출하는 것이 좋습니다.
        // 여기서는 테스트를 위해 단순 Start()에 둡니다.
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.StartDialogue(initialDialogue);
        }
    }
}