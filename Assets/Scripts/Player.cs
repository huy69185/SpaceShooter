using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
/// <summary>
/// Quản lý hành vi của người chơi (Player), bao gồm di chuyển, bắn đạn, nhận sát thương, và game over.
/// Kế thừa từ Singleton để đảm bảo chỉ có một instance duy nhất tồn tại.
/// </summary>
public class Player : Singleton<Player>
{
[Header("References")] // Tham chiếu đến các thành phần liên quan đến người chơi
    [SerializeField] public Rigidbody2D Rb; // Rigidbody2D để xử lý vật lý di chuyển
    [SerializeField] public SpriteRenderer Sprite; // Sprite của người chơi
    [SerializeField] private Projectile _normalProjectile; // Đạn thường
    [SerializeField] private Projectile _chargedProjectile; // Đạn sạc
    [SerializeField] private Transform _projectileSpawn; // Vị trí bắn đạn
    [SerializeField] private GameObject _engineThrust; // Hiệu ứng động cơ
    [SerializeField] private GameObject _explosionParticleGO; // Hiệu ứng nổ khi chết
    [SerializeField] private List<SpriteRenderer> _lifeBarsList = new(); // Danh sách thanh máu
    [SerializeField] private SpriteRenderer _lifeBarBackground; // Nền của thanh máu
    [SerializeField] private ParticleSystem _chargeParticle; // Hiệu ứng sạc đạn
    [SerializeField] private Material _normalMaterial; // Material bình thường
    [SerializeField] private Material _chargedMaterial; // Material khi sạc
    [SerializeField] private Transform _cameraTransform; // Camera để tạo hiệu ứng rung khi nhận sát thương

    [Header("Values")] // Các giá trị gameplay
    [SerializeField] public float MoveSpeed; // Tốc độ di chuyển
    [SerializeField] public float RotationSpeed; // Tốc độ xoay
    [SerializeField] public int Lifes; // Số mạng
    [SerializeField] public float MaxInertiaScale; // Hệ số quán tính tối đa
    [SerializeField] private float _minInertiaScale; // Hệ số quán tính tối thiểu
    [SerializeField] private float _projectileSpeed; // Tốc độ bắn đạn
    [SerializeField] private float _maxChargeTime; // Thời gian sạc tối đa
    [SerializeField] private float _particleStartLifeTimeScale; // Thời gian sống của hạt khi sạc đạn

    [HideInInspector] public bool MovedFirstTime; // Kiểm tra người chơi đã di chuyển lần đầu tiên chưa
    [HideInInspector] public float InertiaScale = 0.3f; // Hệ số quán tính ban đầu
    [HideInInspector] public Vector2 Direction = new Vector2(-1, 0); // Hướng di chuyển mặc định
    [HideInInspector] public bool InputShootPressed; // Kiểm tra người chơi có đang bấm bắn không

    private float _cameraHeight;
    private float _cameraWidth;
    private bool _canShoot = true;
    private float _chargeTime;
    private bool _charging;
    private bool _invulnerable;
    /// <summary>
    /// Khởi tạo kích thước camera để sử dụng trong việc di chuyển màn hình.
    /// </summary>
    private void Start()
    {
          Rb = GetComponent<Rigidbody2D>();
        _cameraHeight = GlobalManager.Instance.CameraHeight;
        _cameraWidth = GlobalManager.Instance.CameraWidth;
    }
    /// <summary>
    /// Kiểm tra và reposition nếu người chơi đi ra ngoài màn hình.
    /// </summary>    
    private void Update()
    {
        RepositionOnLeftScreen();
    }
    /// <summary>
    /// Xử lý logic sạc đạn và bắn đạn.
    /// </summary>
    private void FixedUpdate()
    {
        if (PauseManager.Instance.Paused || !StartManager.Instance.GameStarted || Lifes < 0) return;

        if (InputShootPressed) ChargeShoot();
        if (!InputShootPressed && _canShoot && _charging) StartCoroutine(Shoot());
    }
    /// <summary>
    /// Xử lý quán tính khi di chuyển.
    /// </summary>
    public void Inertia()
    {
        if (InertiaScale > _minInertiaScale)
        {
            InertiaScale -= Time.fixedDeltaTime / 10;
        }
        else
        {
            InertiaScale = _minInertiaScale;
        }

        if (MovedFirstTime)
        {
            Rb.velocity = Direction * MoveSpeed * InertiaScale * Time.fixedDeltaTime;
        }
    }

    public void ShowEngineThrusts()
    {
        _engineThrust.SetActive(true);
    }

