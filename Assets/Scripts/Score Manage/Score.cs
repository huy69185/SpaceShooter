using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TMP_Text scoreText;  // Hiển thị điểm trên HUD
    public TMP_Text gameOverScoreText;  // Hiển thị điểm khi game over (trên game over panel)
    public TMP_Text highScoreText;  // Hiển thị điểm cao nhất trên HUD
    private int score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        score = 0;  // Reset điểm khi bắt đầu trò chơi
        UpdateScoreText(); // Hiển thị điểm ban đầu
        UpdateHighScoreText();  // Hiển thị điểm cao nhất khi game bắt đầu
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScoreText();
        SaveCurrentScore();  // Lưu điểm khi có sự thay đổi
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateGameOverScore()
    {
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + score.ToString();
        }
    }

    public int GetScore()
    {
        return score;
    }

    // Lưu điểm cao nhất vào PlayerPrefs
    public void SaveHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("", 0);
        Debug.Log($"Current High Score: {currentHighScore}");

        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt("", score);
            PlayerPrefs.Save();
            Debug.Log($"{score}");
        }
    }

    // Lấy điểm cao nhất
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("", 0); // Lấy điểm cao nhất
    }

    // Lưu điểm vào PlayerPrefs
    public void SaveCurrentScore()
    {
        PlayerPrefs.SetInt("CurrentScore", score);
        PlayerPrefs.Save();
    }

    // Gọi hàm này khi trò chơi kết thúc để lưu điểm cao nhất và điểm hiện tại
    public void OnGameOver()
    {
        SaveHighScore(); // Lưu điểm cao nhất khi game over
        SaveCurrentScore(); // Lưu điểm hiện tại khi game over
    }

    // Hiển thị điểm cao nhất khi bắt đầu trò chơi
    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            int highScore = GetHighScore();
            highScoreText.text = highScore.ToString();
        }
    }
}
