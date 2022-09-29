using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : BoxCollider
{
    private Animator animator;
    [SerializeField] private List<string> tagsToDisarm;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToDisarm.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToDisarm[i]))
            {
                DisarmTrap();
            }
        }

        for (int j = 0; j < tagsToCheck.Count; j++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[j]))
            {
                if (coll.GetComponent<DamageReceiver>().IsAlive())
                    ActivateTrap(coll);
            }
        }
    }

    private void ActivateTrap(Collider2D coll)
    {
        animator.SetTrigger(Config.ANIMATOR_BEAR_TRAP_TRIGGER);

        coll.SendMessage("ReceiveDamage");

        boxCollider.enabled = false;
    }

    private void DisarmTrap()
    {
        boxCollider.enabled = false;
        animator.SetTrigger(Config.ANIMATOR_BEAR_TRAP_TRIGGER);
    }
}
