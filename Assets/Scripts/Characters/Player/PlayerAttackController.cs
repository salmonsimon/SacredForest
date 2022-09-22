using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : MonoBehaviour
{
    private Animator animator;
    private PlayerMovementController playerMovementController;

    private bool onCooldown = false;
    private float cooldownTime = .5f;

    private bool ableToDoSecondAttack = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void Update()
    {
        if (!GameManager.instance.IsGamePaused())
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (!onCooldown)
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
        onCooldown = true;

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

        yield return new WaitForSeconds(.1f);

        animator.SetBool("IsDoingSecondAttack", false);
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime*(0.75f));

        ableToDoSecondAttack = false;

        yield return new WaitForSeconds(cooldownTime*(0.25f));

        onCooldown = false;
    }

    private IEnumerator CooldownToMove()
    {
        playerMovementController.SetIsAbleToMove(false);

        yield return new WaitForSeconds(.3f);

        playerMovementController.SetIsAbleToMove(true);
    }
}
