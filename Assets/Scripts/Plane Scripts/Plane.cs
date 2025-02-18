using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Plane : MonoBehaviour
{
    public float planeSpeed;
    private Rigidbody2D myBody;

    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Image laserTimeBar;

    private bool canShoot = true;
    public int lives = 3;
    [SerializeField] private List<Image> lifeIcons;
    private bool isInvincible = false;

    private bool isLaserMode = false; // Biến để kiểm tra chế độ laser
    private float nextFireTime = 0f;
    public float fireRate = 0.2f; // Tốc độ bắn thường

    private float laserFireRate = 0.6f; // Tốc độ bắn chậm cho laser
    private float laserDuration = 12f; // Thời gian bắn laser khi nhặt vật phẩm

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        PlaneMovement();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (isLaserMode)
            {
                if (Time.time >= nextFireTime)
                {
                    ShootLaser();
                    nextFireTime = Time.time + laserFireRate;
                }
            }
            else
            {
                if (canShoot)
                {
                    StartCoroutine(ShootBullet());
                }
            }
        }
    }

    void PlaneMovement()
    {
        float xAxis = Input.GetAxisRaw("Horizontal") * planeSpeed;
        float yAxis = Input.GetAxisRaw("Vertical") * planeSpeed;

        myBody.velocity = new Vector2(xAxis, yAxis);

        float targetRotation = -90f;  

        if (xAxis > 0) 
        {
            targetRotation = -90f - 15f;
        }
        else if (xAxis < 0) 
        {
            targetRotation = -90f + 15f;
        }
        else if (yAxis > 0) 
        {
            targetRotation = -90f;
        }
        else if (yAxis < 0) 
        {
            targetRotation = -90f;
        }
        float rotationSpeed = 2.5f; 
        float smoothRotation = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = Quaternion.Euler(0, 0, smoothRotation);
    }

    IEnumerator ShootBullet()
    {
        canShoot = false;
        Vector3 temp = transform.position;
        temp.y += 0.7f;
        Instantiate(bullet, temp, Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }

    void ShootLaser()
    {
        Vector3 shootPosition = transform.position; // Sử dụng vị trí của máy bay
        shootPosition.y += 0.7f; // Để laser xuất hiện phía trước máy bay
        GameObject laser = Instantiate(laserPrefab, shootPosition, Quaternion.Euler(0, 0,90));
        laser.transform.localScale = new Vector3(2f, 2f, 1f);
    }

    public void ActivateLaserMode()
    {
        isLaserMode = true;
        laserTimeBar.gameObject.SetActive(true);
        laserTimeBar.fillAmount = 1f;
        StartCoroutine(DisableLaserModeAfterTime());
    }

    private IEnumerator DisableLaserModeAfterTime()
    {
        float timer = laserDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            laserTimeBar.fillAmount = timer / laserDuration; // Giảm từ 1 về 0
            yield return null;
        }

        isLaserMode = false;
        laserTimeBar.gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Asteroids"))
        {
            if (!isInvincible)
            {
                LoseLife();
            }
        }
        else if (target.CompareTag("HealthPickup"))
        {
            GainLife();
            Destroy(target.gameObject);
        }
    }

    void LoseLife()
    {
        lives--;
        UpdateLivesUI();
        StartCoroutine(InvincibilityFrames());
        Debug.Log("Mạng còn lại: " + lives);

        if (lives <= 0)
        {
            Die();
        }
    }

    public void GainLife()
    {
        if (lives < lifeIcons.Count)
        {
            lives++;
            UpdateLivesUI();
        }
    }

    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeIcons.Count; i++)
        {
            lifeIcons[i].enabled = (i < lives);
        }
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.2f);
        }
        sr.enabled = true;
        isInvincible = false;
    }

    void Die()
    {
        Debug.Log("Máy bay đã bị hạ!");
        gameObject.SetActive(false);
        GamePlay.instance.PlaneDiedShow();
    }
}
