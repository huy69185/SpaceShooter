using UnityEngine;

/// <summary>
/// Quản lý hành vi của đạn (Projectile) trong game.
/// Đạn có thể là đạn thường hoặc đạn sạc, hủy khi ra khỏi màn hình hoặc va chạm với thiên thạch.
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D Rb; // Rigidbody2D để xử lý chuyển động
    [SerializeField] private BoxCollider2D _collider; // Collider của đạn để kiểm tra va chạm

    [Header("Values")]
    [SerializeField] private bool _charged; // Kiểm tra xem đây có phải là đạn sạc không

    private int _asteroidsDestroyed; // Đếm số lượng thiên thạch bị phá hủy bởi viên đạn

    /// <summary>
    /// Kiểm tra nếu đạn ra khỏi màn hình thì xóa nó.
    /// </summary>
    private void Update()
    {
        DeleteOnOutOfCamera();
    }

    /// <summary>
    /// Nếu đạn bay ra khỏi màn hình, tự động hủy 
    /// </summary>
    private void DeleteOnOutOfCamera()
    {
        if (GlobalManager.Instance.OutCameraView(transform.position, _collider.size))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Kiểm tra va chạm giữa đạn và thiên thạch.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Asteroid") // Nếu va chạm với thiên thạch
        {
            _asteroidsDestroyed++; // Tăng số lượng thiên thạch bị phá hủy

            collision.GetComponent<Asteroid>().TakeDamage(_charged, _asteroidsDestroyed); // Gọi phương thức TakeDamage() của thiên thạch
            
            if (!_charged) Destroy(gameObject); // Nếu là đạn thường, hủy đạn sau khi va chạm
        }
    }
}
