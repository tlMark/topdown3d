using UnityEngine.Events;

/// <summary>
/// 시나리오의 주요 흐름을 관리하기 위한 글로벌 이벤트 컬렉션입니다.
/// </summary>
public static class ScenarioEvents
{
    // 1. 플레이어가 오른쪽 좀비를 처치했을 때 호출됩니다.
    // DoorTrigger가 이 이벤트를 구독하여 문을 엽니다.
    public static UnityEvent OnScenarioZombieKilled = new UnityEvent();

    // 2. 플레이어가 Shelter Key를 획득했을 때 호출됩니다.
    // GameManager가 이 이벤트를 구독하여 Shelter Door를 활성화합니다.
    public static UnityEvent OnShelterKeyCollected = new UnityEvent();
}