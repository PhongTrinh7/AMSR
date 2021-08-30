using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int direction;
    public float speed = 10f;
    public float launchPower = 1;
    public GameObject explosion;
    public float duration;
    public int damage;
    public bool knock;
    public LayerMask damageLayers;

    public string startSound;
    public string hitSound;

    private void Start()
    {
        Physics.IgnoreLayerCollision(14, 15);
        AudioManager.Instance.Play(startSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        MCharacter c = other.GetComponent<MCharacter>();

        if (c != null && c.parry)
        {
            AudioManager.Instance.PlayOneShot("Block");
            Instantiate(Resources.Load("DeflectEffect"), other.ClosestPoint(transform.position), Quaternion.identity);
            //c.StartCoroutine(c.GetHit(GetComponent<BoxCollider>(), damage, Vector3.up));
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
        AudioManager.Instance.Play(hitSound);
        Instantiate(explosion, transform.position, Quaternion.identity);

        foreach (Collider col in Physics.OverlapSphere(transform.position, 1, damageLayers))
        {
            MCharacter c = col.GetComponent<MCharacter>();
            if (c != null)
            {
                if (!c.invincible)
                {
                    c.StartCoroutine(c.GetHit(GetComponent<BoxCollider>(), damage, Vector3.up * launchPower));
                }
            }
        }
    }
}
