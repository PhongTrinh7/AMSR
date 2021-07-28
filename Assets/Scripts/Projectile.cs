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

    private void OnTriggerEnter(Collider other)
    {
        Character c = other.GetComponent<Character>();
        if (c != null && c.counter)
        {
            c.StartCoroutine(c.HitStop(.2f));
            direction = -direction;
            return;
        }

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
        foreach (Collider col in Physics.OverlapSphere(transform.position, 1))
        {
            Character c = col.GetComponent<Character>();
            if (c != null)
            {
                c.Health -= damage;
                c.StartCoroutine(c.HitStop((float) damage/10, knock));
            }
        }
    }
}
