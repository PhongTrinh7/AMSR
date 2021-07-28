using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IBG : Enemy
{
    public Projectile projectile;
    public GameObject attackOrigin;

    protected override void Attack()
    {
        Vector3 targetPos = AnguraController.Instance.transform.position;

        if (Mathf.Abs(transform.position.z - targetPos.z) <= 0.1f && attackTimer <= 0 && Mathf.Abs(transform.position.x - targetPos.x) > 4)
        {
            lookDirection.Set(targetPos.x - transform.position.x, 0, 0);
            lookDirection.Normalize();
            transform.localScale = new Vector3(lookDirection.x, 1, 1);

            animator.SetTrigger("Shoot");
            attackTimer += attackCoolDown;
            cantMove = true;
        }

        attackTimer -= Time.deltaTime;
    }

    protected void FireProjectile()
    {
        Instantiate(projectile, attackOrigin.transform.position, Quaternion.identity).direction = (int)lookDirection.x;
    }
}
