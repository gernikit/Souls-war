using System;
using UnityEngine;

public class MainMenuCameraHandler : MonoBehaviour
{
    [SerializeField]
    private float startX = 1;
    [SerializeField]
    private float minX = -13.5f;
    [SerializeField]
    private float maxX = 38.8f;
    [SerializeField]
    private float speed = 1f;

    private bool moveToRight = true;
    private Camera cameraMy;
    

    private void Start()
    {
        cameraMy = gameObject.GetComponent<Camera>();
        gameObject.GetComponent<Transform>().position = new Vector3(startX, transform.position.y, transform.position.z);
    }
    
    private void FixedUpdate()
    {
        CheckBorders();
        MoveBackground();
    }

    private void MoveBackground()
    {
        transform.Translate(new Vector2(moveToRight ? 1 : -1, 0) * Time.fixedDeltaTime * speed);
    }

    private void CheckBorders()
    {
        Vector2 topLeft = cameraMy.ScreenToWorldPoint(new Vector2(0, cameraMy.pixelHeight));
        Vector2 topRight = cameraMy.ScreenToWorldPoint(new Vector2(cameraMy.pixelWidth, cameraMy.pixelHeight));

        if (topRight.x > maxX)
        {
            moveToRight = false;
            ClampPosition((topRight - topLeft).x);
        }
        else if (topLeft.x < minX)
        {
            moveToRight = true;
            ClampPosition((topRight - topLeft).x);
        }
    }

    private void ClampPosition(float worldCameraWidth)
    {
        var position = transform.position;
        float clampedX = Mathf.Clamp(position.x, minX + worldCameraWidth / 2, maxX - worldCameraWidth / 2);
        Vector3 clampedPosition = new Vector3(clampedX, position.y, position.z);
        position = clampedPosition;
        transform.position = position;
    }
}
