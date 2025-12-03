using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count; // 이 값이 Bullet.cs의 per(관통 횟수)로 사용됩니다.
    public float speed; // 공격 속도 (초당 공격 횟수가 아닌, 다음 발사까지 걸리는 시간. 낮을수록 빠름)
    float timer;

    Player player;

    void Awake()
    {
        // GameManager를 통해 Player 인스턴스 참조
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            player = GameManager.instance.player;
        }
        else
        {
            Debug.LogError("GameManager 또는 Player 인스턴스가 초기화되지 않았습니다.");
            enabled = false; // 컴포넌트 비활성화
        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (player == null || !GameManager.instance.isLive)
        {
            return;
        }

        // [핵심 로직] 공격 타이머 증가
        timer += Time.deltaTime;

        // 공격 속도(speed)에 따라 총알 발사
        if (timer > speed)
        {
            timer = 0f;
            Fire();
        }
    }

    public void Init()//원래는 ItemData data에서 정보를 가져오지만 지금은 아니도록 수정
    {
        // 임시 기본 스탯 설정 (ItemData가 없으므로)
        // 실제 게임에서는 ItemData를 통해 이 값들을 설정해야 합니다.
        id = 0; // 예시 ID
        damage = 10f; // 기본 데미지
        count = 1; // 관통력 (per) - 기본 1
        prefabId = 0; // PoolManager의 Bullet 프리팹 ID를 가정

        switch (id)
        {
            case 0: // 기본 무기
                speed = 0.5f; // 0.5초당 1발 발사
                // speed = 0.5f * Character.WeaponRate; // Character 스탯이 있다면 사용
                break;
            default:                
                speed = 1f;
                break;
        }
    }
    
    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        // 레벨업 시 다른 로직이 필요하다면 여기에 추가
        // 예: 발사 속도 변경, 총알 프리팹 변경 등
    }

    // 무기 발사 로직
    void Fire()
    {
        // 1. 풀에서 총알 오브젝트 가져오기
        GameObject bulletObject = GameManager.instance.pool.Get(prefabId);
        if (bulletObject == null) return;
        
        bulletObject.transform.position = transform.position;

        // 2. 총알의 방향 설정
        // Scanner를 사용하지 않으므로, Player의 마지막 이동 방향을 사용합니다.
        //Vector3 dir = player.inputVec;
        
        /*// 이동 방향이 없으면 (정지 상태), Vector3.up (위쪽)을 기본 방향으로 설정합니다.
        if (dir == Vector3.zero)
        {
            dir = Vector3.up; 
        }

        // 3. Bullet 컴포넌트 가져오기
        Bullet bullet = bulletObject.GetComponent<Bullet>();

        // 4. Bullet의 Init 함수 호출 (damage, per(count), dir)
        // Bullet.cs의 Init 시그니처: public void Init(float damage, int per, Vector3 dir)
        bullet.Init(damage, count, dir.normalized); // <--- Bullet의 시그니처에 맞게 count 값 전달
    */}
    
    // 기존 주석 처리된 Init() 부분은 위에 재작성된 Init()으로 대체합니다.
}