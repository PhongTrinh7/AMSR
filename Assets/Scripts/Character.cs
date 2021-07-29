using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public bool dummy;

    //Movement
    public float speed;
    public float jumpHeight = 5f;
    public bool jumping;
    public bool cantMove;
    public bool grounded;

    protected float hori;
    protected float verti;


    //Stats
    public int maxHealth;
    public virtual int Health
    {
        get
        {
            return currentHealth;
        }

        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            if (currentHealth == 0)
            {
                Dispose();
            }
        }
    }
    protected int currentHealth;

    //I-frames
    public bool isInvincible;
    public bool counter;

    //Collision
    public Rigidbody rb;
    protected Collider col;
    protected Collider col2;
    protected float distToGround;

    //Animation
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Vector3 lookDirection = new Vector3(1, 0, 0);
    public float hitStopTime;

    //Attacks
    public LayerMask damageLayers;
    public float attackRange = 0.5f;
    public bool isAttacking;
    public bool isAirAttacking;
    public GameObject hitSparks;

    public float hitForce;
    public float launchForce;

    public GameObject substitution;
    public bool sub;

    public Sprite portrait;

    public GameObject sphere;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        col2 = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        distToGround = col.bounds.extents.y;

        Health = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        animator.SetBool("Grounded", IsGrounded());

        if (isAirAttacking)
        {
            //rb.AddForce(new Vector3(0, 2f, 0));
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
            return;
        }
        //else
        //{
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        //}
    }

    protected virtual void FixedUpdate()
    {
        Vector3 position = rb.position;
        position.x = position.x + speed * hori * Time.deltaTime;
        position.z = position.z + 2 * speed * verti * Time.deltaTime;
        transform.position = position;
    }

    private void OnAnimatorMove()
    {
        transform.position += animator.deltaPosition;
        //transform.forward = animator.deltaRotation * transform.forward;
    }

    protected virtual bool IsGrounded()
    {
        bool b = Physics.Raycast(transform.position, -Vector3.up, distToGround - 1f);
        return b;
    }

    protected void Jump()
    {
        if (cantMove || isAttacking || !IsGrounded())
        {
            return;
        }

        jumping = true;
        animator.SetBool("Jumping", true);
        rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
    }

    protected virtual void GroundAttack()
    {
        isAttacking = true;
    }

    protected virtual void AerialAttack()
    {
        isAirAttacking = true;
    }

    protected virtual void Hit(float stopTime)
    {
        if (stopTime == 0)
        {
            stopTime = .1f;
        }

        int damage = (int) (stopTime * 10);

        AudioManager.Instance.PlayOneShot("Swing");
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection.x * (attackRange + .2f), 1.5f, 0);

        //Instantiate(sphere, attackDirPoint + new Vector3(lookDirection.x * attackRange, 0, 0), Quaternion.identity);
        //Instantiate(sphere, attackDirPoint - new Vector3(lookDirection.x * attackRange, 0, 0), Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy != col && enemy != col2 && !enemy.GetComponent<Character>().isInvincible) StartCoroutine(HitStop(stopTime, enemy, new Vector2(lookDirection.x * hitForce, hitForce * 1.5f), damage, false));
        }
    }

    protected virtual void ForwardLaunchHit(float stopTime)
    {
        if (stopTime == 0)
        {
            stopTime = .3f;
        }

        int damage = (int) (stopTime * 10);

        AudioManager.Instance.PlayOneShot("Swing");
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection.x * (attackRange + .2f), 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy != col && enemy != col2 && !enemy.GetComponent<Character>().isInvincible) StartCoroutine(HitStop(stopTime, enemy, new Vector2(lookDirection.x * launchForce * 1.3f, hitForce), damage));
        }
    }

    protected virtual void UpLaunchHit(float stopTime)
    {
        if (stopTime == 0)
        {
            stopTime = .3f;
        }

        int damage = (int) (stopTime * 10);

        AudioManager.Instance.PlayOneShot("Swing");
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection.x * (attackRange + .2f), 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy != col && enemy != col2 && !enemy.GetComponent<Character>().isInvincible) StartCoroutine(HitStop(stopTime, enemy, new Vector2(lookDirection.x, launchForce), damage));
        }
    }

    protected virtual void DownLaunchHit(float stopTime)
    {
        if (stopTime == 0)
        {
            stopTime = .3f;
        }

        int damage = (int) (stopTime * 10);

        AudioManager.Instance.PlayOneShot("Swing");
        Vector3 attackDirPoint = rb.position + new Vector3(lookDirection.x * (attackRange + .2f), 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy != col && enemy != col2 && !enemy.GetComponent<Character>().isInvincible) StartCoroutine(HitStop(stopTime, enemy, new Vector2(lookDirection.x * hitForce, -launchForce), damage));
        }
    }

    public virtual void BodyAttack(float stopTime)
    {
        if (stopTime == 0)
        {
            stopTime = .3f;
        }

        int damage = (int)(stopTime * 10);

        AudioManager.Instance.PlayOneShot("Swing");
        Vector3 attackDirPoint = rb.position + new Vector3(0, 1.5f, 0);

        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy != col && enemy != col2 && !enemy.GetComponent<Character>().isInvincible) StartCoroutine(HitStop(stopTime, enemy, new Vector2(Vector3.Normalize(enemy.attachedRigidbody.position - rb.position).x * launchForce, hitForce), damage));
        }
    }

    protected virtual IEnumerator HitStop(float stopTime, Collider col, Vector2 force, int damage, bool launch=true)
    {
        StartCoroutine(col.GetComponent<Character>().HitStop(stopTime, launch));

        if (!col.GetComponent<Character>().counter)
        {
            Instantiate(hitSparks, col.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
            AudioManager.Instance.PlayOneShot("Hit");

            animator.speed = 0;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            yield return new WaitForSecondsRealtime(stopTime);
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            animator.speed = 1;

            col.attachedRigidbody.AddForce((Vector3)force, ForceMode.Impulse);
            col.GetComponent<Character>().Health -= damage;
        }
    }

    public virtual IEnumerator HitStop(float stopTime, bool launch=true)
    {
        if (counter)
        {
            //animator.Play("AnglerDeflect");
            animator.SetTrigger("Deflect");
            Debug.Log("bing");
        }
        else
        {
            cantMove = true;

            if (launch)
            {
                animator.SetTrigger("Launched");
            }
            else
            {
                animator.SetTrigger("Hit");
            }

            animator.speed = 0;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            yield return new WaitForSecondsRealtime(stopTime);
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            animator.speed = 1;
        }
    }

    public virtual void Sub()
    {
        Instantiate(substitution, transform.position, Quaternion.identity);
        //Debug.Log("sub");
        AudioManager.Instance.PlayOneShot("Instant Transmission");
        transform.position += (Vector3.up + lookDirection) * 3;
        animator.SetBool("Sub", true);
        sub = false;
        cantMove = false;
    }

    public void CantMove(int b)
    {
        if (b == 1)
        {
            cantMove = true;
        }
        else
        {
            cantMove = false;
        }
    }

    public void ReturnMobility()
    {
        cantMove = false;
        isAttacking = false;
    }

    public void IFrames(int b)
    {
        if (b == 1)
        {
            isInvincible = true;
        }
        else
        {
            isInvincible = false;
        }
    }

    public virtual void Deflect()
    {
        AudioManager.Instance.PlayOneShot("Block");

        Instantiate(Resources.Load("DeflectEffect"), rb.position + new Vector3(lookDirection.x * (attackRange + .2f), 1.5f, 0), Quaternion.identity);

        BodyAttack(.1f);

        //rb.AddForce(-lookDirection * launchForce, ForceMode.Impulse);
    }

    public void CounterFrames(int b)
    {
        if (b == 1)
        {
            counter = true;
            cantMove = true;
        }
        else
        {
            counter = false;
        }
    }

    protected void Dispose()
    {
        transform.position = new Vector3(0, -20, 0);
        Destroy(gameObject, 1f);
    }

}
