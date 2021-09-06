using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MCAnglerMan : MCharacter
{
    public static AnguraController Instance;
    public LTNGO stand;
    public GameObject anglaura;
    public Projectile anglerang;

    //UI
    public HealthBar healthBar;
    public EnergyBar energyBar;
    public int anglerangs;
    public Text anglerangsText;


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

            if (currentHealth == 0)
            {
                animator.SetBool("Dead", true);
                GameManager.Instance.GameOver();
                animator.Play("AnglerDeath");
                invincible = true;
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

    protected override void Start()
    {
        base.Start();

        healthBar.SetMaxHealth(maxHealth);
        Energy = 0;
        anglerangsText.text = "x" + anglerangs;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        hori = Input.GetAxis("Horizontal");
        verti = Input.GetAxis("Vertical");

        if (CheckDoubleTap("d") || CheckDoubleTap("a"))
        {
            sprint = true;
            speed = 6;
        }
        else if (Input.GetKeyUp("d") || Input.GetKeyUp("a"))
        {
            sprint = false;
            speed = 3;
        }


        Vector3 move = new Vector3(hori, 0, verti);
        animator.SetBool("Sprint", sprint);
        animator.SetFloat("MoveSpeed", move.magnitude * speed);

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (Energy >= 25)
            {
                AudioManager.Instance.Play("Revengeance");
                StartCoroutine(TriggerAnim("Stand"));
            }
        }

        if (Input.GetButtonDown("Fire3"))
        {
            animator.SetBool("Block", true);
        } 
        else if (Input.GetButtonUp("Fire3"))
        {
            animator.SetBool("Block", false);
        }

        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine(TriggerAnim("Jump"));
        }

        if (Input.GetButtonDown("Anglerang"))
        {
            StartCoroutine(TriggerAnim("Anglerang"));
        }

        if (Input.GetButtonDown("Ult"))
        {
            UltimatePowa();
        }
    }

    private void SutandoPowa()
    {
        if (Energy >= 25)
        {
            Energy -= 25;
        }
        Instantiate(stand, rb.position + new Vector3(lookDirection, .5f, 0), Quaternion.identity).Direction(lookDirection);
    }

    private void UltimatePowa()
    {
        if (Energy < 100)
        {
            AudioManager.Instance.Play("Nope");
            return;
        }
        AudioManager.Instance.Play("Coming");
        animator.SetTrigger("Ult");
        //GameManager.Instance.StartCoroutine(GameManager.Instance.CameraTarget(gameObject, 1f));
    }

    public void Anglerection()
    {
        Energy -= 100;
        AudioManager.Instance.Play("Moan");
        LTNGO ltngo = Instantiate(stand, rb.position + new Vector3(-lookDirection, .5f, 0), Quaternion.identity);
        ltngo.Direction(lookDirection);
        ltngo.Ult();
    }

    public void Anglerang()
    {
        if (anglerangs > 0)
        {
            Instantiate(anglerang, projectileOrigin.transform.position, Quaternion.identity).direction = lookDirection;
            anglerangs--;
            anglerangsText.text = "x" + anglerangs;
        }
    }

    protected override IEnumerator Hit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            yield return base.Hit(other);

            Energy += attackDamage;
        }
    }
}
