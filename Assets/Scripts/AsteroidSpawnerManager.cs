using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Quản lý việc sinh ra và điều khiển các tiểu hành tinh (Asteroid) trong game.
/// Kế thừa từ Singleton để đảm bảo chỉ có một instance duy nhất tồn tại.
/// </summary>
public class AsteroidSpawnerManager : Singleton<AsteroidSpawnerManager>
{
    [Header("References")]
    [SerializeField] private List<Asteroid> _asteroidsList = new();// Danh sách chứa các thiên thạch lớn
    [SerializeField] private List<Asteroid> _miniAsteroidsList = new();// Danh sách chứa các thiên thạch nhỏ

    [Header("Values")]
    [SerializeField] private float _waitTime;// Thời gian chờ giữa các lần spawn thiên thạch
    [SerializeField] private float _minWaitTime;// Thời gian chờ tối thiểu giữa các lần spawn
    [SerializeField] private float _waitTimePercentScale; // Tỷ lệ giảm thời gian chờ theo phần trăm (%)
    [SerializeField] private float _secondsBetweenScales;// Khoảng thời gian giữa mỗi lần giảm thời gian chờ
    [SerializeField] private float _minAsteroidSpeed;// Tốc độ nhỏ nhất của thiên thạch lớn
    [SerializeField] private float _maxAsteroidSpeed;// Tốc độ lớn nhất của thiên thạch lớn
    [SerializeField] private float _minMiniAsteroidSpeed;// Tốc độ nhỏ nhất của thiên thạch nhỏ
    [SerializeField] private float _maxMiniAsteroidSpeed;// Tốc độ lớn nhất của thiên thạch nhỏ

    [HideInInspector] public List<GameObject> SpawnedAsteroids;

    private float _cameraHeight;
    private float _cameraWidth;
    private bool _canSpawn = true;
    private int _curWaitTimeScale;
    private float _curTime;

    /// <summary>
    /// Khởi tạo giá trị camera khi game bắt đầu.
    /// </summary>
    private void Start()
    {
        _cameraHeight = GlobalManager.Instance.CameraHeight;
        _cameraWidth = GlobalManager.Instance.CameraWidth;
    }
    /// <summary>
    /// Cập nhật liên tục để giảm thời gian chờ spawn asteroid theo thời gian.
    /// </summary>
    private void Update()
    {
        if (Player.Instance.Lifes < 0) return;// Nếu người chơi hết mạng, không thực hiện gì cả

        ScaleWaitTime(); // Điều chỉnh thời gian spawn dựa theo thời gian chơi
    }

    /// <summary>
    /// Kiểm tra điều kiện spawn asteroid trong mỗi FixedUpdate (ổn định hơn so với Update).
    /// </summary>
    private void FixedUpdate()
    {
        if (Player.Instance.Lifes < 0) return;

        if (_canSpawn && StartManager.Instance.GameStarted) StartCoroutine(Spawn());
    }
    /// <summary>
    /// Giảm dần thời gian chờ spawn asteroid theo thời gian.
    /// </summary>
    private void ScaleWaitTime()
    {
        _curTime += Time.deltaTime;// Cập nhật thời gian trôi qua

        if (_curWaitTimeScale < _curTime / _secondsBetweenScales && _waitTime > _minWaitTime)
        {
            _curWaitTimeScale++;// Tăng cấp độ giảm thời gian chờ

            if (_waitTime - _waitTime * _waitTimePercentScale > _minWaitTime)
                _waitTime -= _waitTime * _waitTimePercentScale;
            else
                _waitTime = _minWaitTime;// Đảm bảo không giảm dưới mức tối thiểu
        }
    }
    /// <summary>
    /// Coroutine sinh ra một asteroid mới.
    /// </summary>
    private IEnumerator Spawn()
    {
        _canSpawn = false;// Tạm thời dừng spawn để chờ thời gian

        if (Player.Instance.Lifes >= 0)
        {
            Asteroid asteroid = Instantiate(GetRandomAsteroid());// Chọn asteroid ngẫu nhiên
            SetPosDirSpeed(asteroid); // Đặt vị trí, hướng, tốc độ
            SpawnedAsteroids.Add(asteroid.gameObject); // Lưu vào danh sách asteroid đang tồn tại
        }

        yield return new WaitForSeconds(_waitTime);// Chờ theo thời gian _waitTime

        _canSpawn = true;
    }
    /// <summary>
    /// Lấy một asteroid ngẫu nhiên từ danh sách.
    /// </summary>
    private Asteroid GetRandomAsteroid()
    {
        int randAsteroidIndex = UnityEngine.Random.Range(0, _asteroidsList.Count - 1);

        return _asteroidsList[randAsteroidIndex];
    }
    /// <summary>
    /// Lấy một mini asteroid ngẫu nhiên từ danh sách.
    /// </summary>
    private Asteroid GetRandomMiniAsteroid()
    {
        int randAsteroidIndex = UnityEngine.Random.Range(0, _asteroidsList.Count - 1);

        return _miniAsteroidsList[randAsteroidIndex];
    }

