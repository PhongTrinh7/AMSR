using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnguraController : Character
{
    public AnglerAgent agent;

    public static AnguraController Instance;
    public LTNGO stand;
    public LTNGO ult;
    public GameObject anglaura;
    public Projectile anglerang;
    public int anglerangs;
    public Text anglerangsText;

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
                //agent.AddReward(value - currentHealth);
            }
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
            if (value >= 100 && energy < 100)
            {
                AudioManager.Instance.Play("Charged");
            }
            energy = Mathf.Clamp(value, 0, 100);


            GameManager.Instance.ultPrompt.SetActive(energy == 100);
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
        Energy = 100;
        anglerangsText.text = "x" + anglerangs;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance.dialoguing)
        {
            return;
        }

        //Energy++;

        if (Input.GetButtonDown("Ultimate") || Input.GetButtonDown("Ultimate"))
        {
            AnglerUlt();
        }

        base.Update();
        if (sub)
        {
            if (Input.GetButtonDown("Fire3"))
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

            if (Input.GetButtonDown("Anglerang"))
            {
                animator.SetTrigger("Throw");
            }

            if (Input.GetButtonDown("Fire2"))
            {
                if (Energy >= 25)
                {
                    AudioManager.Instance.PlayOneShot("Revengeance");
                    Instantiate(stand, rb.position + new Vector3(lookDirection.x, .5f, 0), Quaternion.identity).Direction(lookDirection.x);
                    Energy -= 25;
                }
                else
                {
                    AudioManager.Instance.Play("Nope");
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
                    else
                    {
                        AerialAttack();
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
                animator.SetBool("Blocking", true);
            }
        }
        else
        {
            hori = 0;
            verti = 0;
        }

        if (Input.GetButtonUp("Fire3"))
        {
            animator.SetBool("Blocking", false);
            BlockFrames(0);
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
        //agent.AddReward(damage);
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
        //agent.AddReward(5);
        Energy += 5;
    }

    protected void AnglerUlt()
    {
        if (Energy < 100)
        {
            AudioManager.Instance.Play("Nope");
            return;
        }
        Energy -= 100;
        animator.SetTrigger("Ultimate");
        StopAllCoroutines();
        StartCoroutine(GameManager.Instance.CameraTarget(gameObject, 1f));
    }

    public void Anglerection()
    {
        Instantiate(ult, rb.position + new Vector3(-lookDirection.x, .5f, 0), Quaternion.identity).Direction(lookDirection.x);
    }

    public void Anglerang()
    {
        if (anglerangs > 0)
        {
            Instantiate(anglerang, rb.position + new Vector3(lookDirection.x * (attackRange + .2f), 1.5f, 0), Quaternion.identity).direction = (int)lookDirection.x;
            anglerangs--;
            anglerangsText.text = "x" + anglerangs;
        }
    }
}
