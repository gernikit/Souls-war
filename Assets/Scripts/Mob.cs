using System.Collections.Generic;
using UnityEngine;
using Pathfinding.RVO;
using Pathfinding;

public static class MobTargets
{
    //needed to find targets
    public static List<GameObject> targetsListPlayer; //with null!!!!
    public static List<GameObject> targetsListEnemy;  //with null!!!!

    //needed to check win conditionals
    public static List<GameObject> mobsPlayer;
    public static List<GameObject> mobsEnemy;

    //needed to check healers (for fight)
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
            if (!countMobsOfTypeEnemy.ContainsKey(el.GetComponent<Mob>().typeOfMob))
                countMobsOfTypeEnemy[el.GetComponent<Mob>().typeOfMob] = 1;
            else
                countMobsOfTypeEnemy[el.GetComponent<Mob>().typeOfMob]++;
        }

        countMobsOfTypePlayer = new Dictionary<TypeOfMob, int>();
        foreach (GameObject el in mobsPlayer)
        {
            if (!countMobsOfTypePlayer.ContainsKey(el.GetComponent<Mob>().typeOfMob))
                countMobsOfTypePlayer[el.GetComponent<Mob>().typeOfMob] = 1;
            else
                countMobsOfTypePlayer[el.GetComponent<Mob>().typeOfMob]++;
        }
    }

    /*
    static public int GetNotNullMobsCount(bool IsPlayerList)
    {
        int count = 0;
        if (IsPlayerList == true)
        {
            for (int i = 0; i < targetsListEnemy.Count; i++)
            {
                if (targetsListEnemy[i] != null)
                    count++;
            }
        }
        else
        {
            for (int i = 0; i < targetsListPlayer.Count; i++)
            {
                if (targetsListPlayer[i] != null)
                    count++;
            }
        }
        return count;
    }
    */
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]  //for body collider
[RequireComponent(typeof(CircleCollider2D))]  //for attackRange

public class Mob : MonoBehaviour
{
    //script is child in hierarchy!!!
    /*
    public enum AnimatorEvent
    {
        Bool,
        Trigger,
        Int,
        Float
    }
    */
    static public bool gameIsStop = true;

    protected List<GameObject> targetsList;
    protected List<GameObject> allyTargetsList;

    protected RVOController rvoController;
    protected AIDestinationSetter aiDestinationSetter;
    protected AIPath aiPath;
    protected DynamicGridObstacle dynamicGridObstacle;
    ///protected NavMeshAgent agent;
    ///protected NavMeshObstacle agentObstavle;
    ///
    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip bitSound;

    [Header("Mob parameters")]
    public float health = 10;
    [HideInInspector]
    public float maxHealth = 0f;
    public float attackDamage = 1;
    public float firstSpeed = 1f;
    //public float aiPath.maxSpeed;
    public float startTimeBtwAttack = 4f;
    protected float timeBtwAttack = 0f;
    //public float attackRange = 0.5f;
    public bool watchRight = true;
    [Range(0f, 1f)]
    public float allDamageResistance = 0f;
    public TypeOfMob typeOfMob;

    [Header("Mob special options")]
    public bool dontAttack = false;

    [Header("Parent-child property")]
    public GameObject headObject;
    public Transform target;
    public Transform model;
    public Transform proxy;

    [Header("Model")]
    Rigidbody2D rb;
    protected CapsuleCollider2D capsuleCollider;            
    protected Animator animator;

    public static void OnStart()
    {
        gameIsStop = !gameIsStop;
    }

    public void Init()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        capsuleCollider = gameObject.GetComponent<CapsuleCollider2D>();

        //animator = gameObject.GetComponent<Animator>();
        rvoController = proxy.GetComponent<RVOController>();
        aiDestinationSetter = proxy.GetComponent<AIDestinationSetter>();
        aiPath = proxy.GetComponent<AIPath>();
        dynamicGridObstacle = proxy.GetComponent<DynamicGridObstacle>();

        //agent = proxy.GetComponent<NavMeshAgent>();
        //agentObstavle = proxy.GetComponent<NavMeshObstacle>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        animator = gameObject.GetComponent<Animator>();
        capsuleCollider.isTrigger = true;
        //rb.gravityScale = 0;
        //rb.freezeRotation = true;
        ///agentObstavle.enabled = false;
        ///agent.updateRotation = false;
        ///agent.updateUpAxis = false;
        ///agent.speed = firstSpeed;

        if (gameObject.tag == "Enemy")
            SetOutline(Resources.Load<Material>("Outline/SpritesheetMaterial_Outline"));

        aiPath.maxSpeed = firstSpeed;

        headObject = gameObject.transform.parent.gameObject;

