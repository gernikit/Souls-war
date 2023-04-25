using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHealerMob : Mob
{
    protected bool canHeal = false;
    [SerializeField]
    CircleCollider2D auraCollider;//must be set in inspector!!!

    List<Mob> closestAllyTargets;//maybe remove for closestAllyTargetsNoMaxHP
    List<Mob> closestAllyTargetsNoMaxHP;
    protected bool noAllyNearby = false;

    [Header("Healer")]
    public float healingValue = 6f;
    public float distanceToClosestAlly = 5f;//must be less than radiusHealing
    public float radiusHealingAura = 7f;
    //public float healingRange = 10f;
    public int minHealingTargets = 3;
    public int maxHealingTargets = 5;

    void Start()
    {
        Init();
        InitSimpleHealer();
    }

    // Update is called once per frame
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

                if (canHeal)
                {
                    //proxy.position = model.position; // do it because OFFSET POS proxy
                    animator.SetBool("Run", false);
                    MovingSet(false);
                    if (GetNoMaxHPTargets() && !noAllyNearby)
                        ChoseKindOfHealing();
                    else if (noAllyNearby)
                        HealAttack();
                    //OnAttack(); //HEALING
                }
                //else if (agent.speed == 0)
                else if (aiPath.maxSpeed == 0)
                {
                    animator.SetBool("Run", true);
                    MovingSet(true);
                }

                //if (agent.speed > 0 && targetsList != null)
                if (aiPath.maxSpeed > 0 && (allyTargetsList != null || targetsList != null))
                {
                    System.Func<Mob, bool> conditional = mob => mob.typeOfMob != TypeOfMob.Healer;
                    Transform tar = GetCloseatestAllyTarget(conditional);
                    //Debug.Log("pos =" + tar.position + "!name = " + gameObject.transform.parent.name);
                    if (tar != null)
                    {
                        noAllyNearby = false;
                        SetTarget(tar);
                    }
                    else
                    {
                        noAllyNearby = true;
                        SetTarget(GetNeatestTarget());
                    }

                    MoveToTargets();
                }

                if (!dontAttack)
                    CheckingForHeal();

                Cooldown();

                RotatingCreature();
            }
        }
    }

    protected void InitSimpleHealer()
    {
        closestAllyTargets = new List<Mob>();
        closestAllyTargetsNoMaxHP = new List<Mob>();
        //attackDamage = 1;

        if (auraCollider == null)
            Debug.LogError("Healing aura not init!!!");
        else
            auraCollider.radius = radiusHealingAura * 2;
    }

    public new void MoveToTargets()
    {
        /*
         * Moving to targets
         */
        animator.SetBool("Run", true);

        proxy.position = model.position;

        //model.GetComponent<Rigidbody2D>().velocity = agent.velocity;
        model.GetComponent<Rigidbody2D>().velocity = rvoController.velocity;

    }

    protected void CheckingForHeal()
    {
        if (target == null || Vector2.Distance(gameObject.transform.position, target.position) >= distanceToClosestAlly)
            canHeal = false;
        else
            canHeal = true;
    }

    protected bool GetNoMaxHPTargets()
    {
        closestAllyTargetsNoMaxHP.Clear();

        for (int i = 0; i < closestAllyTargets.Count; i++)
        {
            if (!closestAllyTargets[i].IsMaxHP())
                closestAllyTargetsNoMaxHP.Add(closestAllyTargets[i]);
        }
        return closestAllyTargetsNoMaxHP.Count > 0;
    }

    protected void ChoseKindOfHealing()
    {
        if (timeBtwAttack <= 0)
        {
            if (closestAllyTargetsNoMaxHP.Count >= minHealingTargets)
            {
                int i = 0;
                while (i < closestAllyTargetsNoMaxHP.Count && i < maxHealingTargets)//maybe parallel?
                {
                    closestAllyTargetsNoMaxHP[i].TakeHeal(healingValue / closestAllyTargetsNoMaxHP.Count);
                    i++;
                }
                animator.SetTrigger("AuraHealing");
            }
            else if (closestAllyTargetsNoMaxHP.Count >= 1)
            {
                Mob temp = GetMobLowestHP(closestAllyTargets);
                temp.TakeHeal(healingValue);
                animator.SetTrigger("SimpleHealing");
            }

            timeBtwAttack = startTimeBtwAttack;
        }
    }

    protected void HealAttack()
    {
        if (timeBtwAttack <= 0)
        {
            animator.SetTrigger("SimpleHealing");
            target.GetComponent<Mob>().TakeDamage(attackDamage);
            Instantiate(ParticleManager.particlesForMobs[ParticleTypesForMob.BreakingHeart], target.gameObject.transform);//maybe hard for perfomance
            timeBtwAttack = startTimeBtwAttack;
        }
    }

    protected Mob GetMobLowestHP(List<Mob> mobs)
    {
        Mob mob;
        mob = mobs[mobs.Count - 1];
        float lowestHp = mob.maxHealth - mob.health;
        foreach (Mob el in mobs)
        {
            if (lowestHp < el.maxHealth - el.health)
            {
                lowestHp = el.maxHealth - el.health;
                mob = el;  
            }
        }
        return mob;
    }

    void OnTriggerEnter2D(Collider2D collider)//dont work with losestAllyTargetsNoMaxHP because not updating!!!
    {
        if (collider.tag == gameObject.tag &&
            collider.gameObject.layer == 6 &&
            !closestAllyTargets.Contains(collider.gameObject.GetComponent<Mob>()) &&
            collider.gameObject.GetComponent<Mob>().typeOfMob != TypeOfMob.Healer)//no heals another healers
        {
            closestAllyTargets.Add(collider.gameObject.GetComponent<Mob>());
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == gameObject.tag && collider.gameObject.layer == 6)
        {
            if (closestAllyTargets.Contains(collider.gameObject.GetComponent<Mob>()))
                closestAllyTargets.Remove(collider.gameObject.GetComponent<Mob>());

            if (closestAllyTargetsNoMaxHP.Contains(collider.gameObject.GetComponent<Mob>()))
                closestAllyTargetsNoMaxHP.Remove(collider.gameObject.GetComponent<Mob>());
        }
    }
}
