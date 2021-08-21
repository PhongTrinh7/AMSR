using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCFuninja : MCEnemy
{
    protected override void EnemyAttack()
    {
        if (Vector3.Distance(transform.position, target.position) <= xfar && attackTimer < 0)
        {
            StartCoroutine(TriggerAnim("Attack"));
            attackTimer += attackCoolDown;
        }
    }
}
