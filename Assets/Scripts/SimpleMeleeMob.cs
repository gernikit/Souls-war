using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMeleeMob : Mob
{

    [Header("Melee parameters")]
    public int targetsCount;
    List<Collider2D> closeEnemyColliders;
    CircleCollider2D circleCollider;

    void InitSimpleMelee()
    {
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
        closeEnemyColliders = new List<Collider2D>();
        circleCollider.isTrigger = true;
        //circleCollider.radius = attackRange;
    }
    void Start()
    {
        Init();
        InitSimpleMelee();
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
                Cooldown();
                if (closeEnemyColliders.Count > 0)
                {
                    //proxy.position = model.position; // do it because OFFSET POS proxy
                    OnAttack();
                }

                //if (agent.speed > 0 && targetsList != null)
                if (aiPath.maxSpeed > 0 && targetsList != null)
                {
                    if (WinConditionalHandler.winningConditional == WinConditionalHandler.WinConditional.Defence && tag != "Player" && WinConditionalHandler.onlyMovingToTarget)
                        SetTarget(GetCloseatestDefenceTarget());
                    else
                        SetTarget(GetNeatestTarget());
                    MoveToTargets();
                }
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

        //model.GetComponent<Rigidbody2D>().velocity = agent.velocity;
        model.GetComponent<Rigidbody2D>().velocity = rvoController.velocity;

    }

    public void AfterAttack()
    {
        for (int i = 0; i < targetsCount && i < closeEnemyColliders.Count; i++)
        {
            //audioSource.clip = bitSound;
            //audioSource.Play();
            if (closeEnemyColliders.Count > 0 && closeEnemyColliders[i] != null && !closeEnemyColliders[i].GetComponent<Mob>().IsDead())
                closeEnemyColliders[i].GetComponent<Mob>().TakeDamage(attackDamage);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (dontAttack)
            return;
        /*
        if (collider.gameObject.layer == 6) //Mobs
            MovingSet(false);
        */
        switch (gameObject.tag)
        {
            case "Enemy":
                {
                    if (collider.tag == "Player" && collider is CapsuleCollider2D)
                    {
                        List<Collider2D> collider2Ds = new List<Collider2D>();
                        circleCollider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
                        if (!collider.Distance(circleCollider).isOverlapped)//if (!collider2Ds.Contains(collider))
                            return;
                        collider2Ds.Clear();

                        MovingSet(false);
                        animator.SetBool("Run", false);
                        closeEnemyColliders.Add(collider);
                    }
                    break;
                }
            case "Player":
                {
                    if (collider.tag == "Enemy" && collider is CapsuleCollider2D)
                    {
                        //collider.Distance ???
                        List<Collider2D> collider2Ds = new List<Collider2D>();
                        circleCollider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
                        if (!collider.Distance(circleCollider).isOverlapped) //if (!collider2Ds.Contains(collider))
                            return;
                        collider2Ds.Clear();

                        MovingSet(false);
                        animator.SetBool("Run", false);
                        closeEnemyColliders.Add(collider);
                    }
                    break;
                }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.gameObject.layer == 6) /*Mobs*/
        {
            switch (gameObject.tag)
            {
                case "Enemy":
                    {
                        if (collider.tag == "Player")
                        {
                            //if (collider.GetComponent<Mob>().IsDead())
                            //targetsList.Remove(collider.gameObject);
                            closeEnemyColliders.Remove(collider);
                            if (closeEnemyColliders.Count == 0)
                                MovingSet(true);
                        }
                        break;
                    }
                case "Player":
                    {
                        if (collider.tag == "Enemy")
                        {
                            //if (collider.GetComponent<Mob>().IsDead())
                            //targetsList.Remove(collider.gameObject);
                            closeEnemyColliders.Remove(collider);
                            if (closeEnemyColliders.Count == 0)
                                MovingSet(true);
                        }
                        break;
                    }
            }
        }
    }
}
