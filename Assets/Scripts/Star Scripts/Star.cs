using System.Collections;
using UnityEngine;

public class Star : MonoBehaviour
{
    public float minSize = 0.1f;
    public float maxSize = 0.3f;
    public float fallSpeed = 2f;
    public int baseScore = 10; // Điểm cơ bản khi kích thước là minSize
    public int maxScore = 30;  // Điểm tối đa khi kích thước là maxSize
    public AudioClip collectSound;

    private Rigidbody2D myBody;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;

    private bool isCollected = false;
    private float starSize; // Kích thước thực tế của ngôi sao
    private int starScore;  // Điểm số thực tế của ngôi sao

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();

        // Xác định kích thước ngẫu nhiên
        starSize = Random.Range(minSize, maxSize);
        transform.localScale = new Vector3(starSize, starSize, 1f);

        // Tính điểm theo kích thước
        float t = (starSize - minSize) / (maxSize - minSize);

        if (t < 0.33f)
        {
            starScore = 10;
        }
        else if (t < 0.66f)
        {
            starScore = 20;
        }
        else
        {
            starScore = 30;
        }

        // Audio setup
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
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
        if (collision.CompareTag("Player") && !isCollected)
        {
            CollectStar();
        }
    }

    void CollectStar()
    {
        isCollected = true;

        // Cộng điểm theo kích thước ngôi sao
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(starScore);
        }

        // Chỉ phát âm thanh thu thập nếu có
        if (collectSound && audioSource)
        {
            StartCoroutine(PlaySoundAndDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator PlaySoundAndDestroy()
    {
        spriteRenderer.enabled = false;  // Ẩn ngôi sao
        myCollider.enabled = false;      // Vô hiệu hóa va chạm
        audioSource.PlayOneShot(collectSound);
        yield return new WaitForSeconds(collectSound.length);
        Destroy(gameObject);
    }
}
