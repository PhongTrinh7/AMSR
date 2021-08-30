using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MC1 : MCharacter
{
    private bool savageAxe;
    public float savageTimer = 0;
    public float savageInterval = 0.1f;

    //UI
    public HealthBar healthBar;
    public EnergyBar energyBar;


    public override int Health
    {
        get
        {
            return currentHealth;
        }

        set
        {
            if (value < currentHealth)
            {
                Energy += currentHealth - value;
            }
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            healthBar.SetCurrentHealth(currentHealth);
        }
    }

    public int Energy
    {
        get
        {
            return energy;
        }

        set
        {
            if (value >= 100 && energy < 100)
            {
                //AudioManager.Instance.Play("Charged");
            }
            energy = Mathf.Clamp(value, 0, 100);


            //GameManager.Instance.ultPrompt.SetActive(energy == 100);
            energyBar.SetCurrentEnergy(energy);
        }
    }
    private int energy = 0;

    protected override void Start()
    {
        base.Start();

        healthBar.SetMaxHealth(maxHealth);
        Energy = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (dummy)
        {
            return;
        }

        base.Update();

        hori = Input.GetAxis("Morizontal");
        verti = Input.GetAxis("Mertical");

        if (CheckDoubleTap("right") || CheckDoubleTap("left"))
        {
            sprint = true;
            speed = 6;
        }
        else if (Input.GetKeyUp("right") || Input.GetKeyUp("left"))
        {
            sprint = false;
            speed = 3;
        }


        Vector3 move = new Vector3(hori, 0, verti);
        animator.SetBool("Sprint", sprint);
        animator.SetFloat("MoveSpeed", move.magnitude * speed);

        if (Input.GetButtonDown("Mire1"))
        {
            Attack();
        }

        if (Input.GetButtonDown("Mire2"))
        {

        }

        if (Input.GetButtonDown("Mire3"))
        {
            animator.SetBool("Block", true);
        }
        else if (Input.GetButtonUp("Mire3"))
        {
            animator.SetBool("Block", false);
        }

        if (Input.GetButtonDown("MJump"))
        {
            Debug.Log("jump");
            animator.SetBool("Jump", true);
        }
        else if (Input.GetButtonUp("MJump"))
        {
            Debug.Log("unjump");
            animator.SetBool("Jump", false);
        }

        if (Input.GetButtonDown("MGun"))
        {
            StartCoroutine(TriggerAnim("Fire"));
        }
    }

    protected void OnTriggerStay(Collider other)
    {
        savageTimer -= Time.deltaTime;

        if (savageAxe && other != col)
        {
            if (savageTimer < 0)
            {
                StartCoroutine(Hit(other));
                savageTimer = savageInterval;
            }
        }
    }

    protected void AnimSavageAxe(int i)
    {
        if (i == 1)
        {
            savageAxe = true;
        }
        else
        {
            savageAxe = false;
        }
    }

    public override void AnimAttackReset()
    {
        base.AnimAttackReset();
        savageAxe = false;
    }
}
