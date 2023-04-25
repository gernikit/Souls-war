using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRangerMob : Mob
{
    [Header("Ranger parameters")]
    public float shootingRange = 5f;
    protected bool canShoot = false;


    [Header("Ammo parameters")]
    public Transform placeForShoot;
    //public string bulletPath;
    public AmmoType ammoType = AmmoType.None;
    Vector3 targetPosition;

    [Header("Ammo bezier parameters")]
    public float bezierOffset = 4f;
    public float speedAmmo = 4f;
    //TypePath .... ???

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    
    void Update()
    {
        if (gameIsStop)
        {
        }
        else
        {
            if (!IsDead())
            {
                if (targetsList == null || targetsList.Count == 0)
                    GetTargets();

                if (canShoot)
                {
                    //proxy.position = model.position; // do it because OFFSET POS proxy
                    animator.SetBool("Run", false);
                    MovingSet(false);
                    OnAttack();
                }
                //else if (agent.speed == 0)
                else if (aiPath.maxSpeed == 0)
                {
                    animator.SetBool("Run", true);
                    MovingSet(true);
                }

                //if (agent.speed > 0 && targetsList != null)
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
    }

    public new void MoveToTargets()
    {
        /*
         * Moving to targets
         */
        animator.SetBool("Run", true);

        proxy.position = model.position;

        ///model.GetComponent<Rigidbody2D>().velocity = agent.velocity;
        model.GetComponent<Rigidbody2D>().velocity = rvoController.velocity;

    }

    void CheckForShooting()
    {
        if (target == null || Vector2.Distance(gameObject.transform.position, target.position) >= shootingRange)
            canShoot = false;
        else
        {
            canShoot = true;
            targetPosition = new Vector3(target.position.x, target.position.y, target.position.z);
        }
    }

    new void OnAttack()
    {
        if (timeBtwAttack <= 0)
        {
            animator.SetTrigger("Attack");    //spawn bullet!!!
            timeBtwAttack = startTimeBtwAttack;
        }
    }

    //create bullet with velocity
    void AfterShoot()
    {
        //audioSource.clip = bitSound;
        //audioSource.Play();
        //GameObject newBullet = Instantiate(Resources.Load<GameObject>(bulletPath), placeForShoot.position, Quaternion.identity);
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

    void OnTriggerEnter2D(Collider2D collider)
    {
        //nothing for simple ranger
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        //nothing for simple ranger
    }
}
