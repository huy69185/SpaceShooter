using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : Singleton<PauseManager>
{
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private TMP_Dropdown _resolutionsDropdown;
    [SerializeField] private Toggle _fullScreenToggle;

    [HideInInspector] public bool Paused;

    private Resolution[] _resolutions;

    private void Start()
    {
        InitializeResolutionDropdown();

        _fullScreenToggle.isOn = Screen.fullScreen;
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer && Input.GetKeyDown(KeyCode.Escape) && StartManager.Instance.GameStarted && Player.Instance.Lifes >= 0)
        {
            PlayAndPause();
        }
    }

    public void PlayAndPause()
    {
        if (!Paused)
        {
            Paused = true;

            Time.timeScale = 0;

            _pausePanel.SetActive(true);
        }
        else
        {
            Paused = false;

            Time.timeScale = 1;

            _pausePanel.SetActive(false);
        }
    }

    public void ClosePauseBtn()
    {
        Paused = false;
        Time.timeScale = 1;
        _pausePanel.SetActive(false);

        AudioManager.Instance.PlaySfx("ButtonClick");
    }

    public void ControlsBtn()
    {
        _controlsPanel.SetActive(true);
        AudioManager.Instance.PlaySfx("ButtonClick");
    }

    public void CloseControlsBtn()
    {
        _controlsPanel.SetActive(false);
        AudioManager.Instance.PlaySfx("ButtonClick");
    }

    private void InitializeResolutionDropdown()
    {
        _resolutions = Screen.resolutions;

        _resolutionsDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height + " @" + Math.Round(_resolutions[i].refreshRateRatio.value) + "Hz";

            options.Add(option);

            if (Screen.width == _resolutions[i].width && Screen.height == _resolutions[i].height)
            {
                currentResolutionIndex = i;
            }
        }

        _resolutionsDropdown.AddOptions(options);
        _resolutionsDropdown.value = currentResolutionIndex;
        _resolutionsDropdown.RefreshShownValue();
    }

    public void FullScreen(bool enabled)
    {
        Screen.fullScreen = enabled;
    }

    public void ResolutionItem(int resolutionIndex)
    {
        Resolution selectedResolution = _resolutions[resolutionIndex];

        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed, selectedResolution.refreshRateRatio);
    }
    /// <summary>
    /// Quay về Main Menu mà không lưu game.
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1; // Đảm bảo game không bị dừng khi quay lại menu
        SceneManager.LoadScene("MainMenu"); // Đổi "MainMenu" thành tên scene chính xác
    }
}
