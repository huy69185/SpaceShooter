using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Tốc độ di chuyển của viên đạn
    public float speed;

    // Thành phần vật lý của viên đạn
    private Rigidbody2D myBody;

    void Awake()
    {
        // Lấy component Rigidbody2D để điều khiển vật lý của viên đạn
        myBody = GetComponent<Rigidbody2D>();
    }

    // FixedUpdate được gọi mỗi khung hình vật lý
    void FixedUpdate()
    {
        // Cập nhật vận tốc của viên đạn để nó di chuyển theo trục y
        myBody.velocity = new Vector2(0f, speed);
    }

    // Xử lý khi viên đạn chạm vào các đối tượng khác
    void OnTriggerEnter2D(Collider2D target)
    {
        // Nếu viên đạn chạm vào biên giới màn hình, hủy viên đạn
        if (target.tag == "Border")
        {
            Destroy(gameObject);
        }
    }
}
