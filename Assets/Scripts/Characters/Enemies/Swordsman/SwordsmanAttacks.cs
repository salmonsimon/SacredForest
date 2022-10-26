using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsmanAttacks : EnemyAttacks
{
    #region Animation

    [SerializeField] private AnimationClip[] animationClips;

    private Coroutine currentCoroutine = null;

    #endregion

    #region Logic Variables

    [SerializeField] private bool hasSecondAttack = false;
    [SerializeField] private bool hasThirdAttack = false;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        if (hasThirdAttack)
            hasSecondAttack = true;
    }

    public void AttackPattern(bool firstAttack, bool secondAttack, bool thirdAttack)
    {
        int attackCount = System.Convert.ToInt32(firstAttack) + System.Convert.ToInt32(secondAttack) + System.Convert.ToInt32(thirdAttack);

        PlayClips(attackCount);
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

    public void StopCurrentAttackCoroutine()
    {
        StopCoroutine(currentCoroutine);

        ResetIsAttacking();
    }

    public bool HasSecondAttack()
    {
        return hasSecondAttack;
    }

    public bool HasThirdAttack()
    {
        return hasThirdAttack;
    }

    protected override void Death()
    {
        base.Death();

        if (currentCoroutine != null)
            StopCurrentAttackCoroutine();
    }
}
