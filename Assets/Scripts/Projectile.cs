using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int direction;
    public float speed = 10f;
    public GameObject explosion;
    public float duration;
    public int damage;
    public bool knock;
    public LayerMask damageLayers;

    private void OnTriggerEnter(Collider other)
    {
        MCharacter c = other.GetComponent<MCharacter>();

        if (c != null && c.parry)
        {
            direction = -direction;
            return;
        }

        Debug.Log(other);
        Destroy(gameObject);
    }

    private void Update()
    {
        duration -= Time.deltaTime;

        if (duration <= 0)
        {
            Destroy(gameObject);
        }

        if (direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    private void FixedUpdate()
    {
        transform.position += new Vector3(direction * speed, 0, 0);
    }

    private void OnDestroy()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);

        foreach (Collider col in Physics.OverlapSphere(transform.position, 1, damageLayers))
        {
            MCharacter c = col.GetComponent<MCharacter>();
            if (c != null)
            {
                if (!c.invincible)
                {
                    c.Health -= damage;
                    c.StartCoroutine(c.GetHit(GetComponent<BoxCollider>(), damage, Vector3.up));
                }
            }
        }
    }
}