        maxHealth = health;
    }

    public void SetOutline(Material material)
    {
         gameObject.GetComponent<SpriteRenderer>().material = material;
    }
    void Start()
    {
        Init();
        //GetTargets();
        gameObject.layer = 6; /*Mobs*/

    }

    void Update()
    {
        
    }

    protected void GetTargets()
    {
        if (gameObject.tag == "Enemy")
        {
            targetsList = MobTargets.targetsListEnemy;//new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
            allyTargetsList = MobTargets.targetsListPlayer;
        }
        else if (gameObject.tag == "Player")
        {
            targetsList = MobTargets.targetsListPlayer;//new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            allyTargetsList = MobTargets.targetsListEnemy;
        }
    }

    public void SetTarget(Transform nearest)
    {
        //Transform nearest = GetNeatestTarget();
        target = nearest;

        aiDestinationSetter.target = target.transform;

        //agent.SetDestination(target.position);
        ///agent.destination = target.position;
    }

    public void MoveToTargets()
    {
        /*
         * Moving to targets
         */
    }

    /*
    protected void EventForAnimator(AnimatorEvent mod, string nameValue, bool valueBool = false, int valueInt = 0, float valueFloat = 0f)
    {
        switch (mod)
        {
            case (AnimatorEvent.Bool):
                {
                    animator.SetBool(nameValue, valueBool);
                    break;
                }
            case (AnimatorEvent.Trigger):
                {
                    animator.SetTrigger(nameValue);
                    break;
                }
            case (AnimatorEvent.Int):
                {
                    animator.SetInteger(nameValue, valueInt);
                    break;
                }
            case (AnimatorEvent.Float):
                {
                    animator.SetFloat(nameValue, valueFloat);
                    break;
                }
        }
    }
    */

    public void RotatingCreature()
    {
        ///if (agent.velocity.x < -0.1)
        if (rvoController.velocity.x < -0.1)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 180, transform.rotation.z, transform.rotation.w);
            watchRight = false;
        }
        ///if (agent.velocity.x > 0.1)
        if (rvoController.velocity.x > 0.1)
        {
            transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, transform.rotation.w);
            watchRight = true;
        }

        ///if (agent.velocity.x < 0.5)
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

    public void Cooldown()
    {
        if (timeBtwAttack > 0)
            timeBtwAttack -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        damage *= (1 - allDamageResistance);
        health -= damage;
        if (IsDead() && !animator.GetBool("IsDeath"))
        {
            animator.SetTrigger("Death");
            animator.SetBool("IsDeath", true);
        }
    }

    public void TakeHeal(float heal)
    {
        float lostHP = maxHealth - health;
        if (lostHP < heal)
            health += lostHP;
        else
            health += heal;
        Instantiate(ParticleManager.particlesForMobs[ParticleTypesForMob.HealDone], gameObject.transform);//maybe hard for perfomance
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

            for (int j = 0; j < targetsList.Count; j++)
            {
                if (targetsList[j] != null)
                {
                    if (Vector2.Distance(targetsList[j].transform.position, gameObject.transform.position) <
                       Vector2.Distance(nearest.transform.position, gameObject.transform.position))
                    {
                        nearest = targetsList[j];
                    }
                }
            }
            /*
            foreach (GameObject el in targetsList)
            {
                if (Vector2.Distance(el.transform.position, gameObject.transform.position) <
                    Vector2.Distance(nearest.transform.position, gameObject.transform.position))
                {
                    nearest = el;
                }
            }*/

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

        for (int i = 0; i < WinConditionalHandler.targetsDefence.Count; i++)
        {
            if (Vector2.Distance(WinConditionalHandler.targetsDefence[i].transform.position, gameObject.transform.position) <
                Vector2.Distance(nearest.transform.position, gameObject.transform.position))
            {
                nearest = WinConditionalHandler.targetsDefence[i];
            }
        }

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
            } while (nearest == null && i < allyTargetsList.Count);//?

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

            for (int j = 0; j < allyTargetsList.Count; j++)
            {
                if (allyTargetsList[j] != null && allyTargetsList[j] != this.gameObject)
                {
                    if (Vector2.Distance(allyTargetsList[j].transform.position, gameObject.transform.position) <
                       Vector2.Distance(nearest.transform.position, gameObject.transform.position)&&
                       condition(allyTargetsList[j].GetComponent<Mob>()))
                    {
                        nearest = allyTargetsList[j];
                    }
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
            MobTargets.countMobsOfTypePlayer[typeOfMob]--;
        }
        else if (gameObject.tag == "Enemy")
        {
            MobTargets.mobsEnemy.Remove(gameObject);
            MobTargets.countMobsOfTypeEnemy[typeOfMob]--;
        }
        Destroy(headObject);
    }

    public void MovingSet(bool active)
    {
        if (!active)
        {
            ///agent.enabled = false;
            ///agentObstavle.enabled = true;
            ///agent.speed = 0;
            aiPath.canSearch = false;
            rvoController.velocity = new Vector3(0, 0);
            aiPath.maxSpeed = 0;
            rb.velocity = new Vector2(0, 0);

            dynamicGridObstacle.enabled = true;
            dynamicGridObstacle.gameObject.layer = 7;

            //rb.freezeRotation = true;
            //rb.velocity = new Vector2(0, 0);

            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            ///agentObstavle.enabled = false;
            ///agent.enabled = true;
            ///agent.speed = firstSpeed;
            aiPath.canSearch = true;
            aiPath.maxSpeed = firstSpeed;

            dynamicGridObstacle.enabled = false;
            dynamicGridObstacle.gameObject.layer = 0;

            //rb.freezeRotation = false;

            //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public bool IsDead()
    {
        return health <= 0 ? true : false;
    }

    public bool IsMaxHP()
    {
        return maxHealth == health;
    }

}
