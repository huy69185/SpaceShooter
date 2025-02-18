using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Tốc độ rơi xuống
    [SerializeField] private float destroyTime = 10f; // Thời gian tự hủy nếu không ai ăn
    [SerializeField] private AudioClip collectSound; // Âm thanh khi nhặt

    private AudioSource audioSource; // AudioSource để phát âm thanh
    private SpriteRenderer spriteRenderer; // Để ẩn sprite khi nhặt
    private Collider2D myCollider; // Để vô hiệu hóa va chạm khi nhặt
    private bool isCollected = false; // Kiểm tra đã nhặt chưa

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Lấy AudioSource
        spriteRenderer = GetComponent<SpriteRenderer>(); // Lấy SpriteRenderer
        myCollider = GetComponent<Collider2D>(); // Lấy Collider2D

        // Nếu không có AudioSource, thêm vào để phát âm thanh
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        StartCoroutine(DestroyAfterTime()); // Hủy sau thời gian destroyTime nếu không được nhặt
    }

    void Update()
    {
        if (!isCollected)
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime); // Rơi xuống
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isCollected)
        {
            CollectHealth(collision.gameObject); // Gọi hàm nhặt Health
        }
    }

    private void CollectHealth(GameObject player)
    {
        isCollected = true;

        // Thực hiện hành động khi nhặt Health, ví dụ như tăng mạng
        Plane planeScript = player.GetComponent<Plane>();  // Giả sử Player có script Plane
        if (planeScript != null)
        {
            planeScript.GainLife(); // Tăng mạng cho Player
        }

        // Nếu có âm thanh và AudioSource, phát âm thanh và hủy đối tượng
        if (audioSource != null && collectSound != null)
        {
            StartCoroutine(PlaySoundAndDestroy());
        }
        else
        {
            Destroy(gameObject); // Hủy đối tượng ngay nếu không có âm thanh
        }
    }

    IEnumerator PlaySoundAndDestroy()
    {
        // Ẩn sprite và vô hiệu hóa va chạm
        spriteRenderer.enabled = false;
        myCollider.enabled = false;

        // Phát âm thanh thu thập
        audioSource.PlayOneShot(collectSound);

        // Chờ âm thanh phát xong trước khi hủy đối tượng
        yield return new WaitForSeconds(collectSound.length);

        // Hủy đối tượng sau khi nhặt
        Destroy(gameObject);
    }

    // Hàm hủy đối tượng sau thời gian destroyTime nếu chưa được nhặt
    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime); // Chờ đủ thời gian
        if (!isCollected) // Nếu chưa được nhặt
        {
            Destroy(gameObject); // Hủy đối tượng
        }
    }
}
