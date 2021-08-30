using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCFUNdam : MCEnemy
{
    public GameObject explosion;
    public GameObject pilot;

    protected override void EnemyAttack()
    {
        LookAtTarget();

        if ((Vector3.Distance(transform.position, target.position) < xclose + 3) && (attackTimer < 0))
        {
            if (Random.Range(0, 2) == 1)
            {
                StartCoroutine(TriggerAnim("Shield Bash"));
            }
            else
            {
                StartCoroutine(TriggerAnim("JumpShot"));
            }
            attackTimer += attackCoolDown;
        }
        else if ((Mathf.Abs(target.position.z - transform.position.z) < .1f) && (Vector3.Distance(transform.position, target.position) > xclose) && (attackTimer < 0))
        {
            StartCoroutine(TriggerAnim("Shoot"));
            attackTimer += attackCoolDown;
        }
    }

    public override IEnumerator GetHit(Collider tori, float damage, Vector3 direction, bool launch = true, float hitStopMult = 1)
    {
        if (invincible)
        {
            yield break;
        }

        Instantiate(hitSparks, col.ClosestPoint(attackHitBox.transform.position), Quaternion.identity);
        AudioManager.Instance.PlayOneShot("Hit");

        spriteRenderer.material.SetFloat("_FlashAmount", 1);
        speed = 0;

        yield return HitStop(damage * hitStopMult);

        speed = .5f;
        spriteRenderer.material.SetFloat("_FlashAmount", 0);

        Health -= (int)damage;

        if (tori != null && tori.CompareTag("Player"))
        {
            target = tori.transform;
        }
    }

    void Explode()
    {
        AudioManager.Instance.Play("Explosion");
        Instantiate(explosion, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Instantiate(pilot, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        Dispose();
    }
}
