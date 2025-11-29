using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public float scanAngle = 90f; // 탐지 각도 (원뿔의 각도)

    public LayerMask targetLayer;
    public Collider[] targets;
    public Transform nearestTarget;

    void FixedUpdate()
    {
        targets = Physics.OverlapSphere(transform.position, scanRange, targetLayer);
        nearestTarget = GetNearest();
    }

    Transform GetNearest()
    {
        Transform result = null;
        float diff= 100;

        foreach (Collider target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            //float curDiff = Vector3.Distance(myPos, targetPos);
            // 1. 스캐너와 타겟 사이의 방향 벡터 계산
            Vector3 dirToTarget = (targetPos - myPos).normalized;
            // 2. 스캐너의 정면 방향과 타겟 방향 사이의 각도 계산
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            // 3. 각도가 설정한 탐지 각도의 절반 이내에 있을 때만 타겟으로 고려
            if (angle < scanAngle / 2)
            {
                float curDiff = Vector3.Distance(myPos, targetPos);
                if (curDiff < diff)
                {
                    diff = curDiff;
                    result = target.transform;
                }
            }
            /*if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }*/
        }

        return result;
    }
    // 에디터에서 탐지 범위를 시각적으로 보여주기 위한 기즈모
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // 원뿔의 뼈대를 그립니다.
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Gizmos.DrawFrustum(Vector3.zero, scanAngle, scanRange, 0, 1);
    }
}
