using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Mover attachedCharacter;

    [SerializeField] private int currentHitPoints = 1;
    [SerializeField] private int maxHitPoints = 1;

    [SerializeField] private float immuneTime = 1f;
    private bool isImmune = false;

    private void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    protected virtual void ReceiveDamage()
    {
        if (!isImmune)
        {
            currentHitPoints--;
            animator.SetTrigger("Hurt");
            StartCoroutine(ImmuneCooldown());

            if (currentHitPoints < 1)
            {
                Death();
            }
        }
    }

    protected virtual void Death()
    {
        animator.SetTrigger("Death");

        attachedCharacter.Death();
    }

    private IEnumerator ImmuneCooldown()
    {
        isImmune = true;

        yield return new WaitForSeconds(immuneTime);

        isImmune = false;
    }
}
