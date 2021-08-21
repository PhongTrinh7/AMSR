using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC1 : MCharacter
{
    // Update is called once per frame
    protected override void Update()
    {
        if (dummy)
        {
            return;
        }

        base.Update();

        hori = Input.GetAxis("Horizontal");
        verti = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(hori, 0, verti);
        animator.SetFloat("MoveSpeed", move.magnitude * speed);

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }
}
