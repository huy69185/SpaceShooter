using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneBound : MonoBehaviour
{
    
    private float minX, maxX, minY, maxY;

    void Start()
    {
           Vector3 bound = Camera.main.ScreenToViewportPoint (new Vector3 (Screen.width, Screen.height, 0f));

           minX= -bound .x - 1.5f ; maxX= bound.x + 1.5f;
           minY= -bound .y - 3.45f; maxY= bound.y + 3.45f;
    }

    void Update()
    {
        Vector3 temp = transform.position;

        if (temp.x < minX)
        {
            temp.x = minX;
        }
        else if (temp.x > maxX)
        {
            temp.x = maxX;
        }

        if (temp.y < minY)
        {
            temp.y = minY;
        }
        else if (temp.y > maxY)
        {
            temp.y = maxY;
        }

        transform.position = temp;

        }
    }
