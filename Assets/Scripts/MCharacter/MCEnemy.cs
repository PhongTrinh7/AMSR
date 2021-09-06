using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MCEnemy : MCharacter
{
    public float attackCoolDown;
    public float attackTimer = 3;

    public float changeTime = 3.0f;

    protected float timer;
    protected int xdirection = 1;
    protected int ydirection = 1;
    public int xfar, xclose;
    public int leashDist = 15;

    public Transform target = null;

    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        if (timer < 0)
        {
            xdirection = Random.Range(-1, 2);
            ydirection = Random.Range(-1, 2);
            timer = changeTime;

            target = null;
            float minDist = leashDist;

            foreach (GameObject mc in GameObject.FindGameObjectsWithTag("Player"))
            {
                float dist = Vector3.Distance(transform.position, mc.transform.position);
                if (dist < minDist)
                {
                    target = mc.transform;
                    minDist = dist;
                }
            }
        }

        if (cantMove)
        {
            return;
        }
        
        if (target != null && attackTimer < 0)
        {
            Vector3 targetPos = target.position;

            if (transform.position.x - targetPos.x > xfar)
            {
                hori = -1;
            }
            else if (transform.position.x - targetPos.x < -xfar)
            {
                hori = 1;
            }
            else if (transform.position.x - targetPos.x < xclose && transform.position.x - targetPos.x > 0)
            {
                hori = 1;
            }
            else if (transform.position.x - targetPos.x > -xclose && transform.position.x - targetPos.x < 0)
            {
                hori = -1;
            }
            else
            {
                hori = 0;
            }

            if (transform.position.z - targetPos.z > 0)
            {
                verti = -1;
            }
            else
            {
                verti = 1;
            }


            EnemyAttack();
        }
        else
        {
            hori = xdirection;
            verti = ydirection;
        }
    }

    protected virtual void EnemyAttack()
    {
        if (Vector3.Distance(transform.position, target.position) <= xclose + 1 && attackTimer < 0)
        {
            StartCoroutine(TriggerAnim("Attack"));
            attackTimer += attackCoolDown;
        }
    }

    public override IEnumerator GetHit(Collider tori, float damage, Vector3 direction, bool launch = true, float hitStopMult=1)
    {
        yield return base.GetHit(tori, damage, direction, launch, hitStopMult);

        if (tori != null && tori.CompareTag("Player"))
        {
            target = tori.transform;
        }
    }

    protected void LookAtTarget()
    {
        if (target == null)
        {
            return;
        }

        lookDirection = (int) Mathf.Sign(target.position.x - transform.position.x);
        transform.localScale = new Vector3(lookDirection, 1, 1);
    }

    protected override void Death()
    {
        animator.SetTrigger("Death");
    }

    protected override void Dispose()
    {
        GameManager.Instance.Enemies--;

        base.Dispose();
    }
}

