using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public float minSize = 0.6f; // Giữ kích thước hợp lý hơn
    public float maxSize = 1.2f;
    public float baseFallSpeed = 2f;
    public GameObject explosionEffect;
    public AudioClip explosionSound;

    private Rigidbody2D myBody;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Collider2D collider2D;
    private bool isDestroyed = false;
    private int health;
    private float fallSpeed;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider2D = GetComponent<Collider2D>();

        // Nếu object không có AudioSource, tự động thêm
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Random kích thước
        float randomSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(randomSize, randomSize, 1f);

        // Đặt máu dựa trên kích thước (1-4 máu)
        health = Mathf.RoundToInt(Mathf.Lerp(1, 4, (randomSize - minSize) / (maxSize - minSize)));

        // Tốc độ rơi điều chỉnh hợp lý
        fallSpeed = baseFallSpeed * (1f / randomSize);
    }

    private void FixedUpdate()
    {
        myBody.velocity = new Vector2(0f, -fallSpeed);
    }

    private void Update()
    {
        if (transform.position.y < -Camera.main.orthographicSize - 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);
            DestroyAsteroid();
        }

        if (collision.CompareTag("Player"))
        {
            DestroyAsteroid();
        }

        if (collision.CompareTag("Border"))
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && !isDestroyed)
        {
            DestroyAsteroid(); // Gọi phương thức phá hủy
        }
    }

    private IEnumerator DestroyAsteroidCoroutine()
    {
        float explosionDuration = 0.7f; // Giảm thời gian để nổ nhanh hơn
        isDestroyed = true;

        // Ẩn thiên thạch và dừng chuyển động
        spriteRenderer.enabled = false;
        collider2D.enabled = false;
        myBody.velocity = Vector2.zero;

        // Instantiate hiệu ứng nổ
        if (explosionEffect)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosion.transform.localScale = transform.localScale; // Scale theo kích thước thiên thạch
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                explosionDuration = ps.main.duration;
                Destroy(explosion, explosionDuration);
            }
        }

        // Phát âm thanh nổ
        if (explosionSound && audioSource)
        {
            audioSource.PlayOneShot(explosionSound);
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(explosionDuration);


        // Hủy object sau khi hiệu ứng kết thúc
        Destroy(gameObject);
    }

    public void DestroyAsteroid()
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            StartCoroutine(DestroyAsteroidCoroutine());
        }
    }
}