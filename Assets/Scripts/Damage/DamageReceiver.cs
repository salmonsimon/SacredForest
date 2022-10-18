using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private int currentHitPoints = 1;
    [SerializeField] private int maxHitPoints = 1;

    [SerializeField] private float immuneTime = 1f;
    private bool isImmune = false;

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

    private void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    protected virtual void ReceiveDamage(int damage)
    {
        if (!isImmune && isAlive)
        {
            currentHitPoints -= damage;
            GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_HIT_AMPLITUDE, Config.CAMERASHAKE_HIT_DURATION);
            Bleed();

            if (currentHitPoints < 1)
            {
                Death();
            }
            else
            {
                animator.SetTrigger(Config.ANIMATOR_HURT_TRIGGER);
                StartCoroutine(ImmuneCooldown());
            }
        }
    }

    private IEnumerator ImmuneCooldown()
    {
        isImmune = true;

        yield return new WaitForSeconds(immuneTime);

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
