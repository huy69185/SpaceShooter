using DG.Tweening; // Import thư viện DOTween để tạo hiệu ứng animation
using TMPro; // Import thư viện để sử dụng TextMeshPro
using UnityEngine;
using UnityEngine.Rendering.Universal; // Import để điều khiển hệ thống ánh sáng trong URP (Universal Render Pipeline)

/// <summary>
/// Lớp Asteroid quản lý hành vi của một tiểu hành tinh trong game.
/// </summary>
public class Asteroid : MonoBehaviour
{
    [Header("References")] // Nhóm các biến reference trong Unity Inspector
    [SerializeField] public Rigidbody2D Rb; // Thành phần vật lý của tiểu hành tinh (cho phép di chuyển, va chạm)
    [SerializeField] private BoxCollider2D _collider; // Collider của tiểu hành tinh để kiểm tra va chạm
    [SerializeField] private Light2D _light; // Hiệu ứng ánh sáng của tiểu hành tinh (chỉ dùng nếu có URP)
    [SerializeField] private SpriteRenderer _sprite; // Thành phần hiển thị hình ảnh của tiểu hành tinh
    [SerializeField] private TMP_Text _pointsIndicator; // TextMeshPro hiển thị điểm khi phá hủy tiểu hành tinh
    [SerializeField] private ParticleSystem _smokeParticle; // Hiệu ứng khói khi tiểu hành tinh bị phá hủy

    [Header("Values")] // Nhóm các giá trị điều chỉnh trong Inspector
    [SerializeField] private int _hits; // Số lần cần bắn trúng để phá hủy tiểu hành tinh
    [SerializeField] private int _points; // Số điểm nhận được khi phá hủy tiểu hành tinh
    [SerializeField] private bool _miniAsteroid; // Biến kiểm tra xem đây có phải tiểu hành tinh nhỏ không

    /// <summary>
    /// Gọi trong mỗi frame để kiểm tra xem tiểu hành tinh có ra khỏi màn hình không.
    /// </summary>
    private void Update()
    {
        DeleteOnOutOfCamera();
    }

    /// <summary>
    /// Kiểm tra nếu tiểu hành tinh ra khỏi phạm vi camera thì sẽ tự hủy.
    /// </summary>
    private void DeleteOnOutOfCamera()
    {
        if (GlobalManager.Instance.OutCameraView(transform.position, _collider.size))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tính điểm khi tiểu hành tinh bị phá hủy và phát hiệu ứng âm thanh tương ứng.
    /// </summary>
    /// <param name="asteroidsDestroyed">Số tiểu hành tinh bị phá hủy.</param>
    private void SetPoints(int asteroidsDestroyed)
    {
        int totalPoints = _points * asteroidsDestroyed;

        // Cộng điểm vào hệ thống điểm số
        ScoreManager.Instance.AddScore(totalPoints);

        // Phát âm thanh dựa trên số điểm nhận được
        if (totalPoints > 10)
            AudioManager.Instance.PlaySfx("HighPoints"); // Âm thanh khi được nhiều điểm
        else
            AudioManager.Instance.PlaySfx("LowPoints"); // Âm thanh khi được ít điểm

        // Hiển thị hiệu ứng điểm số
        SpawnPointsIndicator(totalPoints);
    }

    /// <summary>
    /// Hàm xử lý khi tiểu hành tinh bị trúng đạn.
    /// </summary>
    /// <param name="charged">Kiểm tra xem đạn có phải là đạn đặc biệt không.</param>
    /// <param name="asteroidsDestroyed">Số lượng tiểu hành tinh bị phá hủy.</param>
    public void TakeDamage(bool charged, int asteroidsDestroyed)
    {
        _hits--; // Giảm số lần bắn trúng còn lại

        // Nếu là đạn đặc biệt, phá hủy ngay lập tức
        if (charged)
        {
            SetPoints(asteroidsDestroyed);

            // Xóa khỏi danh sách tiểu hành tinh trong Spawner
            AsteroidSpawnerManager.Instance.SpawnedAsteroids.Remove(gameObject);

            // Tạo hiệu ứng khói
            Instantiate(_smokeParticle, transform.position, Quaternion.identity);
            Destroy(gameObject); // Xóa tiểu hành tinh
            return;
        }

        // Kiểm tra trạng thái còn lại của tiểu hành tinh sau khi bị bắn
        switch(_hits)
        {
            case 0: // Tiểu hành tinh bị phá hủy
                SetPoints(asteroidsDestroyed);

                // Tạo hiệu ứng khói khi bị phá hủy
                Instantiate(_smokeParticle, transform.position, Quaternion.identity);

                // Nếu không phải tiểu hành tinh nhỏ, tạo thêm các tiểu hành tinh con
                if (!_miniAsteroid)
                {
                    AsteroidSpawnerManager.Instance.SpawnMiniAsteroids(transform.position);
                }

                // Xóa khỏi danh sách quản lý và hủy đối tượng
                AsteroidSpawnerManager.Instance.SpawnedAsteroids.Remove(gameObject);
                Destroy(gameObject);
                break;

            case 1: // Tiểu hành tinh sắp bị phá hủy
                _sprite.DOFade(0, 0.3f).SetLoops(-1, LoopType.Yoyo); // Làm hiệu ứng nhấp nháy bằng DOTween
                AudioManager.Instance.PlaySfx("AsteroidTakeDamage"); // Phát âm thanh khi bị bắn
                break;

            case 2: // Tiểu hành tinh mới bị bắn lần đầu
                _light.intensity = 3; // Tăng độ sáng của ánh sáng tiểu hành tinh
                AudioManager.Instance.PlaySfx("AsteroidTakeDamage"); // Phát âm thanh khi bị bắn
                break;
        }
    }

    /// <summary>
    /// Hiển thị hiệu ứng điểm số khi tiểu hành tinh bị phá hủy.
    /// </summary>
    /// <param name="totalPoints">Tổng số điểm nhận được.</param>
    private void SpawnPointsIndicator(int totalPoints)
    {
        // Tạo text hiển thị điểm tại vị trí của tiểu hành tinh
        TMP_Text instantiatedPointsIndicator = Instantiate(_pointsIndicator, transform.position, Quaternion.identity);
        instantiatedPointsIndicator.text = "+" + totalPoints.ToString();

        // Tạo hiệu ứng text di chuyển lên trên rồi mờ dần
        instantiatedPointsIndicator.transform.DOMoveY(instantiatedPointsIndicator.transform.position.y + 1, 1);
        instantiatedPointsIndicator.DOFade(0, 1);
    }
}
