using DG.Tweening; // Import DOTween để tạo hiệu ứng
using TMPro; // Import TextMeshPro để hiển thị text UI
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quản lý màn hình bắt đầu game, chờ người chơi nhấn phím để vào game.
/// Kế thừa từ Singleton để đảm bảo chỉ có một instance duy nhất.
/// </summary>
public class StartManager : Singleton<StartManager>
{
    [SerializeField] private TMP_Text _startText; // Dòng chữ "Nhấn phím bất kỳ để bắt đầu"
    [SerializeField] private Image _startPanel; // Hình nền che toàn màn hình khi chưa bắt đầu

    [HideInInspector] public bool GameStarted; // Trạng thái game đã bắt đầu hay chưa

    private bool _anyKeyPressed; // Kiểm tra xem người chơi đã nhấn phím chưa

    /// <summary>
    /// Khởi tạo hiệu ứng nhấp nháy cho dòng chữ "Nhấn phím để bắt đầu".
    /// </summary>
    private void Start()
    {
        // Làm mờ chữ theo hiệu ứng nhấp nháy liên tục (fade in-out)
        _startText.DOFade(0, 2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Kiểm tra nếu người chơi nhấn phím bất kỳ, bắt đầu vào game.
    /// </summary>
    private void Update()
    {
        if (Input.anyKeyDown && !GameStarted && !_anyKeyPressed)
        {
            _anyKeyPressed = true; // Đánh dấu đã nhấn phím để tránh gọi lại nhiều lần

            AudioManager.Instance.PlaySfx("GameStarted"); // Phát âm thanh bắt đầu game
            _startText.DOKill(); // Dừng hiệu ứng nhấp nháy của chữ
            _startText.DOFade(0, 0.1f).SetLoops(9, LoopType.Yoyo).OnComplete(() => // Hiệu ứng nhấp nháy nhanh trước khi biến mất
            {
                _startPanel.DOFade(0, 1f).OnComplete(() => // Làm mờ dần màn hình bắt đầu trong 1 giây
                {
                    GameStarted = true; // Đánh dấu game đã bắt đầu
                    AudioManager.Instance.PlayMusic(); // Bắt đầu phát nhạc nền
                    _startPanel.gameObject.SetActive(false); // Ẩn panel nền sau khi hoàn tất hiệu ứng
                });
            });
        }
    }
}
