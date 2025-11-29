using UnityEngine;
using UnityEngine.InputSystem; // 1. 새로운 Input System 네임스페이스 추가

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    float timer;

    Player player;
    //Scanner scanner;
    public GameObject flashlightObject; 
    private Scanner scannerComponent; 
    // 2. 새로운 Input System을 위한 InputAction 변수 추가
    public InputAction flashlightAction;

    void Awake()
    {
        player = GameManager.instance.player;
        //scanner = player.GetComponent<Scanner>();
    }

    void Start()
    {
        Init();

        if (flashlightObject != null)
        {
            scannerComponent = flashlightObject.GetComponent<Scanner>();
        }
    }

    void OnEnable()
    {
        // 3. 컴포넌트가 활성화될 때 액션을 활성화합니다.
        flashlightAction?.Enable();
    }

    void OnDisable()
    {
        flashlightAction?.Disable();
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        // 4. GetKeyDown 대신 InputAction의 WasPressedThisFrame() 사용
        if (flashlightAction != null && flashlightAction.WasPressedThisFrame() && flashlightObject != null)
        {
            bool isActive = flashlightObject.activeSelf;
            flashlightObject.SetActive(!isActive); // 상태 반전
        }

        switch (id)
        {
            case 0:
                timer += Time.deltaTime;
                // 손전등이 켜져 있고, 스캐너가 대상을 찾았을 때만 발사합니다.
                if (flashlightObject != null && flashlightObject.activeSelf &&
                    scannerComponent != null && scannerComponent.nearestTarget != null)
                {
                    if (timer > speed) {
                        timer = 0f;
                        Fire();
                    }
                }
                //transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                /* 원거리 무기 로직
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }*/ //원래는 이거
                break;
        }
    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
        }
    }

    void Fire()
    {
        // Scanner 기반 3D 발사 로직
        //Transform nearestTarget = scanner.nearestTarget;
        // [변경점 4] 조준 방향 및 발사 위치 결정 로직
        Vector3 dir = transform.forward; // 기본: 무기가 바라보는 앞쪽 (마우스 조준 방향)
        Vector3 firePos = transform.position; // 기본 위치


        // 1. 가장 가까운 적이 없으면 발사하지 않음, 원래는 이거
        /*if (nearestTarget == null)
        {
            return;
        }*/

        // 손전등이 할당되어 있다면 발사 위치를 손전등(총구) 위치로 변경
        if (flashlightObject != null)
        {
            firePos = flashlightObject.transform.position;
            // 손전등이 켜져있고 + 적을 찾았다면 -> 자동 조준 발동!
            if (flashlightObject.activeSelf && scannerComponent != null && scannerComponent.nearestTarget != null)
            {
                Vector3 targetPos = scannerComponent.nearestTarget.position;
                dir = targetPos - firePos; // 적을 향한 방향 계산
                dir.y = 0; // 높이 오차 무시
                dir.Normalize(); // 정규화
            }
        }
        // 총알 생성
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        
        // 위치 및 회전 설정
        bullet.position = firePos; 
        bullet.rotation = Quaternion.LookRotation(dir); // 총알이 날아가는 방향을 보게 함
        
        // 발사!
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
        /*
        // 2. 발사 방향 계산 (플레이어 위치 -> 타겟 위치)
        Vector3 targetPos = nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        
        // 3D 탑다운이므로 Y축 이동은 0으로 고정 (총알이 지면과 평행하게 날아감)
        dir.y = 0; 
        
        // 3. 총알 생성 및 발사 (count만큼 반복)
        for (int i = 0; i < count; i++)
        {
            GameObject bullet = GameManager.instance.pool.Get(prefabId);
            bullet.transform.position = transform.position;
            
            // 4. 총알 방향/회전 설정 및 Init 호출
            bullet.transform.rotation = Quaternion.LookRotation(dir); // 발사 방향으로 총알을 회전
            
            // Init(데미지, 관통횟수, 발사방향) 호출
            bullet.GetComponent<Bullet>().Init(damage, count, dir.normalized);
        }*/
    }
    
    public void LevelUp(float damage, int count)
    {
        this.damage = damage;
        this.count += count;

        if (id == 0)
        {
            Batch();
        }
        
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);   //무언가 새로운 장비를 장착했을 때, 자식 오브젝트에 해당 함수 작동케 함
    }

    public void Init()//원래는 ItemData data에서 정보를 가져오지만 지금은 아니도록 수정
    {
        switch (id)
        {
            case 0:
                speed = 0.5f * Character.WeaponRate;
                //transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            //case 1:
            default:                
                break;
        }
        // //basic set
        // name = "Weapon_" + data.itemID;
        // transform.parent = player.transform;
        // transform.localPosition = Vector3.zero;
        //
        // //property set
        // id = data.itemID;
        // damage = data.baseDamage * Character.WeaponDamage;
        // count = data.baseCount + Character.WeaponCount;
        //
        // for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        // {
        //     if (data.projectile == GameManager.instance.pool.prefabs[index])
        //     {
        //         prefabId = index;
        //         break;
        //     }
        // }
        //
        // switch (id)
        // {
        //     case 0:
        //         speed = 0.5f * Character.WeaponRate;
        //         /*speed = 150 * Character.WeaponSpeedRate;*/
        //         /*Batch();*/
        //         break;
        //     //case 1:
        //     default:                
        //         break;
        // }

        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
}
