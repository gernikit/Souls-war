using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
public enum TypesOfTrap
{
    Spikes,
    Spears
}
public class TrapHandler : MonoBehaviour
{
    Collider2D trapCollider;
    Animator animator;

    [SerializeField]
    List<Collider2D> closeEnemyColliders;

    public TypesOfTrap typeOfTrap;//must set in inspector!!!
    public float attackDamage = 1f;
    public float startTimeBtwAttack = 10f;
    protected float timeBtwAttack = 0f;
    public int targetsCount = 1;

    protected AudioSource audioSource;
    [SerializeField]
    protected AudioClip bitSound;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mob.gameIsStop)
        {
        }
        else
        {
            Cooldown();
            if (closeEnemyColliders.Count > 0)
                OnAttack();
        }
    }

    protected void Init()
    {
        trapCollider = gameObject.GetComponent<Collider2D>();
        animator = gameObject.GetComponent<Animator>();

        closeEnemyColliders = new List<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void Cooldown()
    {
        if (timeBtwAttack > 0)
            timeBtwAttack -= Time.deltaTime;
    }
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

    public void AfterAttack()
    {
        for (int i = 0; i < targetsCount && i < closeEnemyColliders.Count; i++)
        {
            if (closeEnemyColliders.Count > 0 && closeEnemyColliders[i] != null && !closeEnemyColliders[i].GetComponent<Mob>().IsDead())
                closeEnemyColliders[i].GetComponent<Mob>().TakeDamage(attackDamage);
            else if (closeEnemyColliders[i] == null) //not optimizated! because not deleted in ExitTrigger//maybe create cleaning function
                closeEnemyColliders.RemoveAt(i);
        }
    }

    public void DestroyTrap()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (typeOfTrap == TypesOfTrap.Spikes || typeOfTrap == TypesOfTrap.Spears)
        {
            if (collider.tag == "Footing")
            {
                if (collider.transform.parent.gameObject.tag == "Player" || collider.transform.parent.gameObject.tag == "Enemy")
                {
                    closeEnemyColliders.Add(collider.transform.parent.gameObject.GetComponent<CapsuleCollider2D>());
                }
                else
                {
                    Debug.LogError("Shadow without model!!!");
                }
            }
        }
        else if ((collider.tag == "Player" || collider.tag == "Enemy") && collider is CapsuleCollider2D && collider.gameObject.layer == 6)
        { 
            if (!collider.Distance(trapCollider).isOverlapped)
                return;

            closeEnemyColliders.Add(collider);
        }                 
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        /*
        if ((collider.tag == "Player" || collider.tag == "Enemy") && collider is CapsuleCollider2D && collider.gameObject.layer == 6)
            if (closeEnemyColliders.Contains(collider))
                closeEnemyColliders.Remove(collider);
        */
        
        if (typeOfTrap == TypesOfTrap.Spikes || typeOfTrap == TypesOfTrap.Spears)
        {
            if (collider.tag == "Footing")
            {
                if (collider.transform.parent.gameObject.tag == "Player" || collider.transform.parent.gameObject.tag == "Enemy")
                {
                    if (closeEnemyColliders.Contains(collider.transform.parent.gameObject.GetComponent<CapsuleCollider2D>()))
                        closeEnemyColliders.Remove(collider.transform.parent.gameObject.GetComponent<CapsuleCollider2D>());
                }
            }
        }
        
    }
}
