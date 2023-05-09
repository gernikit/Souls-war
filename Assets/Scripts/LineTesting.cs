using UnityEngine;

public class LineTesting : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lr;

    [SerializeField]
    private GameObject pointA;
    [SerializeField]
    private GameObject pointB;

    [SerializeField]
    [Range(0, 1)]
    private float distanceAlongLine;

    // Use this for initialization
    private void Start()
    {
        // to make it show up
        lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = 5;
        lr.startWidth = 1f;
    }

    private void Update()
    {
        // work out the point along the line
        Vector3 pointOnLine = pointA.transform.position + ((pointB.transform.position - pointA.transform.position) * distanceAlongLine);

        // work out the sides
        // vector B-C in your diagram
        Vector3 side1 = pointB.transform.position - pointA.transform.position;

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
