using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float immuneTime = 1f;
    private bool isImmune = false;

    [SerializeField] private int maxHitPoints = 1;
    public int MaxHitPoints
    {
        get { return maxHitPoints; }
    }

    #region Properties

    #region CurrentHitPoints

    [SerializeField] private int currentHitPoints = 1;
    public int CurrentHitPoints
    {
        get { return currentHitPoints; }
    }

    public void Damage(int damage)
    {
        currentHitPoints -= damage;

        if (OnCharacterDamaged != null)
            OnCharacterDamaged();
    }

    #endregion

    #region IsAlive Property

    private bool isAlive = true;
    public bool IsAlive
    {
        get { return isAlive; }
        private set
        {
            if (isAlive == value) return;

            isAlive = value;
            if (OnCharacterAliveStatusChange != null)
                OnCharacterAliveStatusChange();
        }
    }

    public delegate void OnCharacterAliveStatusChangeDelegate();
    public event OnCharacterAliveStatusChangeDelegate OnCharacterAliveStatusChange;

    #endregion

    public delegate void OnCharacterDamagedDelegate();
    public event OnCharacterDamagedDelegate OnCharacterDamaged;

    #endregion

    private void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    protected virtual void ReceiveDamage(int damage)
    {
        if (!isImmune && isAlive)
        {
            Damage(damage);
            GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_HIT_AMPLITUDE, Config.CAMERASHAKE_HIT_DURATION);
            Bleed();

            if (currentHitPoints < 1)
            {
                Death();
            }
            else
            {
                animator.SetTrigger(Config.ANIMATOR_HURT_TRIGGER);
                //StartCoroutine(ImmuneCooldown());
            }
        }
    }

    public IEnumerator ImmuneCooldown()
    {
        isImmune = true;

        yield return new WaitForSeconds(immuneTime);

        isImmune = false;
    }

    public IEnumerator SetImmune(float duration)
    {
        StopAllCoroutines();

        isImmune = true;

        yield return new WaitForSeconds(duration);

        isImmune = false;
    }

    protected virtual void Death()
    {
        animator.SetBool(Config.ANIMATOR_IS_DEAD, true);
        animator.SetTrigger(Config.ANIMATOR_DEATH_TRIGGER);

        IsAlive = false;
    }

    public void Resurrect()
    {
        currentHitPoints = maxHitPoints;

        animator.SetBool(Config.ANIMATOR_IS_DEAD, false);

        IsAlive = true;
    }

    private void Bleed()
    {
        if (gameObject.CompareTag(Config.PLAYER_TAG) || gameObject.CompareTag(Config.ENEMY_TAG))
        {
            GameManager.instance.GetBloodManager().Bleed(gameObject.transform, gameObject.transform.position);
        }
    }
}
