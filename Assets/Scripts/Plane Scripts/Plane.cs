using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plane : MonoBehaviour
{
    // Tốc độ di chuyển của máy bay
    public float planeSpeed;

    // Tham chiếu đến Rigidbody2D để điều khiển chuyển động
    private Rigidbody2D myBody;

    // Đối tượng đạn thường
    [SerializeField] private GameObject bullet;

    // Đối tượng laser
    [SerializeField] private GameObject laserPrefab;

    // Thanh hiển thị thời gian còn lại của laser mode
    [SerializeField] private Image laserTimeBar;

    // Biến kiểm soát việc có thể bắn hay không
    private bool canShoot = true;

    // Số mạng còn lại
    public int lives = 3;

    // Danh sách các biểu tượng mạng sống trên UI
    [SerializeField] private List<Image> lifeIcons;

    // Biến kiểm tra trạng thái bất tử (tránh mất mạng liên tục)
    private bool isInvincible = false;

    // Kiểm tra chế độ laser
    private bool isLaserMode = false;

    // Thời gian tiếp theo có thể bắn
    private float nextFireTime = 0f;

    // Tốc độ bắn thường (khoảng thời gian giữa các viên đạn)
    public float fireRate = 0.2f;

    // Tốc độ bắn khi ở chế độ laser
    private float laserFireRate = 0.6f;

    // Thời gian laser mode tồn tại
    private float laserDuration = 12f;

    void Awake()
    {
        // Lấy component Rigidbody2D của máy bay để điều khiển vật lý
        myBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Gọi hàm di chuyển máy bay trong FixedUpdate để xử lý vật lý mượt mà hơn
        PlaneMovement();
    }

    void Update()
    {
        // Khi người chơi nhấn giữ phím Space để bắn
        if (Input.GetKey(KeyCode.Space))
        {
            if (isLaserMode)
            {
                // Nếu đang ở chế độ laser, chỉ bắn khi đủ thời gian giữa hai phát bắn
                if (Time.time >= nextFireTime)
                {
                    ShootLaser();
                    nextFireTime = Time.time + laserFireRate;
                }
            }
            else
            {
                // Nếu không ở chế độ laser và có thể bắn, thực hiện bắn đạn thường
                if (canShoot)
                {
                    StartCoroutine(ShootBullet());
                }
            }
        }
    }

    void PlaneMovement()
    {
        // Lấy giá trị nhập từ người chơi
        float xAxis = Input.GetAxisRaw("Horizontal") * planeSpeed;
        float yAxis = Input.GetAxisRaw("Vertical") * planeSpeed;

        // Cập nhật vận tốc của máy bay dựa trên input
        myBody.velocity = new Vector2(xAxis, yAxis);

        // Góc quay mặc định của máy bay
        float targetRotation = -90f;

        // Điều chỉnh góc quay khi di chuyển sang trái hoặc phải
        if (xAxis > 0)
        {
            targetRotation = -90f - 15f;
        }
        else if (xAxis < 0)
        {
            targetRotation = -90f + 15f;
        }

        // Làm mượt góc quay
        float rotationSpeed = 2.5f;
        float smoothRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
    }

    IEnumerator ShootBullet()
    {
        // Đặt canShoot = false để tránh bắn liên tục
        canShoot = false;

        // Xác định vị trí xuất hiện của viên đạn
        Vector3 temp = transform.position;
        temp.y += 0.7f;

        // Tạo một viên đạn mới
        Instantiate(bullet, temp, Quaternion.identity);

        // Chờ một khoảng thời gian để đảm bảo tốc độ bắn không quá nhanh
        yield return new WaitForSeconds(fireRate);
        canShoot = true;
    }

    void ShootLaser()
    {
        // Xác định vị trí bắn laser
        Vector3 shootPosition = transform.position;
        shootPosition.y += 0.7f;

        // Tạo laser mới và điều chỉnh kích thước
        GameObject laser = Instantiate(laserPrefab, shootPosition, Quaternion.Euler(0, 0, 90));
        laser.transform.localScale = new Vector3(2f, 2f, 1f);
    }

    public void ActivateLaserMode()
    {
        // Kích hoạt chế độ laser
        isLaserMode = true;
        laserTimeBar.gameObject.SetActive(true);
        laserTimeBar.fillAmount = 1f;

        // Bắt đầu bộ đếm thời gian để vô hiệu hóa laser sau một khoảng thời gian
        StartCoroutine(DisableLaserModeAfterTime());
    }

    private IEnumerator DisableLaserModeAfterTime()
    {
        float timer = laserDuration;

        // Giảm dần thời gian của chế độ laser
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            laserTimeBar.fillAmount = timer / laserDuration;
            yield return null;
        }

        // Kết thúc chế độ laser
        isLaserMode = false;
        laserTimeBar.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        // Kiểm tra va chạm với thiên thạch
        if (target.CompareTag("Asteroids"))
        {
            if (!isInvincible)
            {
                LoseLife();
            }
        }
    }

    void LoseLife()
    {
        // Giảm số mạng khi bị va chạm
        lives--;

        // Cập nhật UI hiển thị mạng sống
        UpdateLivesUI();

        // Kích hoạt chế độ bất tử tạm thời để tránh mất mạng liên tục
        StartCoroutine(InvincibilityFrames());

        Debug.Log("Mạng còn lại: " + lives);

        // Nếu hết mạng, máy bay bị phá hủy
        if (lives <= 0)
        {
            Die();
        }
    }

    public void GainLife()
    {
        // Tăng mạng sống nếu chưa đạt tối đa
        if (lives < lifeIcons.Count)
        {
            lives++;
            UpdateLivesUI();
        }
    }

    void UpdateLivesUI()
    {
        // Cập nhật UI để hiển thị đúng số mạng còn lại
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            lifeIcons[i].enabled = (i < lives);
        }
    }

    IEnumerator InvincibilityFrames()
    {
        // Kích hoạt trạng thái bất tử tạm thời
        isInvincible = true;

        // Nhấp nháy Sprite Renderer để biểu thị trạng thái vô địch
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
        }

        // Kết thúc trạng thái bất tử
        sr.enabled = true;
        isInvincible = false;
    }

    void Die()
    {
        // In ra log khi máy bay bị phá hủy
        Debug.Log("Máy bay đã bị hạ!");

        // Ẩn đối tượng máy bay
        gameObject.SetActive(false);

        // Gọi sự kiện kết thúc game từ gameplay manager
        GamePlay.instance.PlaneDiedShow();
    }
}
