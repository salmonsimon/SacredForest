using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovementController playerMovementController;

    private bool onAttackCooldown = false;
    private float attackCooldownDuration = Config.ATTACK_COOLDOWN_DURATION;

    private bool ableToDoSecondAttack = true;

    private bool isAlive = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void Start()
    {
        GetComponent<DamageReceiver>().OnCharacterDeath += Death;
    }

    private void Update()
    {
        if (!GameManager.instance.IsGamePaused() && !GameManager.instance.IsTeleporting() && isAlive)
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
            

        animator.SetTrigger("FirstAttack");

        if(!playerMovementController.IsJumping())
            ableToDoSecondAttack = true;

        StartCoroutine(Cooldown());
    }

    private IEnumerator DoSecondAttack()
    {
        animator.SetBool("IsDoingSecondAttack", true);

        ableToDoSecondAttack = false;

        yield return new WaitForSeconds(Config.SECOND_ATTACK_DURATION);

        animator.SetBool("IsDoingSecondAttack", false);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(attackCooldownDuration*(0.75f));

        ableToDoSecondAttack = false;

        yield return new WaitForSeconds(attackCooldownDuration*(0.25f));

        onAttackCooldown = false;
    }

    private IEnumerator CooldownToMove()
    {
        playerMovementController.SetIsAbleToMove(false);

        yield return new WaitForSeconds(Config.WALLSLIDE_ATTACK_MOVEMENT_COOLDOWN);

        playerMovementController.SetIsAbleToMove(true);
    }

    public void Death()
    {
        isAlive = false;
    }
}
