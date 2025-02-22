using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text _highScoreText; // Hiển thị High Score
    [SerializeField] private TMP_Text _lastScoreText; // Hiển thị điểm của lần chơi trước
    [SerializeField] private GameObject _guidePanel; // Panel hướng dẫn
    [SerializeField] private GameObject _quitConfirmPanel; // Panel xác nhận thoát

    private void Start()
    {
        // Lấy High Score từ PlayerPrefs
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        _highScoreText.text = "" + highScore.ToString();

        // Lấy điểm số của lần chơi trước
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        _lastScoreText.text = "Last Score: " + lastScore.ToString();
    }

    /// <summary>
    /// Chuyển sang Game Scene khi ấn Play.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("Game"); 
    }

    /// <summary>
    /// Hiển thị hoặc ẩn bảng hướng dẫn (Guide).
    /// </summary>
    public void ToggleGuide()
    {
        _guidePanel.SetActive(!_guidePanel.activeSelf);
    }

    /// <summary>
    /// Hiển thị bảng xác nhận thoát game.
    /// </summary>
    public void ShowQuitConfirm()
    {
        _quitConfirmPanel.SetActive(true);
    }

    /// <summary>
    /// Ẩn bảng xác nhận thoát game.
    /// </summary>
    public void CancelQuit()
    {
        _quitConfirmPanel.SetActive(false);
    }

    /// <summary>
    /// Thoát game khi nhấn Confirm trong bảng xác nhận.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quited!"); 
    }

    /// <summary>
    /// Reset High Score 
    /// </summary>
    public void ResetHighScore()
    {
        PlayerPrefs.SetInt("HighScore", 0);
        _highScoreText.text = "0";
        Debug.Log("High Score reseted!");
    }

}
