using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count;
    public float speed;
    float timer;

    Player player;
    Scanner scanner;

    void Awake()
    {
        player = GameManager.instance.player;
        scanner = player.GetComponent<Scanner>();
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        switch (id)
        {
            case 0:
                timer += Time.deltaTime;

                    if (timer > speed)
                    {
                        timer = 0f;
                        Fire();
                    }
                //원래는 이거! transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                /*timer += Time.deltaTime;

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
        Transform nearestTarget = scanner.nearestTarget;

        // 1. 가장 가까운 적이 없으면 발사하지 않음
        if (nearestTarget == null)
        {
            return;
        }
        
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
        }
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

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }
}
