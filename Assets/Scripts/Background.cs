using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lớp Background xử lý việc di chuyển nền trong game dựa trên chuyển động của người chơi.
/// Các lớp nền bao gồm: ngôi sao, bụi, tinh vân, hành tinh.
/// </summary>
public class Background : MonoBehaviour
{
    [Header("References")] // Tham chiếu đến các lớp nền
    [SerializeField] private List<Transform> _starsList = new(); // Danh sách các ngôi sao
    [SerializeField] private List<Transform> _dustList = new(); // Danh sách các bụi không gian
    [SerializeField] private List<Transform> _nebulaeList = new(); // Danh sách các tinh vân
    [SerializeField] private List<Transform> _planetsList = new(); // Danh sách các hành tinh

    [Header("Values")] // Tốc độ di chuyển của từng lớp nền
    [SerializeField] private float _starsSpeed; // Tốc độ di chuyển của ngôi sao
    [SerializeField] private float _dustSpeed; // Tốc độ di chuyển của bụi
    [SerializeField] private float _nebulaeSpeed; // Tốc độ di chuyển của tinh vân
    [SerializeField] private float _planetsSpeed; // Tốc độ di chuyển của hành tinh

    private Vector2 _playerDir; // Hướng di chuyển của người chơi

    /// <summary>
    /// Hàm Update() chạy mỗi frame, cập nhật vị trí nền theo hướng di chuyển của người chơi.
    /// </summary>
    private void Update()
    {
        // Nếu game đang bị tạm dừng, không thực hiện gì cả
        if (PauseManager.Instance.Paused) return;

        // Tính toán hướng di chuyển của người chơi
        _playerDir = Player.Instance.Rb.velocity / Player.Instance.MoveSpeed;

        // Di chuyển từng lớp nền theo tốc độ tương ứng
        Move(_starsList, _starsSpeed);
        Move(_dustList, _dustSpeed);
        Move(_nebulaeList, _nebulaeSpeed);
        Move(_planetsList, _planetsSpeed);
        
        // Định vị lại các lớp nền khi chúng vượt ra khỏi phạm vi màn hình
        Reposition(_starsList);
        Reposition(_dustList);
        Reposition(_nebulaeList);
        Reposition(_planetsList);
    }

    /// <summary>
    /// Di chuyển danh sách các phần tử nền theo hướng di chuyển của người chơi.
    /// </summary>
    private void Move(List<Transform> bgList, float bgSpeed)
    {
        foreach (Transform bg in bgList)
        {
            bg.position += new Vector3(-_playerDir.x * bgSpeed, -_playerDir.y * bgSpeed, bg.position.z);
        }
    }

    /// <summary>
    /// Định vị lại các lớp nền khi chúng ra khỏi màn hình để tạo hiệu ứng nền vô hạn.
    /// </summary>
    private void Reposition(List<Transform> bgList)
    {
        foreach (Transform bg in bgList)
        {
            // Nếu nền di chuyển quá xa về bên phải và người chơi đang di chuyển sang trái, di chuyển nền về bên trái
            if (bg.position.x > 60 && _playerDir.x < 0)
                bg.position -= new Vector3(120, 0, 0);

            // Nếu nền di chuyển quá xa về bên trái và người chơi đang di chuyển sang phải, di chuyển nền về bên phải
            if (bg.position.x < -60 && _playerDir.x > 0)
                bg.position += new Vector3(120, 0, 0);
            
            // Nếu nền di chuyển quá xa xuống dưới và người chơi đang di chuyển lên trên, di chuyển nền lên trên
            if (bg.position.y < -33.75 && _playerDir.y > 0)
                bg.position += new Vector3(0, 67.5f, 0);

            // Nếu nền di chuyển quá xa lên trên và người chơi đang di chuyển xuống dưới, di chuyển nền xuống dưới
            if (bg.position.y > 33.75 && _playerDir.y < 0)
                bg.position -= new Vector3(0, 67.5f, 0);
        }
    }
}
