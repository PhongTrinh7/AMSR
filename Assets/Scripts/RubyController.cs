using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubyController : MonoBehaviour
{
    public float speed;

    public int maxHealth = 5;
    public float timeInvincible = 2.0f;
    public float jumpHeight = 20f;

    public int health { get { return currentHealth; } }
    int currentHealth;

    bool canJump;
    bool isJumping;

    bool isInvincible;
    float invincibleTimer;

    bool cantMove;
    float cantMoveTimer;

    bool cantAttack;
    float cantAttackTimer;

    Rigidbody rigidbody;
    Collider collider;
    float hori;
    float verti;

    Animator animator;
    Vector3 lookDirection = new Vector3(1, 0, 0);

    public LayerMask damageLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;

    public enum Attacks
    {
        NONE,
        LIGHT_1,
        LIGHT_2,
        HEAVY_1
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        canJump = true;
        currentHealth = maxHealth;

        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
    }

    // Update is called once per frame
    void Update()
    {
        if (cantMove)
        {
            cantMoveTimer -= Time.deltaTime;
            if (cantMoveTimer < 0)
                cantMove = false;
        }
        else
        {
            hori = Input.GetAxis("Horizontal");
            verti = Input.GetAxis("Vertical");
        }

        Vector3 move = new Vector3(hori, 0, verti);


        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, 0, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Z", lookDirection.z);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if (canJump && !isJumping && Input.GetButtonDown("Jump"))
        {
            rigidbody.AddForce(new Vector3(0, 1, 0) * jumpHeight);
            //isJumping = true;
            Debug.Log("Jump");
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Launch();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            Debug.Log("k");
        }
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("l");
        }
    }

    private void FixedUpdate()
    {
        Vector3 position = rigidbody.position;
        position.x = position.x + speed * hori * Time.deltaTime;
        position.z = position.z + speed * verti * Time.deltaTime;
        transform.position = position;
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log(currentHealth + "/" + maxHealth);
    }

    public void Launch()
    {
        Vector3 attackDirPoint;

        if (animator.GetFloat("Look X") < 0)
        {
            Debug.Log("Left");
            attackDirPoint = rigidbody.position + new Vector3(-0.5f, 0.5f, 0);
        }
        else
        {
            Debug.Log("Right");
            attackDirPoint = rigidbody.position + new Vector3(0.5f, 0.5f, 0);
        }

        animator.SetTrigger("Launch");
        Collider[] hitEnemies = Physics.OverlapSphere(attackDirPoint, attackRange, damageLayers);

        Debug.Log("hit: " + attackDirPoint);
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("hit: " + enemy);
        }
        //cantMove = true;
        //cantMoveTimer = 1.0f;
    }
}
