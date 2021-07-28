using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public float attackCoolDown;
    public float attackTimer;

    public bool vertical;
    public float changeTime = 3.0f;

    protected float timer;
    protected int xdirection = 1;
    protected int ydirection = 1;
    public int xfar, xclose;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        timer -= Time.deltaTime;

        if (timer < 0)
        {
            xdirection = Random.Range(-1, 2);
            ydirection = Random.Range(-1, 2);
            timer = changeTime;
        }

        if (attackTimer < 0 && !isAttacking && !cantMove)
        {
            Vector3 targetPos = AnguraController.Instance.transform.position;

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

            if (transform.position.z - AnguraController.Instance.transform.position.z > 0)
            {
                verti = -1;
            }
            else
            {
                verti = 1;
            }

            if (hori < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (hori > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else if (isAttacking || cantMove)
        {
            hori = 0;
            verti = 0;
        }
        else if (attackTimer > 0)
        {
            hori = xdirection;
            verti = ydirection;
            Debug.Log(hori);
            Debug.Log(verti);
        }

        Vector3 move = new Vector3(hori, 0, verti);


        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, 0, move.y);
            lookDirection.Normalize();
        }

        Attack();
    }

    protected virtual void Attack()
    {
        if (Mathf.Abs(transform.position.x - AnguraController.Instance.transform.position.x) < xfar && Mathf.Abs(transform.position.z - AnguraController.Instance.transform.position.z) <= .5f && attackTimer <= 0)
        {
            if (AnguraController.Instance.transform.position.y > 1)
            {
                animator.SetTrigger("Jump Attack");
            }
            else
            {
                animator.SetTrigger("Attack");
            }
            cantMove = true;
            attackTimer += attackCoolDown;
        }

        attackTimer -= Time.deltaTime;
    }

    protected override void FixedUpdate()
    {
        Vector3 position = rb.position;
        position.x = position.x + 1.3f * speed * hori * Time.deltaTime;
        position.z = position.z + speed * verti * Time.deltaTime;
        transform.position = position;
    }

    protected override bool IsGrounded()
    {
        bool b = Physics.Raycast(transform.position, -Vector3.up, distToGround);
        return b;
    }

    private void OnDestroy()
    {
        GameManager.Instance.Enemies--;
    }
}
