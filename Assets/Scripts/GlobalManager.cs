using UnityEngine;

/// <summary>
/// GlobalManager quản lý các thông tin toàn cục như kích thước camera và kiểm tra vật thể có nằm ngoài camera hay không.
/// Kế thừa từ Singleton để đảm bảo chỉ có một instance duy nhất tồn tại trong game.
/// </summary>
public class GlobalManager : Singleton<GlobalManager>
{
    [Header("References")]
    [SerializeField] public Camera _camera; // Tham chiếu đến Camera chính trong game

    [HideInInspector] public float CameraHeight; // Chiều cao của Camera
    [HideInInspector] public float CameraWidth; // Chiều rộng của Camera

    /// <summary>
    /// Hàm Awake() được gọi khi game khởi động.
    /// Tính toán kích thước của Camera dựa trên aspect ratio và orthographic size.
    /// </summary>
    public override void Awake()
    {
        base.Awake(); // Gọi Awake() từ lớp Singleton để đảm bảo tính đơn nhất

        // Tính toán chiều cao của Camera (2 lần kích thước orthographic)
        CameraHeight = 2f * _camera.orthographicSize;

        // Tính toán chiều rộng của Camera dựa trên aspect ratio
        CameraWidth = CameraHeight * _camera.aspect;
    }

    /// <summary>
    /// Kiểm tra xem một vật thể có nằm ngoài phạm vi hiển thị của Camera hay không.
    /// </summary>
    /// <param name="pos">Vị trí hiện tại của vật thể.</param>
    /// <param name="size">Kích thước của vật thể.</param>
    /// <returns>Trả về true nếu vật thể nằm ngoài Camera, ngược lại trả về false.</returns>
    public bool OutCameraView(Vector2 pos, Vector2 size)
    {
        if
        (
            pos.x + size.x < _camera.transform.position.x - CameraWidth / 2 || // Nằm ngoài bên trái
            pos.x - size.x > _camera.transform.position.x + CameraWidth / 2 || // Nằm ngoài bên phải
            pos.y + size.y < _camera.transform.position.y - CameraHeight / 2 || // Nằm ngoài bên dưới
            pos.y - size.y > _camera.transform.position.y + CameraHeight / 2 // Nằm ngoài bên trên
        )
        {
            return true; // Vật thể nằm ngoài Camera
        }
        else
        {
            return false; // Vật thể vẫn nằm trong Camera
        }
    }

    /// <summary>
    /// Xác định vật thể đã đi ra ngoài màn hình ở phía nào.
    /// </summary>
    /// <param name="pos">Vị trí của vật thể.</param>
    /// <param name="size">Kích thước của vật thể.</param>
    /// <returns>Trả về SideType tương ứng với cạnh mà vật thể đã vượt qua.</returns>
    public SideType OutCameraViewSide(Vector2 pos, Vector2 size)
    {
        if (pos.x + size.x < _camera.transform.position.x - CameraWidth / 2)
            return SideType.Left; // Nằm ngoài bên trái
        if (pos.x - size.x > _camera.transform.position.x + CameraWidth / 2)
            return SideType.Right; // Nằm ngoài bên phải
        if (pos.y - size.y > _camera.transform.position.y + CameraHeight / 2)
            return SideType.Top; // Nằm ngoài bên trên
        if (pos.y + size.y < _camera.transform.position.y - CameraHeight / 2)
            return SideType.Bot; // Nằm ngoài bên dưới

        return SideType.Null; // Vẫn nằm trong Camera
    }
}
