using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    // Thiết lập kích thước tối thiểu và tối đa của thiên thạch
    public float minSize = 0.6f;
    public float maxSize = 1.2f;
    
    // Tốc độ rơi cơ bản của thiên thạch
    public float baseFallSpeed = 2f;
    
    // Hiệu ứng nổ và âm thanh nổ khi thiên thạch bị phá hủy
    public GameObject explosionEffect;
    public AudioClip explosionSound;

    private Rigidbody2D myBody; // Thành phần vật lý của thiên thạch
    private AudioSource audioSource; // Thành phần âm thanh
    private SpriteRenderer spriteRenderer; // Dùng để hiển thị hình ảnh thiên thạch
    private Collider2D collider2D; // Thành phần va chạm của thiên thạch
    private bool isDestroyed = false; // Kiểm tra xem thiên thạch đã bị phá hủy hay chưa
    private int health; // Lượng máu của thiên thạch
    private float fallSpeed; // Tốc độ rơi thực tế của thiên thạch

    private void Awake()
    {
        // Lấy các thành phần cần thiết
        myBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();
        
        // Nếu object không có AudioSource, tự động thêm vào
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Gán kích thước ngẫu nhiên trong khoảng minSize đến maxSize
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1f);

        // Xác định máu của thiên thạch dựa trên kích thước (từ 1 đến 4 máu)
        health = Mathf.RoundToInt(Mathf.Lerp(1, 4, (randomSize - minSize) / (maxSize - minSize)));

        // Tính tốc độ rơi, các thiên thạch nhỏ sẽ rơi nhanh hơn
        fallSpeed = baseFallSpeed * (1f / randomSize);
    }

    private void FixedUpdate()
    {
        // Cập nhật vận tốc để thiên thạch rơi xuống
        myBody.velocity = new Vector2(0f, -fallSpeed);
    }

    private void Update()
    {
        // Nếu thiên thạch rơi xuống ngoài màn hình, tự hủy để tránh tốn tài nguyên
        if (transform.position.y < -Camera.main.orthographicSize - 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Xử lý va chạm với đạn
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1); // Giảm 1 máu khi bị bắn
            Destroy(collision.gameObject); // Hủy viên đạn
        }

        // Xử lý va chạm với laser (phá hủy ngay lập tức)
        if (collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);
            DestroyAsteroid();
        }

        // Xử lý va chạm với người chơi
        if (collision.CompareTag("Player"))
        {
            DestroyAsteroid();
        }

        // Xử lý va chạm với biên giới màn hình (có thể dùng để giới hạn khu vực xuất hiện thiên thạch)
        if (collision.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }

    // Hàm xử lý khi thiên thạch bị trúng đạn
    public void TakeDamage(int damage)
    {
        health -= damage;

        // Nếu máu về 0 hoặc nhỏ hơn và chưa bị phá hủy trước đó, phá hủy thiên thạch
        if (health <= 0 && !isDestroyed)
        {
            DestroyAsteroid();
        }
    }

    // Coroutine để xử lý hiệu ứng phá hủy thiên thạch
    private IEnumerator DestroyAsteroidCoroutine()
    {
        float explosionDuration = 0.7f; // Thời gian tồn tại của vụ nổ
        isDestroyed = true; // Đánh dấu thiên thạch đã bị phá hủy

        // Ẩn thiên thạch và tắt va chạm
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
        myBody.velocity = Vector2.zero;

        // Hiển thị hiệu ứng nổ
        if (explosionEffect)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.transform.localScale = transform.localScale; // Hiệu ứng nổ có cùng kích thước với thiên thạch
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                explosionDuration = ps.main.duration;
                Destroy(explosion, explosionDuration); // Hủy hiệu ứng sau khi kết thúc
            }
        }

        // Phát âm thanh nổ
        if (explosionSound && audioSource)
        {
            audioSource.PlayOneShot(explosionSound);
            yield return new WaitForSeconds(2f); // Đợi một chút để âm thanh kết thúc
        }

        yield return new WaitForSeconds(explosionDuration);

        // Hủy thiên thạch sau khi hiệu ứng kết thúc
        Destroy(gameObject);
    }

    // Hàm xử lý khi thiên thạch bị phá hủy
    public void DestroyAsteroid()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            StartCoroutine(DestroyAsteroidCoroutine());
        }
    }
}