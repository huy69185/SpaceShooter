using System;
using System.Collections.Generic;
using TMPro; // Thư viện hỗ trợ TextMeshPro (sử dụng cho UI)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Hỗ trợ quản lý các Scene trong Unity

public class PauseManager : Singleton<PauseManager> // Lớp quản lý Pause (tạm dừng game), sử dụng Singleton để đảm bảo chỉ có một thể hiện duy nhất
{
    [SerializeField] private GameObject _pausePanel; // Panel giao diện khi game bị tạm dừng
    [SerializeField] private GameObject _controlsPanel; // Panel giao diện hướng dẫn điều khiển
    [SerializeField] private TMP_Dropdown _resolutionsDropdown; // Dropdown để chọn độ phân giải màn hình
    [SerializeField] private Toggle _fullScreenToggle; // Toggle để bật/tắt chế độ fullscreen

    [HideInInspector] public bool Paused; // Biến lưu trạng thái tạm dừng game (ẩn trên Inspector)

    private Resolution[] _resolutions; // Mảng chứa danh sách các độ phân giải màn hình có thể chọn

    private void Start()
    {
        InitializeResolutionDropdown(); // Khởi tạo danh sách độ phân giải vào dropdown

        _fullScreenToggle.isOn = Screen.fullScreen; // Gán trạng thái fullscreen cho toggle
    }

    private void Update()
    {
        // Kiểm tra nếu đang chạy trên Windows và nhấn phím ESC, đồng thời game đã bắt đầu và người chơi còn mạng
        if (Application.platform == RuntimePlatform.WindowsPlayer && Input.GetKeyDown(KeyCode.Escape) && StartManager.Instance.GameStarted && Player.Instance.Lifes >= 0)
        {
            PlayAndPause(); // Gọi hàm chuyển đổi trạng thái pause
        }
    }

    /// <summary>
    /// Chuyển đổi giữa trạng thái chơi và tạm dừng game.
    /// </summary>
    public void PlayAndPause()
    {
        if (!Paused) // Nếu game chưa bị tạm dừng
        {
            Paused = true; // Đặt trạng thái thành tạm dừng

            Time.timeScale = 0; // Dừng toàn bộ thời gian trong game

            _pausePanel.SetActive(true); // Hiển thị giao diện Pause
        }
        else // Nếu game đang tạm dừng
        {
            Paused = false; // Đặt trạng thái thành tiếp tục chơi

            Time.timeScale = 1; // Khôi phục thời gian trong game

            _pausePanel.SetActive(false); // Ẩn giao diện Pause
        }
    }

    /// <summary>
    /// Đóng giao diện Pause và tiếp tục game.
    /// </summary>
    public void ClosePauseBtn()
    {
        Paused = false;
        Time.timeScale = 1; // Khôi phục thời gian trong game
        _pausePanel.SetActive(false); // Ẩn giao diện Pause

        AudioManager.Instance.PlaySfx("ButtonClick"); // Phát âm thanh khi nhấn nút
    }

    /// <summary>
    /// Mở giao diện hướng dẫn điều khiển.
    /// </summary>
    public void ControlsBtn()
    {
        _controlsPanel.SetActive(true); // Hiển thị panel điều khiển
        AudioManager.Instance.PlaySfx("ButtonClick"); // Phát âm thanh khi nhấn nút
    }

    /// <summary>
    /// Đóng giao diện hướng dẫn điều khiển.
    /// </summary>
    public void CloseControlsBtn()
    {
        _controlsPanel.SetActive(false); // Ẩn panel điều khiển
        AudioManager.Instance.PlaySfx("ButtonClick"); // Phát âm thanh khi nhấn nút
    }

    /// <summary>
    /// Khởi tạo danh sách độ phân giải vào dropdown.
    /// </summary>
    private void InitializeResolutionDropdown()
    {
        _resolutions = Screen.resolutions; // Lấy danh sách độ phân giải có sẵn trên thiết bị

        _resolutionsDropdown.ClearOptions(); // Xóa tất cả các lựa chọn cũ trong dropdown

        List<string> options = new List<string>(); // Danh sách để lưu các độ phân giải dưới dạng chuỗi

        int currentResolutionIndex = 0; // Biến lưu chỉ số độ phân giải hiện tại
        for (int i = 0; i < _resolutions.Length; i++)
        {
            // Tạo chuỗi hiển thị cho từng độ phân giải (VD: "1920 x 1080 @60Hz")
            string option = _resolutions[i].width + " x " + _resolutions[i].height + " @" + Math.Round(_resolutions[i].refreshRateRatio.value) + "Hz";

            options.Add(option); // Thêm độ phân giải vào danh sách

            // Nếu độ phân giải hiện tại trùng với độ phân giải đang sử dụng
            if (Screen.width == _resolutions[i].width && Screen.height == _resolutions[i].height)
            {
                currentResolutionIndex = i; // Lưu chỉ số của độ phân giải hiện tại
            }
        }

        _resolutionsDropdown.AddOptions(options); // Thêm danh sách độ phân giải vào dropdown
        _resolutionsDropdown.value = currentResolutionIndex; // Chọn độ phân giải hiện tại
        _resolutionsDropdown.RefreshShownValue(); // Cập nhật giao diện dropdown
    }

    /// <summary>
    /// Thay đổi chế độ Fullscreen.
    /// </summary>
    /// <param name="enabled">Trạng thái bật/tắt fullscreen</param>
    public void FullScreen(bool enabled)
    {
        Screen.fullScreen = enabled; // Bật hoặc tắt fullscreen dựa vào giá trị truyền vào
    }

    /// <summary>
    /// Thay đổi độ phân giải màn hình theo lựa chọn.
    /// </summary>
    /// <param name="resolutionIndex">Chỉ số của độ phân giải trong dropdown</param>
    public void ResolutionItem(int resolutionIndex)
    {
        Resolution selectedResolution = _resolutions[resolutionIndex]; // Lấy độ phân giải được chọn

        // Đặt độ phân giải màn hình với chế độ fullscreen hoặc cửa sổ
        Screen.SetResolution(selectedResolution.width, selectedResolution.height,
            Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed,
            selectedResolution.refreshRateRatio);
    }

    /// <summary>
    /// Quay về Main Menu mà không lưu game.
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Đảm bảo game không bị dừng khi quay lại menu
        SceneManager.LoadScene("MainMenu"); // Tải scene Main Menu (đổi "MainMenu" thành tên scene chính xác)
    }
}
