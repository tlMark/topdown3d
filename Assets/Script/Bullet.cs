using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;
    Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Enemy") || per == -100)
        {
            return;
        }

        per--;

        if (per < 0)
        {
            rigid.linearVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
        {
            return;
        }
        
        gameObject.SetActive(false);
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per >= 0)
        {
            rigid.linearVelocity = dir * 15f;
        }
    }
}
