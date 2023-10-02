using System.Collections.Generic;
using UnityEngine;
using Pathfinding.RVO;
using Pathfinding;

public static class MobTargets
{
    //need to find targets
    public static List<GameObject> targetsListPlayer; //can contain nulls!!!!
    public static List<GameObject> targetsListEnemy;  //can contain nulls!!!!

    //need to check win conditionals
    public static List<GameObject> mobsPlayer;
    public static List<GameObject> mobsEnemy;

    //need to check healers targets(for fight)
    public static Dictionary<TypeOfMob, int> countMobsOfTypePlayer;
    public static Dictionary<TypeOfMob, int> countMobsOfTypeEnemy;

    static public void OnStartGame()
    {
        targetsListPlayer = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        targetsListEnemy = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        mobsEnemy = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        mobsPlayer = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        countMobsOfTypeEnemy = new Dictionary<TypeOfMob, int>();
        foreach (GameObject el in mobsEnemy)
        {
            if (!countMobsOfTypeEnemy.ContainsKey(el.GetComponent<Mob>().TypeOfMob))
                countMobsOfTypeEnemy[el.GetComponent<Mob>().TypeOfMob] = 1;
            else
                countMobsOfTypeEnemy[el.GetComponent<Mob>().TypeOfMob]++;
        }

        countMobsOfTypePlayer = new Dictionary<TypeOfMob, int>();
        foreach (GameObject el in mobsPlayer)
        {
            if (!countMobsOfTypePlayer.ContainsKey(el.GetComponent<Mob>().TypeOfMob))
                countMobsOfTypePlayer[el.GetComponent<Mob>().TypeOfMob] = 1;
            else
                countMobsOfTypePlayer[el.GetComponent<Mob>().TypeOfMob]++;
        }
    }
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]  //for body collider
[RequireComponent(typeof(CircleCollider2D))]  //for attackRange

public class Mob : MonoBehaviour
{
    static public bool gameIsStop = true;

    protected List<GameObject> targetsList;
    protected List<GameObject> allyTargetsList;

    protected RVOController rvoController;
    protected AIDestinationSetter aiDestinationSetter;
    protected AIPath aiPath;
    protected DynamicGridObstacle dynamicGridObstacle;

    [SerializeField]
    protected AudioClip bitSound;
    protected AudioSource audioSource;

    [Header("Mob parameters")]
    [SerializeField]
    protected float health = 10;
    protected float maxHealth = 0f;
    [SerializeField]
    protected float attackDamage = 1;
    [SerializeField]
    protected float firstSpeed = 1f;
    [SerializeField]
    protected float startTimeBtwAttack = 4f;
    protected float timeBtwAttack = 0f;
    [SerializeField]
    protected bool watchRight = true;
    [SerializeField]
    [Range(0f, 1f)]
    protected float allDamageResistance = 0f;
    [SerializeField]
    protected TypeOfMob typeOfMob;

    [Header("Mob special options")]
    [SerializeField]
    protected bool dontAttack = false;

    [Header("Parent-child property")]
    [SerializeField]
    protected GameObject headObject;// must be set in inspector
    [SerializeField]
    protected Transform target;
    [SerializeField]
    protected Transform model;// must be set in inspector
    [SerializeField]
    protected Transform proxy;// must be set in inspector

    [Header("Model")]
    protected Rigidbody2D rb;
    protected CapsuleCollider2D capsuleCollider;            
    protected Animator animator;
    protected void Start()
    {
        Init();
        gameObject.layer = 6; /*Mobs*/
    }

    public static void OnStart()
    {
        gameIsStop = !gameIsStop;
    }

    public void Init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();

        rvoController = proxy.GetComponent<RVOController>();
        aiDestinationSetter = proxy.GetComponent<AIDestinationSetter>();
        aiPath = proxy.GetComponent<AIPath>();
        dynamicGridObstacle = proxy.GetComponent<DynamicGridObstacle>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        animator = gameObject.GetComponent<Animator>();
        capsuleCollider.isTrigger = true;

        if (gameObject.tag == "Enemy")
            SetOutline(Resources.Load<Material>("Outline/SpritesheetMaterial_Outline"));

        aiPath.maxSpeed = firstSpeed;

        headObject = gameObject.transform.parent.gameObject;

