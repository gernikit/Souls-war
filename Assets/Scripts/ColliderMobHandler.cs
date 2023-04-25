using UnityEngine;

public class ColliderMobHandler : MonoBehaviour
{

    private CapsuleCollider2D capsuleCollider;
    private CircleCollider2D circleCollider;
    private Mob parentHandler; //must be mob!!!
    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        parentHandler = gameObject.transform.parent.GetComponent<Mob>();
        circleCollider.isTrigger = true;
        capsuleCollider.isTrigger = true;
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        //parentHandler.TriggerEnter2D(collider, circleCollider);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        //parentHandler.TriggerExit2D(collider);
    }

}
