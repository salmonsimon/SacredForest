using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    #region Animation

    protected Animator animator;

    #endregion

    #region Action Variables

    protected Vector2 direction;
    protected Vector3 refVelocity = Vector3.zero;

    #endregion

    #region Logic Variables

    protected bool isAbleToJumpBack = true;
    protected bool isAbleToDash = true;
    protected bool isAbleToMove = true;

    protected bool isWalkingAway = false;

    #endregion

    #region Parameters

    private Vector3 originalScale;
    protected Rigidbody2D rigidBody;

    private float dashForce = Config.DASH_FORCE;

    private float movementSmoothing = Config.MOVEMENT_SMOOTHING;
    [SerializeField] private float runSpeed = 2f;

    #endregion

    #region Enemy Mover Options

    [SerializeField] private bool hasJumpBack = false;
    [SerializeField] private bool hasDash = false;

    [SerializeField] private bool walksAwayWhenInAttackCooldown = false;
    [SerializeField] private bool movesWhileAttacking = false;

    #endregion

    protected void Awake()
    {
        originalScale = transform.localScale;

        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public void UpdateMotor(Vector2 movement, bool dashAction, bool jumpBackAction)
    {
        if (isAbleToMove)
        {
            UpdateDirection(movement);
            animator.SetFloat("Speed", Mathf.Abs(movement.x));

            if (isAbleToDash && dashAction)
            {
                StartCoroutine(DoDashAction());
            }

            if (isAbleToJumpBack && jumpBackAction)
            {
                StartCoroutine(DoJumpBackAction());
            }

            Vector3 targetVelocity = new Vector2(movement.x * runSpeed, rigidBody.velocity.y);
            rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref refVelocity, movementSmoothing);
        }
    }

    private void UpdateDirection(Vector2 movement)
    {
        if (movement != Vector2.zero)
            direction = movement.normalized;

        Flip(movement);
    }

    public void Flip(Vector2 movement)
    {
        if (movement.x > 0)
        {
            if (originalScale.x > 0)
            {
                transform.localScale = originalScale;
            }
            else
            {
                transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z);
            }
        }
        else if (movement.x < 0)
        {
            if (originalScale.x > 0)
            {
                transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z);
            }
            else
            {
                transform.localScale = originalScale;
            }
        }
    }

    public bool PickNonAttackAction(bool groundedAfterJumpBack)
    {
        bool jumpBackAction = false;

        int randomDecil = Random.Range(1, 10);

        if (randomDecil <= 2)
        {
            if (hasJumpBack && isAbleToDash && groundedAfterJumpBack)
            {
                jumpBackAction = true;
                StartCoroutine(MoveCooldownAfterJumpBack());
            }
        }
        else if (randomDecil > 2 && randomDecil <= 4)
        {
            if (walksAwayWhenInAttackCooldown && !isWalkingAway)
            {
                StartCoroutine(WalkingAwayCooldown(1f));
            }
        }


        return jumpBackAction;
    }

    private IEnumerator DoJumpBackAction()
    {
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING_BACK, true);
        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_JUMP_BACK_TRIGGER);

        rigidBody.AddForce(new Vector2(-transform.localScale.x * dashForce, 0));

        yield return new WaitForSeconds(Config.DASH_DURATION);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING_BACK, false);

        animator.SetFloat("Speed", 0);

        isAbleToJumpBack = false;

        yield return new WaitForSeconds(Config.JUMP_BACK_COOLDOWN);

        isAbleToJumpBack = true;
    }

    private IEnumerator DoDashAction()
    {
        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_DASH_TRIGGER);

        rigidBody.AddForce(new Vector2(transform.localScale.x * dashForce, 0));

        isAbleToDash = false;

        yield return new WaitForSeconds(Config.DASH_COOLDOWN);

        isAbleToDash = true;
    }

    public void SetIsAbleToMove(bool value)
    {
        isAbleToMove = value;
    }

    public bool IsAbleToJumpBack()
    {
        return isAbleToJumpBack;
    }

    public bool IsAbleToDash()
    {
        return isAbleToDash;
    }

    public bool HasJumpBack()
    {
        return hasJumpBack;
    }

    public bool HasDash()
    {
        return hasDash;
    }

    public bool WalksAwayWhenInAttackCooldown()
    {
        return walksAwayWhenInAttackCooldown;
    }

    public bool MovesWhileAttacking()
    {
        return movesWhileAttacking;
    }

    public bool IsWalkingAway()
    {
        return isWalkingAway;
    }

    public IEnumerator WalkingAwayCooldown(float duration)
    {
        isWalkingAway = true;

        yield return new WaitForSeconds(duration);

        isWalkingAway = false;
    }

    public IEnumerator MoveCooldownAfterJumpBack()
    {
        yield return new WaitForSeconds(.05f);

        SetIsAbleToMove(false);

        yield return new WaitForSeconds(1f);

        SetIsAbleToMove(true);
    }
}