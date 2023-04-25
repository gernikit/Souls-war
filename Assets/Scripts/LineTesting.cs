using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTesting : MonoBehaviour
{
    public LineRenderer lr;

    public GameObject pointA;
    public GameObject pointB;

    [Range(0, 1)]
    public float distanceAlongLine;

    // Use this for initialization
    void Start()
    {
        // to make it show up
        lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.startWidth = 1f;
    }

    void Update()
    {
        // work out the point along the line
        Vector3 pointOnLine = pointA.transform.position + ((pointB.transform.position - pointA.transform.position) * distanceAlongLine);

        // work out the sides
        // vector B-C in your diagram
        Vector3 side1 = pointB.transform.position - pointA.transform.position;
        // into/outof the 2d plane
        Vector3 side2 = new Vector3 (0, 0, 1);

        // get the perpendicular direction
        //Vector3 normalDirection = Vector3.Cross(side1, side2).normalized;
        //Vector3 normalDirection = side1.normalized;

        Vector3 normalDirection;

        if (pointB.transform.position.x > pointA.transform.position.x)
            normalDirection = Vector2.Perpendicular(side1);
        else
            normalDirection = Vector2.Perpendicular(-side1);

        normalDirection *= 0.5f;

        // set the line renderer b-d, d-a, a-d, d-c
        lr.SetPosition(0, pointA.transform.position);
        lr.SetPosition(1, pointOnLine);
        lr.SetPosition(2, pointOnLine + normalDirection);
        lr.SetPosition(3, pointOnLine);
        lr.SetPosition(4, pointB.transform.position);
    }
}
