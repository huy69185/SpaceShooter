using DG.Tweening; // Import DOTween để thực hiện hiệu ứng âm thanh
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import UI để điều khiển các thanh trượt (Slider)

/// <summary>
/// Lớp SourceAndVolume dùng để lưu trữ AudioSource và giá trị âm lượng ban đầu.
/// </summary>
[Serializable]
public class SourceAndVolume
{
    public AudioSource AudioSource; // Thành phần phát âm thanh
    [HideInInspector] public float StartVolume; // Lưu trữ âm lượng ban đầu
}

/// <summary>
/// Quản lý hệ thống âm thanh trong game, bao gồm âm thanh nền (Music) và hiệu ứng âm thanh (SFX).
/// Kế thừa từ Singleton để đảm bảo chỉ có một AudioManager duy nhất tồn tại.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private Slider _masterSlider; // Thanh trượt điều chỉnh âm lượng tổng
    [SerializeField] private Slider _musicSlider; // Thanh trượt điều chỉnh âm lượng nhạc nền
    [SerializeField] private Slider _sfxSlider; // Thanh trượt điều chỉnh âm lượng hiệu ứng âm thanh (SFX)
    
    [SerializeField] private List<SourceAndVolume> _sfxList = new(); // Danh sách các hiệu ứng âm thanh đơn lẻ
    [SerializeField] private List<SourceAndVolume> _sfxLoopList = new(); // Danh sách các hiệu ứng âm thanh lặp lại
    [SerializeField] private SourceAndVolume _music; // Nhạc nền
    
    [SerializeField] private RectTransform _soundRedLine; // Hiệu ứng hình ảnh khi bật/tắt âm thanh

    private float _masterVolume = 1; // Giá trị âm lượng tổng
    private float _musicVolume = 1; // Giá trị âm lượng nhạc nền
    private float _sfxVolume = 1; // Giá trị âm lượng hiệu ứng âm thanh
    private bool _muted; // Kiểm tra xem có đang tắt âm hay không

    /// <summary>
    /// Hàm Start() khởi tạo âm lượng và giá trị thanh trượt.
    /// </summary>
    private void Start()
    {
        SetStartVolume(); // Lưu lại giá trị âm lượng ban đầu
        SetSlidersStartVolume(); // Cập nhật giá trị ban đầu của các thanh trượt
    }

    /// <summary>
    /// Lưu giá trị thanh trượt khi bắt đầu game.
    /// </summary>
    private void SetSlidersStartVolume()
    {
        _masterVolume = _masterSlider.value;
        _musicVolume = _musicSlider.value;
        _sfxVolume = _sfxSlider.value;

        ChangeMusicVolume();
        ChangeSfxVolume();
    }

    /// <summary>
    /// Lưu trữ âm lượng ban đầu của tất cả các AudioSource.
    /// </summary>
    private void SetStartVolume()
    {
        foreach (SourceAndVolume sfx in _sfxList)
        {
            sfx.StartVolume = sfx.AudioSource.volume;
        }

        foreach (SourceAndVolume sfxLoop in _sfxLoopList)
        {
            sfxLoop.StartVolume = sfxLoop.AudioSource.volume;
        }

        _music.StartVolume = _music.AudioSource.volume;
    }

    /// <summary>
    /// Phát hiệu ứng âm thanh theo tên.
    /// </summary>
    public void PlaySfx(string sfxName)
    {
        SourceAndVolume audioFinded = _sfxList.Find(audio => audio.AudioSource.name == sfxName);
        audioFinded.AudioSource.Play();
    }

    /// <summary>
    /// Bật hoặc tắt hiệu ứng âm thanh lặp lại.
    /// </summary>
    public void LoopSfx(string sfxName, bool play)
    {
        SourceAndVolume audioFinded = _sfxLoopList.Find(audio => audio.AudioSource.name == sfxName);

        if (play && !audioFinded.AudioSource.isPlaying)
            audioFinded.AudioSource.Play();

        if (!play && audioFinded.AudioSource.isPlaying)
            audioFinded.AudioSource.Stop();
    }

    /// <summary>
    /// Điều chỉnh âm lượng tổng bằng thanh trượt.
    /// </summary>
    public void MasterVolumeSlider(float volume)
    {
        _masterVolume = volume;
        ChangeMusicVolume();
        ChangeSfxVolume();
    }

    /// <summary>
    /// Điều chỉnh âm lượng nhạc nền bằng thanh trượt.
    /// </summary>
    public void MusicVolumeSlider(float volume)
    {
        _musicVolume = volume;
        ChangeMusicVolume();
    }

    /// <summary>
    /// Điều chỉnh âm lượng hiệu ứng âm thanh bằng thanh trượt.
    /// </summary>
    public void SfxVolumeSlider(float volume)
    {
        _sfxVolume = volume;
        ChangeSfxVolume();
    }

    /// <summary>
    /// Thay đổi âm lượng nhạc nền theo giá trị hiện tại.
    /// </summary>
    private void ChangeMusicVolume()
    {
        _music.AudioSource.volume = _music.StartVolume * _masterVolume * _musicVolume;
    }

    /// <summary>
    /// Thay đổi âm lượng tất cả hiệu ứng âm thanh theo giá trị hiện tại.
    /// </summary>
    private void ChangeSfxVolume()
    {
        foreach (SourceAndVolume audio in _sfxList)
        {
            audio.AudioSource.volume = audio.StartVolume * _masterVolume * _sfxVolume;
        }

        foreach (SourceAndVolume audioLoop in _sfxLoopList)
        {
            audioLoop.AudioSource.volume = audioLoop.StartVolume * _masterVolume * _sfxVolume;
        }
    }

    /// <summary>
    /// Bật hoặc tắt âm thanh toàn bộ.
    /// </summary>
    public void MuteBtn()
    {
        if (!_muted)
        {
            _muted = true;
            _music.AudioSource.volume = 0;

            foreach (SourceAndVolume audio in _sfxList)
            {
                audio.AudioSource.volume = 0;
            }

            foreach (SourceAndVolume audioLoop in _sfxLoopList)
            {
                audioLoop.AudioSource.volume = 0;
            }

            StartCoroutine(RedLineAnimation(true)); // Tạo hiệu ứng gạch đỏ khi tắt âm
        }
        else
        {
            _muted = false;
            AudioManager.Instance.PlaySfx("ButtonClick");
            ChangeMusicVolume();
            ChangeSfxVolume();

            StartCoroutine(RedLineAnimation(false)); // Xóa hiệu ứng gạch đỏ khi bật âm
        }
    }

    /// <summary>
    /// Hiệu ứng vạch đỏ khi bật/tắt âm thanh.
    /// </summary>
    private IEnumerator RedLineAnimation(bool grow)
    {
        for (int i = 0; i < 9; i++)
        {
            _soundRedLine.localScale = new Vector3(_soundRedLine.localScale.x, grow ? _soundRedLine.localScale.y + 0.1f : _soundRedLine.localScale.y - 0.1f, _soundRedLine.localScale.z);
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    /// <summary>
    /// Phát nhạc nền.
    /// </summary>
    public void PlayMusic()
    {
        _music.AudioSource.Play();
    }

    /// <summary>
    /// Làm mờ nhạc nền trong một khoảng thời gian (dùng DOTween).
    /// </summary>
    public void FadeMusic(float time)
    {
        _music.AudioSource.DOFade(0, time);
    }

    /// <summary>
    /// Điều chỉnh tốc độ phát (pitch) của hiệu ứng âm thanh lặp lại.
    /// </summary>
    public void SetPitchLoopSfx(string sfxName, float pitch)
    {
        SourceAndVolume audioFinded = _sfxLoopList.Find(audio => audio.AudioSource.name == sfxName);
        audioFinded.AudioSource.pitch = pitch;
    }
}
