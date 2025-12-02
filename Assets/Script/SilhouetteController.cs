// SilhouetteController.cs (생존자 오브젝트에 부착)
using UnityEngine;

public class SilhouetteController : MonoBehaviour
{
    // 유니티 에디터에서 자식 오브젝트의 MeshRenderer를 연결합니다.
    [Tooltip("실루엣 렌더링을 담당하는 자식 오브젝트의 MeshRenderer를 연결하세요.")]
    public MeshRenderer silhouetteRenderer; 
    
    // 이펙트 활성화/비활성화 함수
    public void SetSilhouetteActive(bool isActive)
    {
        if (silhouetteRenderer != null)
        {
            // 실루엣 렌더러를 활성화/비활성화하여 효과를 토글합니다.
            silhouetteRenderer.enabled = isActive;
        }

        // 시나리오 연동을 위한 디버그 로그
        Debug.Log(gameObject.name + " 실루엣 효과: " + (isActive ? "활성화" : "비활성화"));
    }
    
    // Awake에서 실루엣 효과를 초기에는 꺼둡니다.
    void Awake()
    {
        if (silhouetteRenderer != null)
        {
            silhouetteRenderer.enabled = false;
        }
    }
}