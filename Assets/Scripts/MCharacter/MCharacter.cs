using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCharacter : MonoBehaviour
{
    public bool dummy;

    #region Stats
    protected int currentHealth;
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
                //Death();
            }
        }
    }
    #endregion

    #region Movement
    public float speed;
    public float jumpHeight = 5f;
    public bool jumping;
    public bool cantMove;
    public bool grounded;

    public float hori;
    public float verti;
    #endregion

    #region Collsion
    public Rigidbody rb;
    protected Collider col;
    protected float distToGround = .2f;
    #endregion

    #region Animation
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public int lookDirection = 1;
    #endregion

    #region Action
    public Collider attackHitBox;
    public enum AttackType
    {
        NORMAL,
        UPLAUNCH,
        FORWARDLAUNCH,
        DOWNLAUNCH,
        BACKLAUNCH
    }
    public AttackType attackType = AttackType.NORMAL;
    public int attackDamage = 1;
    public float attackKnockback = 1;

    public bool isAttacking;
    public bool invincible;
    public bool blocking;
    public bool parry;
    #endregion

    #region Misc
    public GameObject hitSparks;
    public GameObject projectileOrigin;
    public Projectile projectile;
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        spriteRenderer.receiveShadows = true;

        Health = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (IsGrounded())
        {

        }
    }

    protected void FixedUpdate()
    {
        if (cantMove)
        {
            return;
        }

        if (hori < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            lookDirection = -1;
        }
        else if (hori > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            lookDirection = 1;
        }
        Vector3 position = rb.position;
        position.x = position.x + speed * hori * Time.deltaTime;
        position.z = position.z + 2 * speed * verti * Time.deltaTime;
        transform.position = position;
    }

    protected void OnAnimatorMove()
    {
        transform.position += animator.deltaPosition;
    }

    protected virtual bool IsGrounded()
    {
        bool b = Physics.Raycast(transform.position, -Vector3.up, distToGround);
        animator.SetBool("Grounded", b);
        return b;
    }

    protected virtual void Attack()
    {
        StartCoroutine(TriggerAnim("Attack"));
    }

    protected IEnumerator TriggerAnim(string trigger)
    {
        animator.SetTrigger(trigger);
        yield return new WaitForSeconds(.1f);
        animator.ResetTrigger(trigger);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other != col)
        {
            StartCoroutine(Hit(other));
        }
    }

    protected virtual IEnumerator Hit(Collider other)
    {
        MCharacter target = other.gameObject.GetComponent<MCharacter>();

        if (target == null)
        {
            yield break;
        }

        Instantiate(hitSparks, other.ClosestPoint(attackHitBox.transform.position), Quaternion.identity);
        AudioManager.Instance.PlayOneShot("Hit");

        StartCoroutine(HitStop(attackDamage));

        switch (attackType)
        {
            case AttackType.NORMAL:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * attackKnockback * 1, 1, 0), false));
                break;
            case AttackType.UPLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * 1, attackKnockback * 10, 0)));
                break;
            case AttackType.FORWARDLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * attackKnockback * 10, 3, 0)));
                break;
            case AttackType.DOWNLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * 3, attackKnockback * -10, 0)));
                break;
            case AttackType.BACKLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(-lookDirection * attackKnockback * 5, 3, 0)));
                break;
        }

        yield return null;
    }

    protected IEnumerator HitStop(float stopTime)
    {
        stopTime /= 10;

        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;
    }

    public virtual IEnumerator GetHit(Collider tori, float damage, Vector3 direction, bool launch=true)
    {
        yield return HitStop(damage);
        
        GetLaunched(direction);

        if (launch)
        {

        }
    }

    public void GetLaunched(Vector3 direction)
    {
        rb.AddForce(direction, ForceMode.Impulse);
    }

    public void FireProjectile()
    {
        Instantiate(projectile, projectileOrigin.transform.position, Quaternion.identity).direction = lookDirection;
    }

    public void AnimAttackHitBox(int i)
    {
        attackHitBox.gameObject.SetActive(true);

        switch (i) {
            case 1:
                attackType = AttackType.UPLAUNCH;
                break;
            case 2:
                attackType = AttackType.FORWARDLAUNCH;
                break;
            case 3:
                attackType = AttackType.DOWNLAUNCH;
                break;
            case 4:
                attackType = AttackType.BACKLAUNCH;
                break;
            default:
                attackType = AttackType.NORMAL;
                break;
        }
    }

    public void AnimAttackDamage(int damage)
    {
        attackDamage = damage;
    }

    public void AnimAttackKnockback(float knockback)
    {
        attackKnockback = knockback;
    }

    public void AnimAttackReset()
    {
        attackHitBox.gameObject.SetActive(false);
        attackType = AttackType.NORMAL;
        attackDamage = 1;
        attackKnockback = 1;
    }

    public void AnimPlaySound(string s)
    {
        AudioManager.Instance.PlayOneShot(s);
    }
}
