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
    private float width;
    private float height;
    

    private void Start()
    {
        cameraMy = gameObject.GetComponent<Camera>();
        gameObject.GetComponent<Transform>().position = new Vector3(startX, transform.position.y, transform.position.z);

        width = cameraMy.pixelWidth;
        height = cameraMy.pixelHeight;
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
