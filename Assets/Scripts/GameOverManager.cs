using TMPro; // Import TextMeshPro để xử lý văn bản UI
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening; // Import DOTween để tạo hiệu ứng
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Import để quản lý chuyển đổi cảnh (scene)

/// <summary>
/// Quản lý màn hình Game Over và xử lý hiệu ứng chuyển cảnh khi người chơi thua.
/// Kế thừa từ Singleton để đảm bảo chỉ có một instance duy nhất.
/// </summary>
public class GameOverManager : Singleton<GameOverManager>
{
    [Header("References")]
    [SerializeField] private List<TMP_Text> _charsList = new(); // Danh sách chữ cái của "Game Over"
    [SerializeField] private TMP_Text _playAgainMessage; // Thông báo "Nhấn phím để chơi lại"
    [SerializeField] private Image _background; // Hình nền tối khi Game Over

    private bool _animationFinished; // Kiểm tra xem animation kết thúc chưa
    private bool _anyKeyPressed; // Kiểm tra xem người chơi đã nhấn phím sau khi Game Over chưa

    /// <summary>
    /// Kiểm tra nếu người chơi đã thua và nhấn bất kỳ phím nào, thì bắt đầu chuyển đổi cảnh.
    /// </summary>
    private void Update()
    {
        // Kiểm tra nếu người chơi đã hết mạng và nhấn bất kỳ phím nào sau khi hiệu ứng Game Over kết thúc
        if (Player.Instance.Lifes < 0 && Input.anyKeyDown && _animationFinished && !_anyKeyPressed)
        {
            _anyKeyPressed = true; // Đánh dấu là đã nhấn phím, tránh gọi lại nhiều lần

            // Làm mờ từng ký tự của chữ "Game Over" trong 3 giây
            foreach (TMP_Text character in _charsList)
            {
                character.DOFade(0, 3f);
            }

            // Làm mờ nhạc nền dần dần
            AudioManager.Instance.FadeMusic(3);

            // Làm mờ dòng chữ "Nhấn phím để chơi lại"
            _playAgainMessage.DOFade(0, 3f);

            // Bật background và làm mờ dần
            _background.gameObject.SetActive(true);
            GameOverManager.Instance.SaveScoreAndReturnToMainMenu();
            _background.DOFade(0, 3f).OnComplete(() => SceneManager.LoadScene("MainMenu")); // Khi hiệu ứng hoàn tất, load lại game

        }
    }

    /// <summary>
    /// Hiệu ứng hiển thị khi Game Over.
    /// </summary>
    public IEnumerator RunAnimation()
    {
        // Làm tối nền dần dần đến 90% độ mờ
        _background.DOFade(0.9f, 2f);

        // Hiển thị từng ký tự của chữ "Game Over" với hiệu ứng di chuyển xuống
        foreach (TMP_Text character in _charsList)
        {
            character.rectTransform.DOAnchorPos(new Vector2(character.rectTransform.anchoredPosition.x, 50), 0.1f);
            yield return new WaitForSeconds(0.2f); // Chờ trước khi di chuyển ký tự tiếp theo
        }

        // Bật thông báo "Nhấn phím để chơi lại"
        _playAgainMessage.enabled = true;

        // Tạo hiệu ứng nhấp nháy cho dòng chữ "Nhấn phím để chơi lại"
        _playAgainMessage.DOFade(0, 1f).SetLoops(-1, LoopType.Yoyo);

        _animationFinished = true; // Đánh dấu rằng hiệu ứng đã kết thúc và có thể nhận input từ người chơi
    }
    public void SaveScoreAndReturnToMainMenu()
    {
        // Lấy điểm số hiện tại từ ScoreManager
        int currentScore = ScoreManager.Instance.GetScore();

        // Lưu điểm vào PlayerPrefs
        PlayerPrefs.SetInt("LastScore", currentScore);

        // Kiểm tra nếu đạt High Score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            Debug.Log("New High Score: " + currentScore);
        }

        // Quay lại Main Menu
        SceneManager.LoadScene("MainMenu");
    }
}
