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
            if (isAlive = value) return;

            isAlive = value;
            if (OnCharacterDeath != null)
                OnCharacterDeath();
        }
    }

    public delegate void OnCharacterDeathDelegate();
    public event OnCharacterDeathDelegate OnCharacterDeath;

    private void Awake()
    {
        currentHitPoints = maxHitPoints;
    }

    protected virtual void ReceiveDamage()
    {
        if (!isImmune && isAlive)
        {
            currentHitPoints--;
            GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_HIT_AMPLITUDE, Config.CAMERASHAKE_HIT_DURATION);

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

    protected virtual void Death()
    {
        animator.SetBool(Config.ANIMATOR_IS_DEAD, true);
        animator.SetTrigger(Config.ANIMATOR_DEATH_TRIGGER);

        IsAlive = false;

        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<CapsuleCollider2D>().enabled = false;
    }

    private IEnumerator ImmuneCooldown()
    {
        isImmune = true;

        yield return new WaitForSeconds(immuneTime);

        isImmune = false;
    }
}
