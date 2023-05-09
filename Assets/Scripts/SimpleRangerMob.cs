using UnityEngine;

public class SimpleRangerMob : Mob
{
    [Header("Ranger parameters")]
    [SerializeField]
    protected float shootingRange = 5f;
    protected bool canShoot = false;


    [Header("Ammo parameters")]
    [SerializeField]
    protected Transform placeForShoot;
    [SerializeField]
    protected AmmoType ammoType = AmmoType.None;
    protected Vector3 targetPosition;

    [Header("Ammo bezier parameters")]
    [SerializeField]
    protected float bezierOffset = 4f;
    [SerializeField]
    protected float speedAmmo = 4f;

    private void Start()
    {
        Init();
    }
    
    private void Update()
    {
        if (!gameIsStop)
        {
            BehaviorProcessing();
        }
    }

    protected void BehaviorProcessing()
    {
        if (!IsDead())
        {
            if (targetsList == null || targetsList.Count == 0)
                GetTargets();

            if (canShoot)
            {
                animator.SetBool("Run", false);
                MovingSet(false);
                OnAttack();
            }
            else if (aiPath.maxSpeed == 0)
            {
                animator.SetBool("Run", true);
                MovingSet(true);
            }

            if (aiPath.maxSpeed > 0 && targetsList != null)
            {
                SetTarget(GetNeatestTarget());
                MoveToTargets();
            }

            if (!dontAttack)
                CheckForShooting();

            Cooldown();

            RotatingCreature();
        }
    }
    protected new void MoveToTargets()
    {
        animator.SetBool("Run", true);

        proxy.position = model.position;

        model.GetComponent<Rigidbody2D>().velocity = rvoController.velocity;

    }

    protected void CheckForShooting() 
    {
        if (target == null || Vector2.Distance(gameObject.transform.position, target.position) >= shootingRange)
            canShoot = false;
        else
        {
            canShoot = true;
            targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
        }
    }

    public new void OnAttack()
    {
        if (timeBtwAttack <= 0)
        {
            animator.SetTrigger("Attack");    //spawn bullet!!!
            timeBtwAttack = startTimeBtwAttack;
        }
    }

    //create bullet with velocity
    public void AfterShoot()
    {
        GameObject newBullet = Instantiate(Resources.Load<GameObject>("Ammo/" + ammoType.ToString()), placeForShoot.position, transform.rotation);
        newBullet.GetComponent<AmmoHandler>().SetDamage(attackDamage);
        newBullet.tag = gameObject.tag + "Ammo";
        newBullet.GetComponent<AmmoHandler>().SetAmmoType(ammoType);
        newBullet.GetComponent<AmmoHandler>().SetTarget(targetPosition);
        newBullet.GetComponent<AmmoHandler>().SetOffsetBezier(bezierOffset);
        newBullet.GetComponent<AmmoHandler>().SetSpeed(speedAmmo);
        newBullet.GetComponent<AmmoHandler>().SetMaxDistance(shootingRange);
        newBullet.GetComponent<AmmoHandler>().CalculatePathAndFollow();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //nothing for simple ranger
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //nothing for simple ranger
    }
}
