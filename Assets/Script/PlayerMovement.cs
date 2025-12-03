using UnityEngine;

/// <summary>
/// 플레이어의 물리 이동을 처리하는 컴포넌트입니다.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("--- Movement Settings ---")]
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 15f; // 마우스 커서 기반 회전을 위해 회전 속도를 빠르게 조정

    private Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        rigid.freezeRotation = true; // 물리 기반 이동에서 회전 잠금
    }

    /// <summary>
    /// 외부(Player.cs)에서 호출하여 플레이어를 이동시키고 마우스 방향을 바라보게 회전시킵니다.
    /// </summary>
    /// <param name="inputVector">이동 입력 벡터 (월드 축 기준)</param>
    /// <param name="lookDirection">플레이어 위치에서 마우스 위치로 향하는 방향 벡터 (Y=0)</param>
    public void MoveAndRotate(Vector3 inputVector, Vector3 lookDirection)
    {
        // 1. 이동 처리 (MoveInput을 월드 축 기준으로 사용)
        Vector3 velocity = inputVector * moveSpeed;
        rigid.linearVelocity = new Vector3(velocity.x, rigid.linearVelocity.y, velocity.z);
        
        // 2. 회전 처리 (마우스 위치를 바라보도록 회전)
        // lookDirection이 Vector3.zero가 아닐 때만 회전
        if (lookDirection != Vector3.zero)
        {
            // 원하는 회전값 계산
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            
            // 부드러운 회전 적용
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}