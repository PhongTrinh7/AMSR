using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTNGO : MonoBehaviour
{
    protected Rigidbody rb;
    protected Collider col;
    public Animator animator;
    public AudioSource audioSource;

    protected Vector3 lookDirection;

    //Attacks
    public LayerMask damageLayers;
    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public float hitForce;
    public float launchForce;

    public float hitStopTime;
    public GameObject hitSparks;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();
    }

    public void Direction(float dir)
    {
        lookDirection = new Vector3(dir, 0, 0);
        transform.localScale = new Vector3(dir, 1, 1);
    }

    public void Hit()
    {
        AudioManager.Instance.PlayOneShot("Swing");
        audioSource.PlayOneShot(audioSource.clip);
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection.x, 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                StartCoroutine(enemy.GetComponent<Character>().HitStop(hitStopTime, false));
                StartCoroutine(HitStop(hitStopTime / 2, enemy, new Vector2(lookDirection.x * hitForce, hitForce / 2)));
            }
        }
    }

    public void ForwardLaunchHit()
    {
        AudioManager.Instance.PlayOneShot("Swing");
        audioSource.PlayOneShot(audioSource.clip);
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection.x, 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                StartCoroutine(enemy.GetComponent<Character>().HitStop(hitStopTime));
                StartCoroutine(HitStop(hitStopTime, enemy, new Vector2(lookDirection.x * launchForce, hitForce)));
            }
        }
    }

    protected IEnumerator HitStop(float stopTime, Collider col, Vector2 force)
    {
        Instantiate(hitSparks, col.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        AudioManager.Instance.PlayOneShot("Hit");
        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionY;
        yield return new WaitForSecondsRealtime(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;

        if (col == null)
        {
            yield return null;
        }

        col.attachedRigidbody.AddForce(force, ForceMode.Impulse);
    }

    private void Seppukku()
    {
        Destroy(gameObject);
    }
}
