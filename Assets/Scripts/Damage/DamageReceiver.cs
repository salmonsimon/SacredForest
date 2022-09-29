using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] GameObject attachedCharacter;

    [SerializeField] private int currentHitPoints = 1;
    [SerializeField] private int maxHitPoints = 1;

    [SerializeField] private float immuneTime = 1f;
    private bool isImmune = false;

    private bool isAlive = true;

    private void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    protected virtual void ReceiveDamage()
    {
        if (!isImmune && isAlive)
        {
            currentHitPoints--;

            if (currentHitPoints < 1)
            {
                Death();
            }
            else
            {
                animator.SetTrigger("Hurt");
                StartCoroutine(ImmuneCooldown());
            }
        }
    }

    protected virtual void Death()
    {
        animator.SetBool("IsDead", true);
        animator.SetTrigger("Death");

        isAlive = false;

        attachedCharacter.GetComponent<PlayerMovementController>().Death();
        attachedCharacter.GetComponent<PlayerAttackController>().Death();
    }

    private IEnumerator ImmuneCooldown()
    {
        isImmune = true;

        yield return new WaitForSeconds(immuneTime);

        isImmune = false;
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
