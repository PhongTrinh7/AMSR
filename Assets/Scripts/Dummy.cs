using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Character
{
    // Start is called before the first frame update
    protected override void Start()
    {
        Health = maxHealth;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.AddTorque(0, 0, 30, ForceMode.Impulse);
    }

    // Update is called once per frame
    protected override void Update()
    {
        Health--;
    }

    protected override IEnumerator HitStop(float stopTime, Collider col, Vector2 force, int damage, bool launch = true)
    {
        Instantiate(hitSparks, col.transform.position, Quaternion.identity);
        AudioManager.Instance.PlayOneShot("Hit");

        //BodyAttackEnd();
        StartCoroutine(col.GetComponent<Character>().HitStop(stopTime, launch));

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSecondsRealtime(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

        col.attachedRigidbody.AddForce((Vector3)force, ForceMode.Impulse);
        col.GetComponent<Character>().Health -= damage;
    }

    public override IEnumerator HitStop(float stopTime, bool launch = true)
    {
        //BodyAttack(stopTime);
        //Debug.Log("huh");
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        yield return new WaitForSecondsRealtime(stopTime);
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

    }
}
