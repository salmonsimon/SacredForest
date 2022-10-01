using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAttacks : MonoBehaviour
{
    #region Animation

    [SerializeField] private AnimationClip[] animationClips;
    private Animator animator;

    private Coroutine currentCoroutine = null;

    #endregion

    #region Logic Variables

    private bool isAttacking = false;

    private bool onAttackCooldown = false;
    [SerializeField] private float attackCooldownDuration = 3f;

    [SerializeField] private bool hasSecondAttack = false;
    [SerializeField] private bool hasThirdAttack = false;

    private bool isDead = false;

    #endregion


    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (hasThirdAttack)
            hasSecondAttack = true;
    }

    public void AttackPattern(bool firstAttack, bool secondAttack, bool thirdAttack)
    {
        int attackCount = System.Convert.ToInt32(firstAttack) + System.Convert.ToInt32(secondAttack) + System.Convert.ToInt32(thirdAttack);

        PlayClips(attackCount);

        StartCoroutine(AttackCooldown());
    }

    public void PlayClips(int attackCount)
    {
        float cumulativeLength = 0;

        for (int i = 0; i < attackCount; i++)
        {
            currentCoroutine = StartCoroutine(PlayClip(Animator.StringToHash(animationClips[i].name), cumulativeLength));
            cumulativeLength += animationClips[i].length;
        }

        StartCoroutine(IsAttackingCooldown(cumulativeLength));
    }

    IEnumerator PlayClip(int clipHash, float startTime)
    {
        yield return new WaitForSeconds(startTime);

        animator.Play(clipHash);
    }

    private IEnumerator IsAttackingCooldown(float duration)
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

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void StopCurrentAttackCoroutine()
    {
        StopCoroutine(currentCoroutine);
    }

    public bool OnAttackCooldown()
    {
        return onAttackCooldown;
    }

    public bool HasSecondAttack()
    {
        return hasSecondAttack;
    }

    public bool HasThirdAttack()
    {
        return hasThirdAttack;
    }
}
