using System.Collections;
using UnityEngine;

public class WeaponDrop : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f; // Tốc độ rơi
    [SerializeField] private float destroyTime = 10f; // Tự hủy nếu không ai nhặt
    [SerializeField] private AudioClip collectSound; // Âm thanh khi nhặt

    private AudioSource audioSource; // AudioSource để phát âm thanh
    private SpriteRenderer spriteRenderer; // Để ẩn sprite khi nhặt
    private Collider2D myCollider; // Để vô hiệu hóa va chạm khi nhặt
    private bool isCollected = false; // Kiểm tra đã nhặt chưa

    void Start()
    {
        // Kiểm tra và đảm bảo có AudioSource để phát âm thanh
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)  // Nếu không có AudioSource, thêm mới
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Kiểm tra nếu âm thanh không có, gán mặc định (ví dụ: âm thanh tĩnh)
        if (collectSound == null)
        {
            collectSound = Resources.Load<AudioClip>("DefaultCollectSound"); // Đảm bảo có âm thanh mặc định trong thư mục Resources
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        // Đảm bảo sẽ tự hủy nếu không được nhặt trong khoảng thời gian
        StartCoroutine(DestroyAfterTime());
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
            EquipWeapon(collision.gameObject); // Gọi hàm nhặt vũ khí
        }
    }

    private void EquipWeapon(GameObject player)
    {
        isCollected = true;

        // Kích hoạt vũ khí cho player
        Plane planeScript = player.GetComponent<Plane>();
        if (planeScript != null)
        {
            planeScript.ActivateLaserMode(); // Kích hoạt chế độ laser
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

    private IEnumerator PlaySoundAndDestroy()
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

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(destroyTime); // Chờ đủ thời gian
        if (!isCollected) // Nếu chưa được nhặt
        {
            Destroy(gameObject); // Hủy đối tượng
        }
    }
}
