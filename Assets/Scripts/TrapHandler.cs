using System.Collections.Generic;
using UnityEngine;
public enum TypesOfTrap
{
    Spikes,
    Spears
}
public class TrapHandler : MonoBehaviour
{
    private Collider2D trapCollider;
    private Animator animator;

    [SerializeField]
    private List<Collider2D> closeEnemyColliders;

    [SerializeField]
    private TypesOfTrap typeOfTrap;//must set in inspector!!!
    [SerializeField]
    private float attackDamage = 1f;
    [SerializeField]
    private float startTimeBtwAttack = 10f;
    [SerializeField]
    private float timeBtwAttack = 0f;
    [SerializeField]
    private int targetsCount = 1;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip bitSound;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!Mob.gameIsStop)
        {
            Cooldown();
            if (closeEnemyColliders.Count > 0)
                OnAttack();
        }
    }

    private void Init()
    {
        trapCollider = gameObject.GetComponent<Collider2D>();
        animator = gameObject.GetComponent<Animator>();

        closeEnemyColliders = new List<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Cooldown()
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

    private void OnTriggerEnter2D(Collider2D collider)
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

    private void OnTriggerExit2D(Collider2D collider)
    {       
        if (typeOfTrap == TypesOfTrap.Spikes || typeOfTrap == TypesOfTrap.Spears &&
            collider.tag == "Footing" &&
            collider.transform.parent.gameObject.tag == "Player" || collider.transform.parent.gameObject.tag == "Enemy" &&
            closeEnemyColliders.Contains(collider.transform.parent.gameObject.GetComponent<CapsuleCollider2D>()))
        {
            closeEnemyColliders.Remove(collider.transform.parent.gameObject.GetComponent<CapsuleCollider2D>());
        }
    }
}
