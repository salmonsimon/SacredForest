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