        MaxHealth = Health;
    }

    public void SetOutline(Material material)
    {
         gameObject.GetComponent<SpriteRenderer>().material = material;
    }

    public void TakeDamage(float damage)
    {
        damage *= (1 - allDamageResistance);
        Health -= damage;
        if (IsDead() && !animator.GetBool("IsDeath"))
        {
            animator.SetTrigger("Death");
            animator.SetBool("IsDeath", true);
        }
    }

    public void TakeHeal(float heal)
    {
        float lostHP = MaxHealth - Health;
        if (lostHP < heal)
            Health += lostHP;
        else
            Health += heal;
        Instantiate(ParticleManager.particlesForMobs[ParticleTypesForMob.HealDone], gameObject.transform);//maybe hard for perfomance
    }

    public void MovingSet(bool active)
    {
        if (!active)
        {
            aiPath.canSearch = false;
            rvoController.velocity = new Vector3(0, 0);
            aiPath.maxSpeed = 0;
            rb.velocity = new Vector2(0, 0);

            dynamicGridObstacle.enabled = true;
            dynamicGridObstacle.gameObject.layer = 7;
        }
        else
        {
            aiPath.canSearch = true;
            aiPath.maxSpeed = firstSpeed;

            dynamicGridObstacle.enabled = false;
            dynamicGridObstacle.gameObject.layer = 0;
        }
    }
    protected void GetTargets()
    {
        if (gameObject.tag == "Enemy")
        {
            targetsList = MobTargets.targetsListEnemy;
            allyTargetsList = MobTargets.targetsListPlayer;
        }
        else if (gameObject.tag == "Player")
        {
            targetsList = MobTargets.targetsListPlayer;
            allyTargetsList = MobTargets.targetsListEnemy;
        }
    }

    protected void SetTarget(Transform nearest)
    {
        target = nearest;
        aiDestinationSetter.target = target.transform;
    }

    protected void MoveToTargets()
    {
        /*
         * Moving to targets
         */
    }

    protected void RotatingCreature()
    {
        if (rvoController.velocity.x < -0.1)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
            watchRight = false;
        }
        else if (rvoController.velocity.x > 0.1)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
            watchRight = true;
        }

        if (rvoController.velocity.x < 0.5)
        {
            if (target != null)
            {
                Vector3 lol = transform.TransformPoint(transform.position);
                if (target.position.x - gameObject.transform.position.x > 0)
                    transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
                else if (target.position.x - gameObject.transform.position.x < 0)
                    transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
            }
        }
    }

    protected void Cooldown()
    {
        if (timeBtwAttack > 0)
            timeBtwAttack -= Time.deltaTime;
    }
    protected Transform GetNeatestTarget()
    {
        if (targetsList.Count > 0)
        {
            int i = 0;
            GameObject nearest;
            do
            {
                nearest = targetsList[i];
                i++;
            } while (nearest == null && i < targetsList.Count);

            if (nearest == null)
                return null;

            Vector2 myPosition = gameObject.transform.position;
            float nearestSqrDistance = ((Vector2)nearest.transform.position - myPosition).sqrMagnitude;

            for (int j = 0; j < targetsList.Count; j++)
            {
                if (targetsList[j] != null)
                {
                    float sqrDistance = ((Vector2)targetsList[j].transform.position - myPosition).sqrMagnitude;

                    if (sqrDistance < nearestSqrDistance)
                    {
                        nearest = targetsList[j];
                        nearestSqrDistance = sqrDistance;
                    }
                    
                    /*
                    if (Vector2.Distance(targetsList[j].transform.position, gameObject.transform.position) <
                       Vector2.Distance(nearest.transform.position, gameObject.transform.position))
                    {
                        nearest = targetsList[j];
                    }
                    */
                }
            }
            return nearest.transform;
        }
        else
            return null;
    }

    protected Transform GetCloseatestDefenceTarget()
    {
        GameObject nearest;
        if (WinConditionalHandler.targetsDefence == null || WinConditionalHandler.targetsDefence.Count == 0)
            return null;

        int j = 0;

        do
        {
            nearest = WinConditionalHandler.targetsDefence[j];
            j++;
        } while (nearest == null && j < WinConditionalHandler.targetsDefence.Count);

        if (nearest == null)
            return null;
        
        Vector2 myPosition = gameObject.transform.position;
        float nearestSqrDistance = ((Vector2)nearest.transform.position - myPosition).sqrMagnitude;

        for (int i = 0; i < WinConditionalHandler.targetsDefence.Count; i++)
        {
            float sqrDistance = ((Vector2)WinConditionalHandler.targetsDefence[i].transform.position - myPosition).sqrMagnitude;

            if (sqrDistance < nearestSqrDistance)
            {
                nearest = WinConditionalHandler.targetsDefence[i];
                nearestSqrDistance = sqrDistance;
            }
        }
        
        // for (int i = 0; i < WinConditionalHandler.targetsDefence.Count; i++)
        // {
        //     if (Vector2.Distance(WinConditionalHandler.targetsDefence[i].transform.position, gameObject.transform.position) <
        //         Vector2.Distance(nearest.transform.position, gameObject.transform.position))
        //     {
        //         nearest = WinConditionalHandler.targetsDefence[i];
        //     }
        // }

        return nearest.transform;
    }

    protected Transform GetCloseatestAllyTarget()
    {
        if (allyTargetsList.Count > 0)
        {
            int i = 0;
            GameObject nearest = null;
            GameObject temp = null;
            do
            {
                temp = allyTargetsList[i];
                if (temp != gameObject)
                    nearest = allyTargetsList[i];
                i++;
            } while (nearest == null && i < allyTargetsList.Count);//need more tests for that condition

            if (nearest == null)
                return null;

            for (int j = 0; j < allyTargetsList.Count; j++)
            {
                if (allyTargetsList[j] != null && allyTargetsList[j] != this.gameObject)
                {
                    if (Vector2.Distance(allyTargetsList[j].transform.position, gameObject.transform.position) <
                       Vector2.Distance(nearest.transform.position, gameObject.transform.position))
                    {
                        nearest = allyTargetsList[j];
                    }
                }
            }

            if (nearest == gameObject)
                return null;

            return nearest.transform;
        }
        else
            return null;
    }

    protected Transform GetCloseatestAllyTarget(System.Func<Mob, bool> condition)
    {
        if (allyTargetsList.Count > 0)
        {
            int i = 0;
            GameObject nearest = null;
            GameObject temp = null;
            do
            {
                temp = allyTargetsList[i];
                if (temp != gameObject && temp != null && condition(temp.GetComponent<Mob>()))
                    nearest = allyTargetsList[i];
                i++;
            } while (nearest == null && i < allyTargetsList.Count);//?

            if (nearest == null)
                return null;
            
            Vector2 myPosition = gameObject.transform.position;
            float nearestSqrDistance = ((Vector2)nearest.transform.position - myPosition).sqrMagnitude;

            for (int j = 0; j < allyTargetsList.Count; j++)
            {
                if (allyTargetsList[j] != null && allyTargetsList[j] != this.gameObject)
                {
                    float sqrDistance = ((Vector2)allyTargetsList[j].transform.position - myPosition).sqrMagnitude;

                    if (sqrDistance < nearestSqrDistance && condition(allyTargetsList[j].GetComponent<Mob>()))
                    {
                        nearest = allyTargetsList[j];
                        nearestSqrDistance = sqrDistance;
                    }
                    
                    /*
                    if (Vector2.Distance(allyTargetsList[j].transform.position, gameObject.transform.position) <
                       Vector2.Distance(nearest.transform.position, gameObject.transform.position)&&
                       condition(allyTargetsList[j].GetComponent<Mob>()))
                    {
                        nearest = allyTargetsList[j];
                    }
                    */
                }
            }

            if (condition(nearest.GetComponent<Mob>()) && nearest != gameObject)
                return nearest.transform;
            else
                return null;

        }
        else
            return null;
    }

    protected Transform GetCloseatestTarget(System.Func<Mob, bool> condition)
    {
        if (condition == null)
            return null;

        if (targetsList.Count > 0)
        {
            int i = 0;
            GameObject nearest;
            do
            {
                nearest = targetsList[i];
                i++;
            } while (nearest == null && i < targetsList.Count);

            if (nearest == null)
                return null;

            for (int j = 0; j < targetsList.Count; j++)
            {
                if (targetsList[j] != null)
                {
                    if (Vector2.Distance(targetsList[j].transform.position, gameObject.transform.position) <
                       Vector2.Distance(nearest.transform.position, gameObject.transform.position) &&
                       condition(targetsList[j].GetComponent<Mob>()))
                    {
                        nearest = targetsList[j];
                    }
                }
            }

            if (condition(nearest.GetComponent<Mob>()))
                return nearest.transform;
            else
                return null;
        }
        else
            return null;
    }
    
    public TypeOfMob TypeOfMob { get => typeOfMob; set => typeOfMob = value; }
    public float Health { get => health; set => health = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public bool IsDead() { return Health <= 0 ? true : false; }
    public bool IsMaxHP() { return MaxHealth == Health; }
    //for attack
    public void OnAttack()
    {
        if (timeBtwAttack <= 0)
        {
            animator.SetTrigger("Attack");
            timeBtwAttack = startTimeBtwAttack;
        }
    }

    public void OnBitSound()
    {
        audioSource.clip = bitSound;
        audioSource.Play();
    }

    public void AfterDeath()
    {
        if (gameObject.tag == "Player")
        {
            MobTargets.mobsPlayer.Remove(gameObject);
            MobTargets.countMobsOfTypePlayer[TypeOfMob]--;
        }
        else if (gameObject.tag == "Enemy")
        {
            MobTargets.mobsEnemy.Remove(gameObject);
            MobTargets.countMobsOfTypeEnemy[TypeOfMob]--;
        }
        Destroy(headObject);
    }
}
