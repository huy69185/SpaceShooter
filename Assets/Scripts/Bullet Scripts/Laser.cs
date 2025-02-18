using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Laser : MonoBehaviour
{
    public float speed = 5f; // Laser bay chậm hơn đạn thường
    public float lifetime = 5f; // Tự hủy sau 5 giây

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Border")
        {
            Destroy(gameObject);
        }
    }
}
