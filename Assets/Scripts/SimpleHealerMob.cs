using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHealerMob : Mob
{
    protected bool canHeal = false;

    [SerializeField] protected CircleCollider2D auraCollider;//must be set in inspector!!!
    [SerializeField] protected HealingAura healingAura;

    [SerializeField] protected ParticleSystem healDone;
    [SerializeField] protected ParticleSystem breakingHeart;
    
    protected List<Mob> closestAllyTargets;//maybe remove for closestAllyTargetsNoMaxHP
    protected List<Mob> closestAllyTargetsNoMaxHP;
    protected bool noAllyNearby = false;

    [Header("Healer")]
    [SerializeField]
    protected float healingValue = 6f;
    [SerializeField]
    protected float distanceToClosestAlly = 5f;//must be less than radiusHealing
    [SerializeField]
    protected float radiusHealingAura = 7f;
    [SerializeField]
    protected int minHealingTargets = 3;
    [SerializeField]
    protected int maxHealingTargets = 5;

    private void OnEnable()
    {
        healingAura.AuraTriggerEnter += OnAuraTriggerEnter;
        healingAura.AuraTriggerExit += OnAuraTriggerExit;
    }

    private void OnDisable()
    {
        healingAura.AuraTriggerEnter -= OnAuraTriggerEnter;
        healingAura.AuraTriggerExit -= OnAuraTriggerExit;
    }

    private void Start()
    {
        Init();
        InitSimpleHealer();
        auraCollider.gameObject.SetActive(true);
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

            if (canHeal)
            {
                animator.SetBool("Run", false);
                MovingSet(false);
                if (GetNoMaxHPTargets() && !noAllyNearby)
                    ChoseKindOfHealing();
                else if (noAllyNearby)
                    HealAttack();
            }
            else if (aiPath.maxSpeed == 0)
            {
                animator.SetBool("Run", true);
                MovingSet(true);
            }

            if (aiPath.maxSpeed > 0 && (allyTargetsList != null || targetsList != null))
            {
                System.Func<Mob, bool> conditional = mob => mob.TypeOfMob != TypeOfMob.Healer;
                Transform tar = GetCloseatestAllyTarget(conditional);
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

    protected void InitSimpleHealer()
    {
        closestAllyTargets = new List<Mob>();
        closestAllyTargetsNoMaxHP = new List<Mob>();

        if (auraCollider == null)
            Debug.LogError("Healing aura not init!!!");
        else
            auraCollider.radius = radiusHealingAura * 2;
    }

    public new void MoveToTargets()
    {
        animator.SetBool("Run", true);

        proxy.position = model.position;

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
            //Instantiate(ParticleManager.particlesForMobs[ParticleTypesForMob.BreakingHeart], target.gameObject.transform);//maybe hard for perfomance
            breakingHeart.gameObject.transform.position = target.gameObject.transform.position;
            breakingHeart.gameObject.SetActive(true);
            timeBtwAttack = startTimeBtwAttack;
        }
    }

    protected Mob GetMobLowestHP(List<Mob> mobs)
    {
        Mob mob;
        mob = mobs[mobs.Count - 1];
        float lowestHp = mob.MaxHealth - mob.Health;
        foreach (Mob el in mobs)
        {
            if (lowestHp < el.MaxHealth - el.Health)
            {
                lowestHp = el.MaxHealth - el.Health;
                mob = el;  
            }
        }
        return mob;
    }

    private void OnAuraTriggerEnter(Collider2D collider)
    {
        if  (collider.gameObject.layer != 6)
            return;
        
        if (collider.tag == gameObject.tag &&
            !closestAllyTargets.Contains(collider.gameObject.GetComponent<Mob>()) &&
            collider.gameObject.GetComponent<Mob>().TypeOfMob != TypeOfMob.Healer)//no heals another healers
        {
            closestAllyTargets.Add(collider.gameObject.GetComponent<Mob>());
        }
    }
    
    private void OnAuraTriggerExit(Collider2D collider)
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
