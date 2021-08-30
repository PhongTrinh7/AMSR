using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCharacter : MonoBehaviour
{
    public bool dummy;

    #region Stats
    public int currentHealth;
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
                Death();
            }
        }
    }
    #endregion

    #region Movement
    public bool sprint;
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

    float buttCD = .5f;
    int buttCnt = 0;

    int layerMask = 1 << 8; //8 For Character Layer.
    #endregion

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        Physics.IgnoreLayerCollision(8, 8);
        layerMask = ~layerMask;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        spriteRenderer.receiveShadows = true;

        Health = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.Instance.dialoguing)
        {
            return;
        }

        IsGrounded();
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

        //Store user input as a movement vector
        Vector3 m_Input = new Vector3(hori, 0, verti * 2);

        //Apply the movement vector to the current position, which is
        //multiplied by deltaTime and speed for a smooth MovePosition
        rb.MovePosition(transform.position + m_Input * Time.deltaTime * speed);

        //Vector3 position = rb.position;
        //position.x = position.x + speed * hori * Time.deltaTime;
        //position.z = position.z + 2 * speed * verti * Time.deltaTime;
        //transform.position = position;
    }

    protected void OnAnimatorMove()
    {
        transform.position += animator.deltaPosition;
    }

    protected bool CheckDoubleTap(string button)
    {
        if (Input.GetKeyDown(button))
        {

            if (buttCD > 0 && buttCnt == 1/*Number of Taps you want Minus One*/)
            {
                return true;
            }
            else
            {
                buttCD = .5f;
                buttCnt += 1;
            }
        }

        if (buttCD > 0)
        {

            buttCD -= 1 * Time.deltaTime;

        }
        else
        {
            buttCnt = 0;
        }

        return false;
    }

    protected virtual bool IsGrounded()
    {
        bool b = Physics.Raycast(transform.position, -Vector3.up, distToGround, layerMask);
        animator.SetBool("Grounded", b);
        return b;
    }

    protected virtual void Jump()
    {
        if (IsGrounded())
        {
            rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
        }
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

        if (target == null || target.invincible)
        {
            yield break;
        }

        StartCoroutine(HitStop(attackDamage));

        switch (attackType)
        {
            case AttackType.NORMAL:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * attackKnockback * 1, 2, 0), false));
                break;
            case AttackType.UPLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * 1, attackKnockback * 15, 0)));
                break;
            case AttackType.FORWARDLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * attackKnockback * 15, 3, 0)));
                break;
            case AttackType.DOWNLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(lookDirection * 3, attackKnockback * -15, 0)));
                break;
            case AttackType.BACKLAUNCH:
                target.StartCoroutine(target.GetHit(col, attackDamage, new Vector3(-lookDirection * attackKnockback * 10, 3, 0)));
                break;
        }

        yield return null;
    }

    public IEnumerator HitStop(float stopTime)
    {
        stopTime /= 20;

        //Vector3 vel = rb.velocity;

        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSeconds(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;

        //rb.velocity = vel;
    }

    public virtual IEnumerator GetHit(Collider tori, float damage, Vector3 direction, bool launch=true, float hitStopMult=1)
    {
        if (invincible)
        {
            yield break;
        }

        if (CheckBlock(tori))
        {
            AudioManager.Instance.PlayOneShot("Block");

            if (parry)
            {
                Instantiate(Resources.Load("DeflectEffect"), col.ClosestPoint(attackHitBox.transform.position), Quaternion.identity);
                animator.Play("Deflect");
                yield break;
            }
            else
            {
                Instantiate(Resources.Load("BlockEffect"), col.ClosestPoint(attackHitBox.transform.position), Quaternion.identity);
            }

            rb.AddForce(new Vector3(-lookDirection * damage, 0, 0), ForceMode.Impulse);
            Health -= (int) damage / 2;
            yield break;
        }

        if (launch)
        {
            StartCoroutine(TriggerAnim("Launched"));
        }
        else
        {
            StartCoroutine(TriggerAnim("Hit"));
        }

        Instantiate(hitSparks, col.ClosestPoint(attackHitBox.transform.position), Quaternion.identity);
        AudioManager.Instance.PlayOneShot("Hit");
        Health -= (int)damage;

        yield return HitStop(damage * hitStopMult);

        GetLaunched(direction);
    }

    protected virtual bool CheckBlock(Collider tori)
    {
        return tori != null && lookDirection == (int) Mathf.Sign(tori.transform.position.x - transform.position.x) && blocking;
    }

    public void GetLaunched(Vector3 direction)
    {
        rb.velocity.Set(0, 0, 0);
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

    public virtual void AnimAttackReset()
    {
        attackHitBox.gameObject.SetActive(false);
        attackType = AttackType.NORMAL;
        attackDamage = 1;
        attackKnockback = 1;
    }

    protected void AnimBlock(int i)
    {
        if (i == 1)
        {
            blocking = true;
        } else
        {
            blocking = false;
        }
    }

    protected void AnimParry(int i)
    {
        if (i == 1)
        {
            parry = true;
        }
        else
        {
            parry = false;
        }
    }

    protected void AnimInvincible(int i)
    {
        if (i == 1)
        {
            invincible = true;
        }
        else
        {
            invincible = false;
        }
    }

    public void AnimPlaySound(string s)
    {
        AudioManager.Instance.PlayOneShot(s);
    }

    protected virtual void Death()
    {

    }

    protected virtual void Dispose()
    {
        Destroy(gameObject);
    }
}
