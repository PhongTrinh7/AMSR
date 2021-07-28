using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockworkController : MonoBehaviour
{
    Rigidbody rb2D;
    Animator anim;
    public float speed = 3.0f;
    public bool vertical;
    public float changeTime = 3.0f;

    float timer;
    int direction = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        //Vector2 position = rb2D.position;
        //position.x = position.x + Time.deltaTime * speed;

        //rb2D.MovePosition(position);
    }

    public IEnumerator HitStop(float stopTime)
    {
        //animator.speed = 0;
        rb2D.isKinematic = true;
        yield return new WaitForSecondsRealtime(stopTime);
        rb2D.isKinematic = false;
        //animator.speed = 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider enemy = collision.collider;

        //StartCoroutine(enemy.GetComponent<ClockworkController>().HitStop(hitStopTime/2));
        //StartCoroutine(enemy.GetComponent<Character>().HitStop(1 / 2));
        //StartCoroutine(HitStop(1 / 2, enemy, new Vector2(-1 * 3, 3)));
        enemy.gameObject.GetComponent<Animator>().SetTrigger("Launched");
        //enemy.gameObject.GetComponent<Animator>().SetTrigger("Hit");
        enemy.attachedRigidbody.AddForce(new Vector3(-50, 20, 0), ForceMode.Impulse);

        Debug.Log("hit: " + enemy);
        //Instantiate(hitSparks, attackDirPoint, Quaternion.identity);
    }
}
