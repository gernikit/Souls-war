using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraHandler : MonoBehaviour
{
    [SerializeField]
    private float minXPos = 0;//left Border
    [SerializeField]
    private float maxXPos = 0;//right Border
    [SerializeField]
    private float minYPos = 0;
    [SerializeField]
    private float maxYPos = 0;
    [SerializeField]
    private float sensitivityZoom = 10f;
    [SerializeField]
    private float sensitivityMove = 100f;

    Vector3 touchStart;
    private float targetPosX;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        targetPosX = transform.position.x;

        float width = mainCamera.pixelWidth;
        float height = mainCamera.pixelHeight;

        minXPos -= mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).x - mainCamera.ScreenToWorldPoint(new Vector2(width / 2, height / 2)).x;//center
        maxXPos -= mainCamera.ScreenToWorldPoint(new Vector2(width, height)).x - mainCamera.ScreenToWorldPoint(new Vector2(width / 2, height / 2)).x;//center
    }
    private void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0)) //for moving camera
        {
            float positionX = mainCamera.ScreenToWorldPoint(Input.mousePosition).x - touchStart.x;

            targetPosX = transform.position.x - positionX;

            targetPosX = Mathf.Clamp(transform.position.x - positionX, minXPos, maxXPos);

            transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosX, sensitivityMove * Time.deltaTime),
                         transform.position.y,
                         transform.position.z);
        }
    }
}
