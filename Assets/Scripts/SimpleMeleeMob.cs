using System.Collections.Generic;
using UnityEngine;

public class SimpleMeleeMob : Mob
{

    [Header("Melee parameters")]
    [SerializeField]
    protected int targetsCount;
    protected List<Collider2D> closeEnemyColliders;
    protected CircleCollider2D circleCollider;

    private void Start()
    {
        Init();
        InitSimpleMelee();
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
            Cooldown();
            if (closeEnemyColliders.Count > 0)
                OnAttack();

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

    protected void InitSimpleMelee()
    {
        circleCollider = gameObject.GetComponent<CircleCollider2D>();
        closeEnemyColliders = new List<Collider2D>();
        circleCollider.isTrigger = true;
    }
    protected new void MoveToTargets()
    {
        animator.SetBool("Run", true);

        proxy.position = model.position;

        rb.velocity = rvoController.velocity;
    }

    public void AfterAttack()
    {
        for (int i = 0; i < targetsCount && i < closeEnemyColliders.Count; i++)
        {
            if (closeEnemyColliders.Count > 0 && closeEnemyColliders[i] != null && !closeEnemyColliders[i].GetComponent<Mob>().IsDead())
                closeEnemyColliders[i].GetComponent<Mob>().TakeDamage(attackDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (dontAttack)
            return;

        switch (gameObject.tag)
        {
            case "Enemy":
                {
                    if (collider.tag == "Player" && collider is CapsuleCollider2D)
                    {
                        List<Collider2D> collider2Ds = new List<Collider2D>();
                        circleCollider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
                        if (!collider.Distance(circleCollider).isOverlapped)
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
                        List<Collider2D> collider2Ds = new List<Collider2D>();
                        circleCollider.OverlapCollider(new ContactFilter2D().NoFilter(), collider2Ds);
                        if (!collider.Distance(circleCollider).isOverlapped)
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

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.gameObject.layer == 6) /*Mobs*/
        {
            switch (gameObject.tag)
            {
                case "Enemy":
                    {
                        if (collider.tag == "Player")
                        {
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
