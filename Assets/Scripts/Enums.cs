/// <summary>
/// Xác định hướng di chuyển của vật thể trong game.
/// </summary>
public enum DirType
{
    Up,    // Di chuyển lên
    Down,  // Di chuyển xuống
    Left,  // Di chuyển sang trái
    Right  // Di chuyển sang phải
}

/// <summary>
/// Xác định vị trí của vật thể so với màn hình hoặc bản đồ.
/// </summary>
public enum SideType
{
    Top,    // Ở phía trên
    Bot,    // Ở phía dưới
    Left,   // Ở bên trái
    Right,  // Ở bên phải
    Null    // Không có vị trí cụ thể
}

/// <summary>
/// Xác định loại hiệu ứng âm thanh (SFX) trong game.
/// </summary>
public enum SfxType
{
    PlayerShoot  // Âm thanh khi người chơi bắn
}
