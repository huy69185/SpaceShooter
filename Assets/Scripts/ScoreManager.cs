using TMPro; // Import TextMeshPro để hiển thị điểm số trên UI
using UnityEngine;

/// <summary>
/// Quản lý điểm số trong game.
/// Kế thừa từ Singleton để đảm bảo chỉ có một instance duy nhất tồn tại.
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    [Header("References")]
    [SerializeField] private TMP_Text _scoreText; // UI Text để hiển thị điểm số

    private int _score; // Biến lưu trữ điểm số hiện tại

    /// <summary>
    /// Tăng điểm số và cập nhật giao diện UI.
    /// </summary>
    /// <param name="points">Số điểm cần thêm vào.</param>
    public void AddScore(int points)
    {
        _score += points; // Cộng thêm điểm số mới
        _scoreText.text = _score.ToString(); // Cập nhật UI hiển thị điểm số
    }
        /// <summary>
    /// Lấy điểm số hiện tại.
    /// </summary>
    public int GetScore()
    {
        return _score; // Trả về điểm hiện tại
    }
}
