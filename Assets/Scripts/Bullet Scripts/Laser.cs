using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Laser : MonoBehaviour
{
    // Tốc độ di chuyển của laser
    public float speed = 5f; // Laser bay chậm hơn đạn thường
    
    // Thời gian tồn tại tối đa của laser trước khi tự hủy
    public float lifetime = 5f; 

    void Start()
    {
        // Hủy laser sau thời gian lifetime để tránh tiêu tốn tài nguyên
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Di chuyển laser theo hướng lên trên với tốc độ đã thiết lập
        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra va chạm với đối tượng khác
        if (other.tag == "Border") // Nếu laser chạm vào biên giới màn hình, hủy laser
        {
            Destroy(gameObject);
        }
        // Có thể mở rộng để laser tương tác với các đối tượng khác như kẻ địch
    }
}
