using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlay : MonoBehaviour
{
    public static GamePlay instance;

    private void Awake()
    {
        MakeInstance();
        Time.timeScale = 1f; // Đảm bảo thời gian chạy bình thường khi vào scene
    }

    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Đảm bảo chỉ có một instance của GamePlay
        }
    }

    [SerializeField]
    private GameObject pausePanel, gameOverPanel;

    // Hiển thị bảng tạm dừng
    public void PauseGameButton()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Dừng thời gian
    }

    // Tiếp tục trò chơi
    public void ResumeButton()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Tiếp tục thời gian
    }

    public void MenuButton()
    {
        Time.timeScale = 1f; // Đặt lại thời gian về bình thường trước khi rời scene
        SceneManager.LoadScene("MainMenu"); // Quay lại màn hình chính
    }

    // Chơi lại từ đầu
    public void PlayAgainButton()
    {
        Time.timeScale = 1f; // Đảm bảo game không bị dừng khi chơi lại

        int currentScore = ScoreManager.instance.GetScore(); // Lấy điểm hiện tại
        int highScore = ScoreManager.instance.GetHighScore(); // Lấy điểm cao nhất

        // Kiểm tra và cập nhật điểm cao nhất nếu cần
        if (currentScore > highScore)
        {
            ScoreManager.instance.SaveHighScore(); // Lưu điểm cao nhất
        }

        // Reset điểm hiện tại khi chơi lại
        SceneManager.LoadScene("GamePlay"); // Tải lại scene hiện tại
        ScoreManager.instance.SaveCurrentScore(); // Lưu điểm khi chơi lại
    }

    // Quay về màn hình chính và lưu điểm cao nhất
    public void QuitButton()
    {
        // Lưu điểm cao nhất khi thoát game
        int currentScore = ScoreManager.instance.GetScore(); // Lấy điểm hiện tại
        int highScore = ScoreManager.instance.GetHighScore(); // Lấy điểm cao nhất

        // Nếu điểm hiện tại cao hơn điểm cao nhất, cập nhật điểm cao nhất
        if (currentScore > highScore)
        {
            ScoreManager.instance.SaveHighScore(); // Lưu điểm cao nhất
        }

        Time.timeScale = 1f; // Đặt lại thời gian về bình thường trước khi rời scene
        SceneManager.LoadScene("MainMenu"); // Quay lại màn hình chính
    }

    // Hiển thị màn hình kết thúc khi máy bay chết
    public void PlaneDiedShow()
    {
        gameOverPanel.SetActive(true); // Hiển thị bảng Game Over
        ScoreManager.instance.UpdateGameOverScore(); // Cập nhật điểm trên game over panel
    }
}
