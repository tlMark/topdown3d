using UnityEngine;

/// <summary>
/// 무기에서 발사되는 투사체(총알)의 이동, 데미지, 관통력을 처리하는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    // [설정] 총알의 기본 이동 속도 (Weapon.cs와 연동)
    private const float BULLET_SPEED = 15f; 
    
    // [속성] 무기로부터 받는 값
    public float damage;
    public int per; // 관통 횟수 (0이면 현재 적을 마지막으로 비활성화)
    
    // [컴포넌트]
    private Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        // Rigidbody 설정: isKinematic = false, Gravity = false (물리 기반 이동을 위해)
    }
    
    /// <summary>
    /// 총알의 속성(데미지, 관통력, 방향)을 설정하고 이동을 시작합니다.
    /// </summary>
    /// <param name="damage">총알의 데미지</param>
    /// <param name="per">관통 횟수 (0 이상: 일반 관통, -100: 영구 지속)</param>
    /// <param name="dir">총알이 나아갈 방향 벡터</param>
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        // 관통 횟수가 -100(영구 지속)이 아닌 경우에만 이동을 시작
        if (per >= 0 || per == -100)
        {
            // 총알이 발사될 때, 리지드바디의 속도를 설정하여 이동시킵니다.
            rigid.linearVelocity = dir * BULLET_SPEED;
        }
        else
        {
            // 비정상적인 per 값에 대한 처리 (혹시 모를 오류 방지)
            rigid.linearVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 물리 충돌 감지 (트리거 충돌)
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    void OnTriggerEnter(Collider collision)
    {
        // 1. 적이 아니거나, 이미 영구 지속 총알이거나, 유효한 관통력이 없을 경우 처리 중단
        if (!collision.CompareTag("Enemy"))
        {
            return;
        }
        
        // 2. 관통력 감소
        // per == -100은 영구 지속을 의미하며, 이 경우 관통력을 감소시키지 않습니다.
        if (per > -100) 
        {
            per--;
        }
        
        // 3. Enemy 스크립트에 데미지 전달 (Enemy.cs에서 처리)
        // Enemy.cs에서 충돌을 처리하므로 여기서는 관통 로직만 담당합니다.
        
        // 4. 관통력을 모두 소진했으면 총알 비활성화
        if (per < 0 && per > -100) // per >= 0에서 시작하여 0 미만이 되었을 경우
        {
            DisableBullet();
        }
        
        // 참고: 영구 지속 총알(per == -100)은 이 로직으로 비활성화되지 않습니다.
    }

    /// <summary>
    /// 영역 이탈 감지 (월드 경계 또는 풀링 영역)
    /// </summary>
    void OnTriggerExit(Collider collision)
    {
        // "Area" 태그를 가진 콜라이더 (예: 맵 경계)를 벗어나면 비활성화
        if (collision.CompareTag("Area"))
        {
            DisableBullet();
        }
    }
    
    /// <summary>
    /// 총알을 비활성화하고 속도를 멈춥니다.
    /// </summary>
    private void DisableBullet()
    {
        // 속도를 멈춰서 풀로 돌아간 후에도 미끄러지지 않게 합니다.
        rigid.linearVelocity = Vector3.zero; 
        gameObject.SetActive(false);
    }
}