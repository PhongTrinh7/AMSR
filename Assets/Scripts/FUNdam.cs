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

        if (Mathf.Abs(transform.position.z - targetPos.z) <= 0.4f && attackTimer <= 0)
        {
            lookDirection.Set(targetPos.x - transform.position.x, 0, 0);
            lookDirection.Normalize();
            transform.localScale = new Vector3(lookDirection.x, 1, 1);

            if (Mathf.Abs(transform.position.x - targetPos.x) < 4)
            {
                animator.SetTrigger("Shield Bash");
                attackTimer += attackCoolDown/2;
                cantMove = true;
                isAttacking = true;
            }
            else if (Mathf.Abs(transform.position.x - targetPos.x) >= 4 && Mathf.Abs(transform.position.x - targetPos.x) < 10)
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

        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSecondsRealtime(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;

        spriteRenderer.material.SetFloat("_FlashAmount", 0);

        cantMove = false;
    }

    void Death()
    {
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
