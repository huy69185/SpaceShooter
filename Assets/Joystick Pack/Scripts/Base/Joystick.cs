using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; // Import để xử lý các sự kiện chạm (touch)

/// <summary>
/// Class này xử lý joystick ảo để điều khiển nhân vật trong game.
/// Kế thừa từ IPointerDownHandler, IDragHandler, IPointerUpHandler để nhận input từ người dùng.
/// </summary>
public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // Trả về giá trị trục X của joystick (nếu có snap, sẽ làm tròn giá trị)
    public float Horizontal { get { return (snapX) ? SnapFloat(input.x, AxisOptions.Horizontal) : input.x; } }
    
    // Trả về giá trị trục Y của joystick (nếu có snap, sẽ làm tròn giá trị)
    public float Vertical { get { return (snapY) ? SnapFloat(input.y, AxisOptions.Vertical) : input.y; } }
    
    // Trả về vector hướng dựa trên trục X và Y
    public Vector2 Direction { get { return new Vector2(Horizontal, Vertical); } }

    // Phạm vi di chuyển tối đa của joystick
    public float HandleRange
    {
        get { return handleRange; }
        set { handleRange = Mathf.Abs(value); } // Đảm bảo giá trị luôn dương
    }

    // Vùng "chết" (dead zone), nếu joystick di chuyển trong vùng này thì không có tác dụng
    public float DeadZone
    {
        get { return deadZone; }
        set { deadZone = Mathf.Abs(value); } // Đảm bảo giá trị luôn dương
    }

    // Lựa chọn trục hoạt động của joystick (X, Y hoặc cả hai)
    public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }

    // Có bật tính năng snap cho trục X không?
    public bool SnapX { get { return snapX; } set { snapX = value; } }

    // Có bật tính năng snap cho trục Y không?
    public bool SnapY { get { return snapY; } set { snapY = value; } }

    // Các biến cấu hình joystick
    [SerializeField] private float handleRange = 1; // Độ nhạy của joystick
    [SerializeField] private float deadZone = 0; // Độ nhạy trong vùng chết
    [SerializeField] private AxisOptions axisOptions = AxisOptions.Both; // Mặc định joystick hoạt động trên cả hai trục
    [SerializeField] private bool snapX = false; // Mặc định không snap X
    [SerializeField] private bool snapY = false; // Mặc định không snap Y

    // Các thành phần UI của joystick
    [SerializeField] protected RectTransform background = null; // Vùng nền joystick
    [SerializeField] private RectTransform handle = null; // Nút joystick
    private RectTransform baseRect = null; // Gốc của joystick trong UI

    private Canvas canvas; // Lưu trữ canvas của joystick
    private Camera cam; // Camera dùng để tính toán tọa độ

    private Vector2 input = Vector2.zero; // Lưu trữ dữ liệu đầu vào joystick

    /// <summary>
    /// Hàm khởi tạo, chạy khi game bắt đầu.
    /// Thiết lập joystick và kiểm tra lỗi nếu joystick không nằm trong Canvas.
    /// </summary>
    protected virtual void Start()
    {
        HandleRange = handleRange;
        DeadZone = deadZone;
        baseRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        
        if (canvas == null)
            Debug.LogError("Joystick không được đặt trong Canvas!");

        // Căn chỉnh joystick về trung tâm
        Vector2 center = new Vector2(0.5f, 0.5f);
        background.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Khi người chơi nhấn vào joystick, tự động gọi OnDrag để di chuyển joystick.
    /// </summary>
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    /// <summary>
    /// Khi người chơi kéo joystick, tính toán vị trí và điều chỉnh hướng di chuyển.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;

        Vector2 position = RectTransformUtility.WorldToScreenPoint(cam, background.position);
        Vector2 radius = background.sizeDelta / 2;
        input = (eventData.position - position) / (radius * canvas.scaleFactor);

        FormatInput();
        HandleInput(input.magnitude, input.normalized, radius, cam);
        handle.anchoredPosition = input * radius * handleRange;
    }

    /// <summary>
    /// Xử lý đầu vào joystick, kiểm tra xem có vượt qua dead zone không.
    /// </summary>
    protected virtual void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (magnitude > deadZone)
        {
            if (magnitude > 1)
                input = normalised;
        }
        else
            input = Vector2.zero;
    }

    /// <summary>
    /// Giới hạn joystick chỉ di chuyển theo một trục nếu cài đặt.
    /// </summary>
    private void FormatInput()
    {
        if (axisOptions == AxisOptions.Horizontal)
            input = new Vector2(input.x, 0f);
        else if (axisOptions == AxisOptions.Vertical)
            input = new Vector2(0f, input.y);
    }

    /// <summary>
    /// Xử lý chức năng snap của joystick, đảm bảo hướng di chuyển chính xác.
    /// </summary>
    private float SnapFloat(float value, AxisOptions snapAxis)
    {
        if (value == 0)
            return value;

        if (axisOptions == AxisOptions.Both)
        {
            float angle = Vector2.Angle(input, Vector2.up);
            if (snapAxis == AxisOptions.Horizontal)
            {
                if (angle < 22.5f || angle > 157.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            else if (snapAxis == AxisOptions.Vertical)
            {
                if (angle > 67.5f && angle < 112.5f)
                    return 0;
                else
                    return (value > 0) ? 1 : -1;
            }
            return value;
        }
        else
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
        }
        return 0;
    }

    /// <summary>
    /// Khi người chơi thả joystick, đặt lại vị trí về trung tâm.
    /// </summary>
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Chuyển đổi tọa độ màn hình thành tọa độ UI (RectTransform).
    /// </summary>
    protected Vector2 ScreenPointToAnchoredPosition(Vector2 screenPosition)
    {
        Vector2 localPoint = Vector2.zero;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(baseRect, screenPosition, cam, out localPoint))
        {
            Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
            return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
        }
        return Vector2.zero;
    }
}

/// <summary>
/// Enum để xác định joystick hoạt động theo cả hai trục, chỉ X hoặc chỉ Y.
/// </summary>
public enum AxisOptions { Both, Horizontal, Vertical }