    public void HideEngineThrusts()
    {
        _engineThrust.SetActive(false);
    }
    /// <summary>
    /// Xử lý logic sạc đạn.
    /// </summary>
    private void ChargeShoot()
    {
        if (!_charging)
        {
            _charging = true;
            _chargeParticle.Play();
            AudioManager.Instance.LoopSfx("ChargingAttack", true);
        }

        _chargeTime += Time.fixedDeltaTime;

        MainModule chargeParticleMain = _chargeParticle.main;
        if (_chargeTime <= _maxChargeTime)
        {
            chargeParticleMain.startLifetimeMultiplier = _particleStartLifeTimeScale * _chargeTime;
            AudioManager.Instance.SetPitchLoopSfx("ChargingAttack", 0.75f + _chargeTime / 2);
        }
        if (_chargeTime > _maxChargeTime && chargeParticleMain.startLifetimeMultiplier != 0.2f)
        {
            chargeParticleMain.startLifetimeMultiplier = _particleStartLifeTimeScale;
            Sprite.material = _chargedMaterial;
        }
    }

    /// <summary>
    /// Xử lý bắn đạn.
    /// </summary>
    private IEnumerator Shoot()
    {
        _charging = false;
        _canShoot = false;
        _chargeParticle.Stop();
        Sprite.material = _normalMaterial;

        AudioManager.Instance.LoopSfx("ChargingAttack", false);
        AudioManager.Instance.SetPitchLoopSfx("ChargingAttack", 0.75f);

        if (_chargeTime >= _maxChargeTime)
        {
            InstantiateProjectile(_chargedProjectile);
            AudioManager.Instance.PlaySfx("ChargedShoot");
        }
        else
        {
            InstantiateProjectile(_normalProjectile);
            AudioManager.Instance.PlaySfx("NormalShoot");
        }

        _chargeTime = 0;

        yield return new WaitForSeconds(0.5f);

        _canShoot = true;
    }

    /// <summary>
    /// Tạo đạn và đặt hướng di chuyển.
    /// </summary>
    private void InstantiateProjectile(Projectile projectile)
    {
        Projectile instantiatedProjectile = Instantiate(projectile);
        instantiatedProjectile.transform.position = _projectileSpawn.position;
        float angle = (Sprite.transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        instantiatedProjectile.Rb.velocity = direction * _projectileSpeed * Time.fixedDeltaTime;
        instantiatedProjectile.transform.Rotate(Sprite.transform.eulerAngles);
    }

    private void TakeDamage()
    {
        if (_invulnerable) return; 
        
        _invulnerable = true;

        Lifes--;

        HandleLifeBars();

        Sprite.DOFade(0, 0.1f).SetLoops(10, LoopType.Yoyo).OnComplete(() => _invulnerable = false);

        if (Lifes < 0)
        {
            
            GameOver();
        }
        else
        {
            AudioManager.Instance.PlaySfx("PlayerTakeDamage");
            _cameraTransform.DOMove(new Vector3(0.3f, 0.2f, _cameraTransform.position.z), 0.1f).SetLoops(10, LoopType.Yoyo);
        }
    }

    private void HandleLifeBars()
    {
        if (Lifes < 0) return;

        _lifeBarsList[Lifes].transform.DOScaleX(0, 0.4f).OnComplete(() =>
        {
            if (Lifes == 2)
            {
                _lifeBarsList[0].color = new Color(230, 230, 0);
                _lifeBarsList[1].color = new Color(230, 230, 0);
            }

            if (Lifes == 1)
            {
                _lifeBarsList[0].color = new Color(230, 0, 0);
            }

            if (Lifes == 0)
            {
                _lifeBarBackground.enabled = false;//
            }
        });
    }

    private void GameOver()
    {
        Sprite.enabled = false;
        _explosionParticleGO.SetActive(true);
        AsteroidSpawnerManager.Instance.DestroySpawnedAsteroids();
        HideEngineThrusts();

        AudioManager.Instance.PlaySfx("GameOver");

        StartCoroutine(GameOverManager.Instance.RunAnimation());
    }

    private void RepositionOnLeftScreen()
    {
        switch (GlobalManager.Instance.OutCameraViewSide(transform.position, new Vector2(1, 1)))
        {
            case SideType.Left:
                transform.position = new Vector2(_cameraWidth / 2 + 1, transform.position.y);
                break;
            case SideType.Right:
                transform.position = new Vector2(-_cameraWidth / 2 - 1, transform.position.y);
                break;
            case SideType.Top:
                transform.position = new Vector2(transform.position.x, -_cameraHeight / 2 - 1);
                break;
            case SideType.Bot:
                transform.position = new Vector2(transform.position.x, _cameraHeight / 2 + 1);
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Asteroid" || collision.tag == "MiniAsteroid")
        {
            TakeDamage();
            Destroy(collision.gameObject);
        }
    }
}
