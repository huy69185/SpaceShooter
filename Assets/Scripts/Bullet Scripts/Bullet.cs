using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;

    private Rigidbody2D myBody;

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        myBody.velocity = new Vector2(0f, speed);
    }

    void OnTriggerEnterPlane2D(Collider2D target)
    {
     
        if (target.tag == "Border")
        {
            Destroy(gameObject);
        }
    }
}
