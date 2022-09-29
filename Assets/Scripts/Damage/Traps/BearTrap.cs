using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : BoxCollider
{
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
    }

    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[i]))
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
}
