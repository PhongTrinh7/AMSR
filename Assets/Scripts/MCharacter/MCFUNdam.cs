using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCFUNdam : MCEnemy
{
    protected override void EnemyAttack()
    {
        LookAtTarget();

        if ((Vector3.Distance(transform.position, target.position) < xclose + 3) && (attackTimer < 0))
        {
            if (Random.Range(0, 2) == 1)
            {
                animator.SetTrigger("Shield Bash");
            }
            else
            {
                animator.SetTrigger("JumpShot");
            }
            attackTimer += attackCoolDown;
        }
        else if ((Mathf.Abs(target.position.z - transform.position.z) < .1f) && (Vector3.Distance(transform.position, target.position) > xclose) && (attackTimer < 0))
        {
            animator.SetTrigger("Shoot");
            attackTimer += attackCoolDown;
        }
    }

    public override IEnumerator GetHit(Collider tori, float damage, Vector3 direction, bool launch = true)
    {
        spriteRenderer.material.SetFloat("_FlashAmount", 1);

        yield return HitStop(damage);

        spriteRenderer.material.SetFloat("_FlashAmount", 0);

        GetLaunched(direction);

        if (tori != null && tori.CompareTag("Player"))
        {
            target = tori.transform;
        }
    }
}
