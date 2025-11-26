using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Transform target;

    bool isLive;

    Rigidbody rigid;
    Collider coll;
    Animator anim;
    WaitForFixedUpdate wait;

    // Awake is called when the script instance is being loaded.
     void Awake()
    {
        coll = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return;
        }

        Vector3 dirVec = target.position - rigid.position;
        Vector3 nextVec = new Vector3(dirVec.x, 0, dirVec.z).normalized * speed;

        rigid.linearVelocity = nextVec;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
        {
            return;
        }

        if (!isLive)
        {
            return;
        }

    }

    void OnEnable()
    {
        target = GameManager.instance.player.transform;

        isLive = true;
        coll.enabled = true;
        rigid.isKinematic = false;
        anim.SetBool("Dead", false);
        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;
        target = GameManager.instance.player.transform;
    }

    void OnTriggerEnter(Collider collision)
    {
        if(!collision.CompareTag("Bullet") || !isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return;
        }

        Bullet bullet = collision.GetComponent<Bullet>();
        if (bullet == null) return;

        health -= bullet.damage;
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
            // AudioManager.instance.PlaySfx(AudioManager.SFX.Hit); // AudioManager가 구현되면 주석 해제
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.isKinematic = true;
            anim.SetBool("Dead", true);

            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            /*if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.SFX.Dead);
            }*/
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode.Impulse);
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}