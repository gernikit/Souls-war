using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraHandler : MonoBehaviour
{
    public float maxZoom = 5.5f;
    public float minZoom = 5.5f;
    public float minXPos = 0;//leftBorde
    public float maxXPos = 0;//rightBorder
    public float minYPos = 0;
    public float maxYPos = 0;
    public float sensitivityZoom = 10f;
    public float sensitivityMove = 100f;

    Vector3 touchStart;
    private float targetPosX;
    private float targetPosY;

    Camera mainCamera;

    public void Start()
    {
        mainCamera = GetComponent<Camera>();
        targetPosX = transform.position.x;
        targetPosY = transform.position.y;

        float width = mainCamera.pixelWidth;
        float height = mainCamera.pixelHeight;

        minXPos -= mainCamera.ScreenToWorldPoint(new Vector2(0, 0)).x - mainCamera.ScreenToWorldPoint(new Vector2(width / 2, height / 2)).x;//center
        maxXPos -= mainCamera.ScreenToWorldPoint(new Vector2(width, height)).x - mainCamera.ScreenToWorldPoint(new Vector2(width / 2, height / 2)).x;//center
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.touchCount == 2 && maxZoom != minZoom) //for zoom
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            ZoomCamera(difference * 0.01f);
        }
        else if (Input.GetMouseButton(0)) //for moving camera
        {
            float positionX = mainCamera.ScreenToWorldPoint(Input.mousePosition).x - touchStart.x;
            float positionY = mainCamera.ScreenToWorldPoint(Input.mousePosition).y - touchStart.y;

            targetPosX = transform.position.x - positionX;
            targetPosY = transform.position.y - positionY;

            targetPosX = Mathf.Clamp(transform.position.x - positionX, minXPos, maxXPos);
            targetPosY = Mathf.Clamp(transform.position.y - positionY, minYPos, maxYPos);

            transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosX, sensitivityMove * Time.deltaTime),
                         transform.position.y,//Mathf.Lerp(transform.position.y, targetPosY, sensitivityMove * Time.deltaTime),
                         transform.position.z);
        }



    }
    void ZoomCamera(float increment)
    {
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(GetComponent<Camera>().orthographicSize - increment * sensitivityZoom, minZoom, maxZoom);
    }
}
