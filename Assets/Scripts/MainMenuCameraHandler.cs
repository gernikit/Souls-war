using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraHandler : MonoBehaviour
{
    public float minX = 0f;
    public float maxX = 1f;
    public float speed = 1f;
    bool moveToRight = true;
    Camera cameraMy;
    float width;
    float height;
    [SerializeField]
    float startX = 1;

    void Start()
    {
        cameraMy = gameObject.GetComponent<Camera>();
        gameObject.GetComponent<Transform>().position = new Vector3(startX, transform.position.y, transform.position.z);

        width = cameraMy.pixelWidth;
        height = cameraMy.pixelHeight;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckBorders();
        MoveBackground();
    }

    void MoveBackground()
    {
        if (moveToRight)
        {
            transform.Translate(new Vector2(1, 0) * Time.fixedDeltaTime * speed);
        }
        else
        {
            transform.Translate(new Vector2(-1, 0) * Time.fixedDeltaTime * speed);
        }
    }

    void CheckBorders()
    {
        Vector2 topLeft = cameraMy.ScreenToWorldPoint(new Vector2(0, height));
        Vector2 topRight = cameraMy.ScreenToWorldPoint(new Vector2(width, height));

        if (topRight.x > maxX)
        {
            moveToRight = false;
        }
        else if (topLeft.x < minX)
        {
            moveToRight = true;
        }
    }
}
