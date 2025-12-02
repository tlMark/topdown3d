// 새 C# 파일 (예: DialogueData.cs)을 생성하거나, DialogueManager.cs 상단에 정의합니다.
using UnityEngine;

[System.Serializable]
public struct Dialogue
{
    public string speakerName; // 화자 이름 (예: "선배", "Seeker")
    public Sprite portrait;     // 화자 초상화 이미지 (선택 사항)
    [TextArea(3, 10)]
    public string dialogueText; // 실제 대사 내용
}

// 하나의 대화 시퀀스를 담을 구조체
[System.Serializable]
public struct DialogueSequence
{
    public Dialogue[] dialogues;
}