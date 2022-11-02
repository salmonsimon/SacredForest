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

    protected virtual void ReceiveDamage(Damage damage)
    {
        if (!isImmune && isAlive)
        {
            Damage(damage.damage);
            GameManager.instance.GetCinemachineShake().ShakeCamera(Config.CAMERASHAKE_HIT_AMPLITUDE, Config.CAMERASHAKE_HIT_DURATION);
            PlayDamageAudioClip(damage.damageType);
            Bleed();

            if (currentHitPoints < 1)
            {
                Death();
            }
            else
            {
                animator.SetTrigger(Config.ANIMATOR_HURT_TRIGGER);
            }
        }
    }

    public IEnumerator ImmuneCooldown()
    {
        StopAllCoroutines();

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
        transform.gameObject.layer = LayerMask.NameToLayer(Config.DEAD_LAYER);

        animator.SetBool(Config.ANIMATOR_IS_DEAD, true);
        animator.SetTrigger(Config.ANIMATOR_DEATH_TRIGGER);

        if (TryGetComponent(out CharacterSFX characterSFX))
        {
            characterSFX.PlayRandomDeathAudioClip();
        }

        IsAlive = false;
    }

    public void Resurrect()
    {
        transform.gameObject.layer = LayerMask.NameToLayer(transform.gameObject.tag);

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

    private void PlayDamageAudioClip(string damageType)
    {
        if (gameObject.CompareTag(Config.PLAYER_TAG) || gameObject.CompareTag(Config.ENEMY_TAG))
        {
            switch (damageType)
            {
                case Config.SWORD_DAMAGE:
                    GameManager.instance.GetSFXManager().PlayRandomSwordDamageClip();
                    break;

                case Config.DEFAULT_DAMAGE:
                    GameManager.instance.GetSFXManager().PlayRandomDefaultDamageClip();
                    break;

                case Config.BLUDGEONING_DAMAGE:
                    GameManager.instance.GetSFXManager().PlayRandomBludgeoningDamageClip();
                    break;

                case Config.FIRE_DAMAGE:
                    GameManager.instance.GetSFXManager().PlayRandomFireDamageClip();
                    break;
            }
        }
    }
}
