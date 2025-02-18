using UnityEngine;

public class BGScaler : MonoBehaviour
{
    public float borderOffset = 0.2f; // Điều chỉnh độ rộng của border, có thể tăng giảm

    void Start()
    {
        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Screen.width / Screen.height;

        worldWidth -= borderOffset;  // Trừ đi khoảng viền để có border ở hai bên

        transform.localScale = new Vector3(worldWidth, worldHeight, 1f);
    }
}
