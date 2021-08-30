using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTNGO : MonoBehaviour
{
    protected Rigidbody rb;
    protected Collider col;
    public Animator animator;
    public AudioSource audioSource;

    protected int lookDirection;

    //Attacks
    public LayerMask damageLayers;
    public float attackRange = 0.5f;
    public int attackDamage = 1;

    public float hitForce;
    public float launchForce;
    public GameObject hitSparks;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    public void Ult()
    {
        animator.Play("FurashuBang");
    }

    public void Direction(int dir)
    {
        lookDirection = dir;
        transform.localScale = new Vector3(dir, 1, 1);
    }

    public void Hit()
    {
        AudioManager.Instance.Play("Swing");
        AudioManager.Instance.Play("Ora");
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection, 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                MCharacter target = enemy.gameObject.GetComponent<MCharacter>();

                if (target == null)
                {
                    return;
                }

                StartCoroutine(HitStop(attackDamage * 2));
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * 3, 3, 0), true, 2));
            }
        }
    }

    public void ForwardLaunchHit()
    {
        AudioManager.Instance.Play("Swing");
        AudioManager.Instance.Play("Ora");
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection, 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                MCharacter target = enemy.gameObject.GetComponent<MCharacter>();

                if (target == null)
                {
                    return;
                }

                StartCoroutine(HitStop(attackDamage * 3));
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * 15, 5, 0), true, 3));
            }
        }
    }

    public void Flash()
    {
        GameManager.Instance.StartCoroutine(GameManager.Instance.Trip(2f));
        AudioManager.Instance.Play("Tokiwotomare");
        Vector3 attackDirPoint = rb.position + new Vector3(0, 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, 30, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                MCharacter target = enemy.gameObject.GetComponent<MCharacter>();

                target.StartCoroutine(target.GetHit(col, 10, new Vector3(0, 5, 0), true, 5));
            }
        }
    }

    protected IEnumerator HitStop(float stopTime)
    {
        stopTime /= 20;

        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;
    }

    private void Seppukku()
    {
        Destroy(gameObject);
    }
}
