using System.Collections;
using UnityEngine;

public enum AmmoType
{
    None,
    RockAmmo,
    Arrow
}

public class AmmoHandler : MonoBehaviour
{
    private Collider2D ammoCollider;
    private Animator animator;
    private Rigidbody2D rb;
    private AmmoType ammoType;
    
    private Vector3 targetPos;
    private float minY;
    private float maxDistanceBetween;

    private float damage = 0;
    private float speed = 1f;

    private float offsetBezier = 1f;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (targetPos != null && ammoType == AmmoType.None)
        {
            Debug.LogError("Ammo Type = None");
            return;
        }
        CheckForDestroy();

    }

    private void Init()
    {
        ammoCollider = gameObject.GetComponent<PolygonCollider2D>();
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();

        gameObject.layer = 8;//AMMO
        rb.gravityScale = 0;
    }


    public void CalculatePathAndFollow()
    {
        if (targetPos == null)
            Destroy(gameObject);
        StartCoroutine(FollowBezierPath());
    }

    public void SetAmmoType(AmmoType type)
    {
        ammoType = type;
    }
    public void SetOffsetBezier(float offset)
    {
        offsetBezier = offset;
    }

    public void SetSpeed(float speedAmmo)
    {
        speed = speedAmmo;
    }

    public void SetDamage(float dam)
    {
        damage = dam;
    }
    public void SetMaxDistance(float distance)
    {
        maxDistanceBetween = distance;
    }
    public void SetTarget(Vector2 tar)
    {
        minY = gameObject.transform.position.y;
        targetPos = tar;
        if (minY > tar.y)
            minY = tar.y;
    }

    private IEnumerator FollowBezierPath()
    {
        Vector3 objectPosition;

        Vector3 p0 = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        Vector3 p2 = new Vector3(targetPos.x, targetPos.y, targetPos.z);

        Vector3 centerVector = (p2 + p0) / 2;
        Debug.DrawLine(Vector3.zero, centerVector, Color.blue,4f);

        Vector3 targetVector = p2 - p0;

        Vector3 p1 = Vector2.Perpendicular(targetVector);

        //for correct direction
        if (p2.x > p0.x)
            p1 = Vector2.Perpendicular(targetVector);
        else
            p1 = Vector2.Perpendicular(-targetVector);
        Debug.DrawLine(Vector3.zero, p1, Color.red, 4f);

        float fallibilitySize = offsetBezier / p1.magnitude;
        float fallibilityDistance = targetVector.magnitude / maxDistanceBetween;
        p1 *= fallibilitySize * fallibilityDistance;
        p1 += centerVector;

        Debug.DrawLine(Vector3.zero, p1, Color.green, 4f);
        float tParam = 0;

        while (tParam < 1)
        {
            tParam += Time.deltaTime * (speed / Bezier.GetFirstDerivativeQuadraticBezier(p0, p1, p2, tParam).magnitude);

            objectPosition = Bezier.GetPointQuadraticBezier(p0, p1, p2, tParam);

            if (ammoType == AmmoType.Arrow)
                RotateAmmoByMovement(objectPosition - transform.position);

            gameObject.transform.position = objectPosition;

            yield return new WaitForEndOfFrame();
        }
    }

    private void RotateAmmoByMovement(Vector2 relativePos)
    {
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);//???
    }

    private void CheckForDestroy()
    {
        if (Vector2.Distance(gameObject.transform.position, targetPos) < 0.01)
        {
            speed = 0;
            animator.SetTrigger("Destroy");
            targetPos = new Vector2();
        }
    }

    private void DealDamage(Collider2D collider)
    {
        if (speed != 0)
        {
            speed = 0;
            animator.SetTrigger("Destroy");
            collider.gameObject.GetComponent<Mob>().TakeDamage(damage);
        }
    }

    private void AfterDestroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (damage == 0)
            Debug.LogError("Damage == 0");

        switch (gameObject.tag)
        {
            case "EnemyAmmo":
                {
                    if (collider.tag == "Player" && collider is CapsuleCollider2D)
                    {
                        if (!collider.Distance(ammoCollider).isOverlapped)
                            return;

                        DealDamage(collider);
                    }
                    break;
                }
            case "PlayerAmmo":
                {
                    if (collider.tag == "Enemy" && collider is CapsuleCollider2D)
                    {
                        if (!collider.Distance(ammoCollider).isOverlapped)
                            return;

                       DealDamage(collider);
                    }
                    break;
                }
        }
    }
}
