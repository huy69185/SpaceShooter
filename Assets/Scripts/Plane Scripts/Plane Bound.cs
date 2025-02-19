using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBound : MonoBehaviour
{
    // Biến lưu trữ giới hạn di chuyển của máy bay trong màn hình
    private float minX, maxX, minY, maxY;

    void Start()
    {
        // Lấy kích thước màn hình và chuyển đổi sang tọa độ Viewport
        Vector3 bound = Camera.main.ScreenToViewportPoint(new Vector3(Screen.width, Screen.height, 0f));

        // Xác định giới hạn di chuyển của máy bay dựa vào kích thước màn hình
        minX = -bound.x - 1.5f; // Giới hạn trái
        maxX = bound.x + 1.5f;  // Giới hạn phải
        minY = -bound.y - 3.45f; // Giới hạn dưới
        maxY = bound.y + 3.45f;  // Giới hạn trên
    }

    void Update()
    {
        // Lấy vị trí hiện tại của máy bay
        Vector3 temp = transform.position;

        // Kiểm tra và giữ máy bay trong giới hạn theo trục X
        if (temp.x < minX)
        {
            temp.x = minX;
        }
        else if (temp.x > maxX)
        {
            temp.x = maxX;
        }

        // Kiểm tra và giữ máy bay trong giới hạn theo trục Y
        if (temp.y < minY)
        {
            temp.y = minY;
        }
        else if (temp.y > maxY)
        {
            temp.y = maxY;
        }

        // Cập nhật lại vị trí của máy bay với giới hạn đã thiết lập
        transform.position = temp;
    }
}
