using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    #region Animation

    protected Animator animator;

    #endregion

    #region Logic Variables

    [SerializeField] protected bool isAttacking = false;

    protected bool onAttackCooldown = false;
    [SerializeField] protected float attackCooldownDuration = 2f;

    protected bool isAlive = true;

    #endregion


    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        DamageReceiver damageReceiver = GetComponent<DamageReceiver>();

        damageReceiver.OnCharacterAliveStatusChange += Death;
        damageReceiver.OnCharacterDamaged += Damaged;
    }

    protected IEnumerator PlayClip(int clipHash, float startTime)
    {
        yield return new WaitForSeconds(startTime);

        if (isAlive)
        {
            animator.Play(clipHash);
        }
    }

    protected virtual IEnumerator IsAttackingCooldown(float duration)
    {
        isAttacking = true;

        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }

    public IEnumerator AttackCooldown()
    {
        onAttackCooldown = true;

        yield return new WaitForSeconds(attackCooldownDuration);

        onAttackCooldown = false;
    }

    public bool OnAttackCooldown()
    {
        return onAttackCooldown;
    }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    protected virtual void Damaged()
    {

    }

    protected virtual void Death()
    {
        isAlive = false;
    }
}
