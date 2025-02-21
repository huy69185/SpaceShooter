using TMPro; // Import TextMeshPro để hiển thị text UI
using UnityEngine;

/// <summary>
/// BuildManager xử lý cài đặt UI và điều khiển phù hợp với nền tảng chạy game (Android, PC, WebGL).
/// </summary>
public class BuildManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _startText; // Text hiển thị khi bắt đầu game
    [SerializeField] private TMP_Text _playAgainText; // Text hiển thị khi chơi lại
    [SerializeField] private GameObject _joystick; // Joystick điều khiển cho mobile
    [SerializeField] private GameObject _shootButton; // Nút bắn dành cho mobile
    [SerializeField] private PlayerKeyboard _playerKeyboard; // Điều khiển bằng bàn phím (PC)
    [SerializeField] private PlayerJoystick _playerJoystick; // Điều khiển bằng joystick (Mobile)
    [SerializeField] private GameObject _resolutions; // Cài đặt độ phân giải (PC)
    [SerializeField] private GameObject _fullScreen; // Nút chuyển đổi fullscreen (PC)
    [SerializeField] private GameObject _controlsBtn; // Nút chỉnh cài đặt điều khiển (PC)

    /// <summary>
    /// Hàm Awake() kiểm tra nền tảng và thiết lập UI/điều khiển phù hợp.
    /// </summary>
    private void Awake()
    {
        // Nếu chạy trên Android (điện thoại, máy tính bảng)
        if (Application.platform == RuntimePlatform.Android)
        {
            _startText.text = "tap to start"; // Đổi text thành "Chạm để bắt đầu"
            _playAgainText.text = "tap to play again"; // Đổi text thành "Chạm để chơi lại"
            _joystick.SetActive(true); // Hiển thị joystick
            _shootButton.SetActive(true); // Hiển thị nút bắn
            _playerKeyboard.enabled = false; // Tắt điều khiển bằng bàn phím
            _playerJoystick.enabled = true; // Bật điều khiển bằng joystick
            _resolutions.SetActive(false); // Ẩn cài đặt độ phân giải
            _fullScreen.SetActive(false); // Ẩn nút fullscreen
            _controlsBtn.SetActive(false); // Ẩn nút cài đặt điều khiển
        }

        // Nếu chạy trên PC (Windows, macOS, Linux)
        if (Application.platform != RuntimePlatform.Android)
        {
            _joystick.SetActive(false); // Ẩn joystick
            _shootButton.SetActive(false); // Ẩn nút bắn
            _playerKeyboard.enabled = true; // Bật điều khiển bằng bàn phím
            _playerJoystick.enabled = false; // Tắt điều khiển bằng joystick
            _fullScreen.SetActive(true); // Hiển thị nút fullscreen
            _controlsBtn.SetActive(true); // Hiển thị nút chỉnh điều khiển
        }

        // Nếu chạy trên trình duyệt (WebGL)
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _resolutions.SetActive(false); // Ẩn cài đặt độ phân giải (WebGL không hỗ trợ thay đổi độ phân giải)
        }
    }
}