    /// <summary>
    /// Thiết lập vị trí, hướng, tốc độ cho asteroid khi xuất hiện.
    /// </summary>
    private void SetPosDirSpeed(Asteroid asteroid)
    {
        float randSpeed = UnityEngine.Random.Range(_minAsteroidSpeed, _maxAsteroidSpeed);
        int randDirType = UnityEngine.Random.Range(0, Enum.GetNames(typeof(DirType)).Length);
        float randPos = 0;
        Vector2 randDir = Vector2.zero;

        switch ((DirType)randDirType)
        {
            case DirType.Up:
                randPos = UnityEngine.Random.Range(-_cameraWidth / 2, _cameraWidth / 2);
                asteroid.transform.position = new Vector2(randPos, _cameraHeight / 2);
                randDir = new Vector2(RandomDir(randPos), -1);
                
                break;

            case DirType.Down:
                randPos = UnityEngine.Random.Range(-_cameraWidth / 2, _cameraWidth / 2);
                asteroid.transform.position = new Vector2(randPos, -_cameraHeight / 2);
                randDir = new Vector2(RandomDir(randPos), 1);
                break;

            case DirType.Left:
                randPos = UnityEngine.Random.Range(-_cameraHeight / 2, _cameraHeight / 2);
                asteroid.transform.position = new Vector2(-_cameraWidth / 2, randPos);
                randDir = new Vector2(1, RandomDir(randPos));
                break;

            case DirType.Right:
                randPos = UnityEngine.Random.Range(-_cameraHeight / 2, _cameraHeight / 2);
                asteroid.transform.position = new Vector2(_cameraWidth / 2, randPos);
                randDir = new Vector2(-1, RandomDir(randPos));
                break;
        }

        asteroid.Rb.velocity = randDir * randSpeed * Time.fixedDeltaTime;
    }
    /// <summary>
    /// Hàm hỗ trợ để tạo hướng di chuyển ngẫu nhiên.
    /// </summary>
    private float RandomDir(float pos)
    {
        if (pos < 0)
            return UnityEngine.Random.Range(0f, 1f);
        else
            return UnityEngine.Random.Range(-1f, 0f);
    }
    /// <summary>
    /// Spawn các tiểu hành tinh nhỏ khi một tiểu hành tinh lớn bị phá hủy.
    /// </summary>
    public void SpawnMiniAsteroids(Vector2 startPos)
    {
        int randQuantity = UnityEngine.Random.Range(2, 4);

        for(int i = 0; i < randQuantity; i++)
        {
            Asteroid miniAsteroid = Instantiate(GetRandomMiniAsteroid(), startPos, Quaternion.identity);
            Vector2 randDir = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            float randSpeed = UnityEngine.Random.Range(_minMiniAsteroidSpeed, _maxMiniAsteroidSpeed);
            miniAsteroid.Rb.velocity = randDir * randSpeed * Time.fixedDeltaTime;
            SpawnedAsteroids.Add(miniAsteroid.gameObject);
        }
    }

    public void DestroySpawnedAsteroids()
    {
        for (int i = 0; i < SpawnedAsteroids.Count; i++)
        {
            Destroy(SpawnedAsteroids[i]);
        }
    }
}
