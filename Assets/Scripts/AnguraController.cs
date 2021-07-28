using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnguraController : Character
{
    public static AnguraController Instance;
    public LTNGO stand;
    public GameObject anglaura;

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
            if (value != currentHealth)
            {
                Energy += 5;
            }
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
            healthBar.SetCurrentHealth(currentHealth);

            if (currentHealth == 0)
            {
                animator.SetBool("Dead", true);
                GameManager.Instance.GameOver();
                animator.Play("AnglerDeath");
                this.enabled = false;
            }
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
            energy = Mathf.Clamp(value, 0, 100);
            energyBar.SetCurrentEnergy(energy);
            anglaura.SetActive(energy >= 25);
        }
    }
    private int energy = 0;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        healthBar.SetMaxHealth(maxHealth);
        Energy = 0;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance.dialoguing)
        {
            return;
        }

        //Energy++;

        base.Update();
        if (sub)
        {
            if (Input.GetButton("Fire3"))
            {
                if (Energy >= 25)
                {
                    Energy -= 25;
                    Sub();
                    return;
                }
            }
        }

        if (!cantMove && !isAttacking)
        {

            hori = Input.GetAxis("Horizontal");
            verti = Input.GetAxis("Vertical");

            if (hori < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (hori > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if (Energy >= 25)
                {
                    AudioManager.Instance.PlayOneShot("Revengeance");
                    Instantiate(stand, rb.position + new Vector3(lookDirection.x, .5f, 0), Quaternion.identity).Direction(lookDirection.x);
                    Energy -= 25;
                }
            }

            if (Input.GetAxisRaw("Sprint") >= .5f || Input.GetButton("Sprint"))
            {
                speed = 6;
                if (Input.GetButtonDown("Fire1"))
                {
                    if (IsGrounded())
                    {
                        cantMove = true;
                        animator.Play("AnglerRunSlash");
                    }
                    return;
                }
            } else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (!IsGrounded())
                    {
                        AerialAttack();
                    }
                    else
                    {
                        GroundAttack();
                    }
                }
            }
            
            if (Input.GetAxisRaw("Sprint") == 0 || Input.GetKeyUp("left shift"))
            {
                speed = 3;
            }

            if (Input.GetButtonDown("Fire3"))
            {
                animator.SetTrigger("Counter");
            }
        }
        else
        {
            hori = 0;
            verti = 0;
        }

        Vector3 move = new Vector3(hori, 0, verti);


        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, 0, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look Direction", lookDirection.x);
        animator.SetFloat("Speed", move.magnitude * speed);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }

    protected override IEnumerator HitStop(float stopTime, Collider col, Vector2 force, int damage, bool launch = true)
    {
        Instantiate(hitSparks, col.transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
        AudioManager.Instance.PlayOneShot("Hit");
        col.GetComponent<Character>().Health -= damage;
        Energy += damage;
        //BodyAttackEnd();

        StartCoroutine(col.GetComponent<Character>().HitStop(stopTime, launch));

        animator.speed = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionY;
        yield return new WaitForSecondsRealtime(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        animator.speed = 1;

        col.attachedRigidbody.AddForce((Vector3)force, ForceMode.Impulse);
    }

    public override void Deflect()
    {
        base.Deflect();

        Energy += 5;
    }

    public void PlaySound(string s)
    {
        AudioManager.Instance.PlayOneShot(s);
    }
}
