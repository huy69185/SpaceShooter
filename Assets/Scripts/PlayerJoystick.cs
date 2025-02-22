using UnityEngine;

public class PlayerJoystick : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Joystick _joystick; // Tham chiếu đến joystick để điều khiển nhân vật

    private bool _inputShootPressed; // Biến kiểm tra trạng thái nhấn nút bắn

    private void Update()
    {
        // Cập nhật trạng thái bắn của người chơi dựa trên _inputShootPressed
        if (_inputShootPressed)
            Player.Instance.InputShootPressed = true;
        else
            Player.Instance.InputShootPressed = false;
    }

    private void FixedUpdate()
    {
        // Kiểm tra nếu người chơi đã hết mạng (Lifes < 0)
        if (Player.Instance.Lifes < 0)
        {
            // Nếu nhân vật vẫn đang di chuyển, dừng lại
            if (Player.Instance.Rb.velocity != Vector2.zero)
                Player.Instance.Rb.velocity = Vector2.zero;
            return;
        }

        // Nếu game đang tạm dừng hoặc chưa bắt đầu, thoát khỏi hàm
        if (PauseManager.Instance.Paused || !StartManager.Instance.GameStarted) return;

        // Gọi các phương thức di chuyển và xoay nhân vật
        Rotate();
        Move();
    }

    /// <summary>
    /// Xoay nhân vật theo hướng của joystick.
    /// </summary>
    private void Rotate()
    {
        if (_joystick.Direction == Vector2.zero) return; // Nếu joystick không di chuyển, không xoay

        // Tính toán góc xoay từ hướng joystick (Mathf.Atan2 trả về góc theo radian, nhân với Rad2Deg để đổi sang độ)
        float rotationInDegrees = Mathf.Atan2(_joystick.Direction.y, _joystick.Direction.x) * Mathf.Rad2Deg + 270;

        // Chuyển đổi góc xoay thành quaternion (hệ tọa độ quay của Unity)
        Quaternion rotationInQuaternion = Quaternion.Euler(0, 0, rotationInDegrees);

        // Xoay nhân vật về hướng joystick với tốc độ quay được giới hạn
        Player.Instance.Sprite.transform.rotation = Quaternion.RotateTowards(
            Player.Instance.Sprite.transform.rotation, rotationInQuaternion, Player.Instance.RotationSpeed * Time.fixedDeltaTime
        );
    }

    /// <summary>
    /// Di chuyển nhân vật theo hướng joystick.
    /// </summary>
    private void Move()
    {
        if (_joystick.Direction != Vector2.zero) // Nếu joystick đang di chuyển
        {
            // Kiểm tra nếu đây là lần đầu tiên di chuyển nhân vật
            if (!Player.Instance.MovedFirstTime)
            {
                Player.Instance.MovedFirstTime = true;
            }

            // Tính toán hướng di chuyển từ góc xoay của nhân vật
            float rotationInDegrees = Player.Instance.Sprite.transform.eulerAngles.z + 90; // Điều chỉnh góc để trùng với hướng di chuyển
            float rotationInRadians = rotationInDegrees * Mathf.Deg2Rad; // Đổi đơn vị từ độ sang radian
            Player.Instance.Direction = new Vector2(Mathf.Cos(rotationInRadians), Mathf.Sin(rotationInRadians)); // Tạo vector hướng

            // Cập nhật vận tốc của nhân vật theo hướng và tốc độ di chuyển
            Player.Instance.Rb.velocity = Player.Instance.Direction * Player.Instance.MoveSpeed * Time.fixedDeltaTime;

            // Nếu quán tính chưa đạt mức tối đa, tăng quán tính
            if (Player.Instance.InertiaScale != Player.Instance.MaxInertiaScale)
            {
                Player.Instance.InertiaScale = Player.Instance.MaxInertiaScale;
            }

            Player.Instance.ShowEngineThrusts(); // Hiển thị hiệu ứng động cơ khi nhân vật di chuyển
        }
        else // Nếu joystick không được di chuyển
        {
            Player.Instance.Inertia(); // Giữ quán tính di chuyển tự nhiên
            Player.Instance.HideEngineThrusts(); // Ẩn hiệu ứng động cơ
        }
    }

    /// <summary>
    /// Xử lý sự kiện bấm nút bắn.
    /// </summary>
    public void ShootButton()
    {
        // Khi nhấn nút bắn, thay đổi trạng thái _inputShootPressed
        _inputShootPressed = !_inputShootPressed;
    }
}
