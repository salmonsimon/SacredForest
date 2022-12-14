using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elemental : Enemy
{
    [SerializeField] protected bool isTransformed = false;
    public bool IsTransformed { get { return IsTransformed; } set { isTransformed = value; } }

    [SerializeField] protected bool isTransforming = false;
    public bool IsTransforming { get { return isTransforming; }}

    #region Parameters

    [SerializeField] protected float transformationDuration = 3f;
    public float TransformationDuration { get { return transformationDuration; } }

    #endregion

    protected override void Damaged()
    {
        if (isAlive)
        {
            onActionCooldown = false;

            enemyMover.StayInPosition();

            CheckDamageToTransform();
        }
    }

    private void CheckDamageToTransform()
    {
        if (!isTransformed)
        {
            if (damageReceiver.CurrentHitPoints <= (float)(damageReceiver.MaxHitPoints / 2f))
            {
                Transform();
                isTransforming = true;
            }
        }

        if (!isTransforming)
        {
            StartCoroutine(Startled(Config.STUN_DURATION));
            StartCoroutine(damageReceiver.ImmuneCooldown());
        }

    }

    public virtual void Transform()
    {
        StartCoroutine(damageReceiver.SetImmune(transformationDuration + Config.STUN_DURATION));
        StartCoroutine(Startled(transformationDuration + Config.STUN_DURATION));

        isTransformed = true;

        StartCoroutine(WaitAndSetTransform());
    }

    private IEnumerator WaitAndSetTransform()
    {
        yield return new WaitForSeconds(Config.SMALL_DELAY);

        animator.SetBool(Config.ELEMENTAL_ANIMATOR_IS_TRANSFORMED, true);
        animator.SetTrigger(Config.ELEMENTAL_ANIMATOR_TRANSFORM_TRIGGER);

        yield return new WaitForSeconds(transformationDuration);

        isTransforming = false;
    }

    protected override void Death()
    {
        base.Death();
    }
}
