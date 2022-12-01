using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Animator animator;

    private PlayerMovementController playerMovementController;
    private DamageReceiver damageReceiver;

    [SerializeField] private BoxCollider2D swordCollider;

    private bool onAttackCooldown = false;
    private float attackCooldownDuration = Config.ATTACK_COOLDOWN_DURATION;

    private bool ableToDoSecondAttack = true;

    public void Reset()
    {
        onAttackCooldown = false;
        ableToDoSecondAttack = true;

        swordCollider.enabled = false;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();

        playerMovementController = GetComponent<PlayerMovementController>();
        damageReceiver = GetComponent<DamageReceiver>();
    }

    private void Update()
    {
        if (!GameManager.instance.IsGamePaused() && !GameManager.instance.IsTeleporting() && damageReceiver.IsAlive)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!onAttackCooldown)
                {
                    DoAttack();
                }
                else if (ableToDoSecondAttack)
                {
                    StartCoroutine(DoSecondAttack());
                }
            }
        }
    }

    public void DoAttack()
    {
        onAttackCooldown = true;

        if (playerMovementController.IsWallSliding())
        {
            playerMovementController.FinishWallSliding();
            StartCoroutine(CooldownToMove());
        }

        StartCoroutine(IsAttackingCooldown());
        animator.SetTrigger(Config.ANIMATOR_FIRST_ATTACK_TRIGGER);

        if(!playerMovementController.IsJumping())
            ableToDoSecondAttack = true;

        StartCoroutine(AttackCooldown());
    }

    private IEnumerator DoSecondAttack()
    {
        animator.SetBool(Config.ANIMATOR_IS_DOING_SECOND_ATTACK, true);

        ableToDoSecondAttack = false;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName(Config.PLAYER_ANIMATOR_FIRST_ATTACK_NAME))
        {
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(Config.PLAYER_ANIMATOR_FIRST_ATTACK_NAME))
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(Config.SECOND_ATTACK_DURATION);

        animator.SetBool(Config.ANIMATOR_IS_DOING_SECOND_ATTACK, false);
    }

    private IEnumerator IsAttackingCooldown()
    {
        animator.SetBool(Config.ANIMATOR_IS_ATTACKING, true);

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        animator.SetBool(Config.ANIMATOR_IS_ATTACKING, false);
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldownDuration);

        ableToDoSecondAttack = false;
        onAttackCooldown = false;
    }

    public void EndAttackCooldown()
    {
        onAttackCooldown = false;
    }

    private IEnumerator CooldownToMove()
    {
        playerMovementController.SetIsAbleToMove(false);

        yield return new WaitForSeconds(Config.WALLSLIDE_ATTACK_MOVEMENT_COOLDOWN);

        playerMovementController.SetIsAbleToMove(true);
    }

    public void Resurrection()
    {
        Reset();
    }

}
