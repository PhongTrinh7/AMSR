using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCIBG : MCEnemy
{
    protected override void EnemyAttack()
    {
        LookAtTarget();

        if ((Mathf.Abs(target.position.z - transform.position.z) < .1f) && (Vector3.Distance(transform.position, target.position) <= xfar) && (attackTimer < 0))
        {
            StartCoroutine(TriggerAnim("Shoot"));
            attackTimer += attackCoolDown;
        }
    }
}
