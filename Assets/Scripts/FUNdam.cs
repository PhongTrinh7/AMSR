using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FUNdam : Enemy
{
    private bool dying;
    public GameObject explosion;
    public Projectile laser;
    public GameObject attackOrigin;

    public override int Health
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

    protected override void Update()
    {
        if (dying)
        {
            return;
        }

        base.Update();
    }

    protected override void Attack()
    {
        Vector3 targetPos = AnguraController.Instance.transform.position;

        if (Mathf.Abs(transform.position.z - targetPos.z) <= xclose && attackTimer <= 0)
        {

            if (Mathf.Abs(transform.position.x - targetPos.x) < xclose)
            {
                if (Random.Range(0, 2) == 1) {
                    animator.SetTrigger("Shield Bash");
                }
                else {
                    animator.SetTrigger("JumpShot");
                }
                attackTimer += attackCoolDown/2;
                cantMove = true;
                isAttacking = true;
            }
            else if (Mathf.Abs(transform.position.x - targetPos.x) >= xclose && Mathf.Abs(transform.position.x - targetPos.x) < xfar)
            {
                animator.SetTrigger("Shoot");
                attackTimer += attackCoolDown;
                cantMove = true;
                isAttacking = true;
            }
        }

        attackTimer -= Time.deltaTime;
    }

    protected void FireProjectile()
    {
        AudioManager.Instance.Play("Pew");
        Instantiate(laser, attackOrigin.transform.position, Quaternion.identity).direction = (int) lookDirection.x;
    }

    public override IEnumerator HitStop(float stopTime, bool launch = true)
    {
        cantMove = true;

        spriteRenderer.material.SetFloat("_FlashAmount", 1);

        //StopAllCoroutines();

        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSecondsRealtime(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;
        

        spriteRenderer.material.SetFloat("_FlashAmount", 0);

        cantMove = false;

        yield return null;
    }

    protected override void Death()
    {
        StopAllCoroutines();
        this.enabled = false;
        gameObject.layer = 0;
        animator.SetTrigger("Death");
    }

    void Explode()
    {
        AudioManager.Instance.Play("Explosion");
        Instantiate(explosion, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        BodyAttack(.2f);
        //StopAllCoroutines();
        //Destroy(gameObject);
        Dispose();
    }
}
