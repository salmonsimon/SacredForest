using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    #region Animation

    protected Animator animator;
    [SerializeField] protected ParticleSystem particlesJump;
    [SerializeField] protected ParticleSystem particlesLand;

    #endregion

    #region Game Objects

    private DamageReceiver damageReceiver;

    #endregion

    #region Check Colliders

    [SerializeField] private CircleCollider groundCheck;
    [SerializeField] private CircleCollider walkAwayCheck;
    [SerializeField] private CircleCollider stillMoreToWalkCheck;
    [SerializeField] private CircleCollider jumpBackGroundCheck;

    #endregion

    #region Action Variables

    protected Vector2 direction;
    protected Vector3 refVelocity = Vector3.zero;

    #endregion

    #region Logic Variables

    protected bool isGrounded = false;
    protected bool isGroundedAfterJumpBack = false;
    protected bool stillMoreToWalk = false;

    protected bool isAbleToJumpBack = true;
    protected bool isAbleToDash = true;
    protected bool isAbleToMove = true;

    protected bool isWalkingAway = false;

    protected bool isAlive = true;

    #endregion

    #region Parameters

    private Vector3 originalScale;
    protected Rigidbody2D rigidBody;

    [SerializeField] private float dashForce = Config.DASH_FORCE;
    [SerializeField] private float walkAwayDuration = 1.5f;
    private float jumpBackForce = Config.JUMP_BACK_FORCE;

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
        damageReceiver = GetComponent<DamageReceiver>();
    }

    private void Start()
    {
        damageReceiver.OnCharacterAliveStatusChange += Death;
    }

    private void FixedUpdate()
    {
        bool wasGrounded = isGrounded;
        isGrounded = groundCheck.IsColliding();

        if (!wasGrounded && isGrounded)
            OnLanding();

        isGroundedAfterJumpBack = jumpBackGroundCheck.IsColliding();
        stillMoreToWalk = stillMoreToWalkCheck.IsColliding();
    }

    protected virtual void Death()
    {
        isAbleToMove = false;
        isAlive = false;

        OnLanding();

        if (isGrounded)
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<CapsuleCollider2D>().enabled = false;
        }
        else
        {
            StartCoroutine(WaitUntilGroundedToDisable());
        }

    }

    private IEnumerator WaitUntilGroundedToDisable()
    {
        while (!isGrounded)
        {
            if (isGrounded)
            {
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                GetComponent<CapsuleCollider2D>().enabled = false;

                break;
            }

            yield return new WaitForSeconds(.5f);
        }
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
                return;
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

    public bool PickNonAttackAction(bool groundedAfterJumpBack, int jumpBackDecil = 2, int walkAwayDecil = 2)
    {
        bool jumpBackAction = false;

        int randomDecil = Random.Range(1, 10);

        if (randomDecil <= jumpBackDecil)
        {
            if (hasJumpBack && isAbleToJumpBack && groundedAfterJumpBack)
            {
                jumpBackAction = true;
            }
        }
        else if (randomDecil > jumpBackDecil && randomDecil <= jumpBackDecil + walkAwayDecil)
        {
            if (walksAwayWhenInAttackCooldown && !isWalkingAway && walkAwayCheck.IsColliding())
            {
                StartCoroutine(WalkingAwayCooldown(walkAwayDuration));
            }
        }

        return jumpBackAction;
    }

    private IEnumerator DoJumpBackAction()
    {
        StartCoroutine(MovementCooldown(Config.MOVEMENT_AFTER_JUMP_BACK_COOLDOWN));

        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING_BACK, true);
        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_JUMP_BACK_TRIGGER);

        rigidBody.AddForce(new Vector2(-transform.localScale.x * (jumpBackForce / 2f), jumpBackForce / 2f));

        particlesLand.Play();
        particlesJump.Play();

        yield return new WaitForSeconds(Config.JUMP_BACK_DURATION);
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

    public void OnLanding()
    {
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING_BACK, false);

        if (isGrounded)
        {
            rigidBody.velocity *= .1f;
            particlesLand.Play();
        }
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

    public IEnumerator MovementCooldown(float duration)
    {
        SetIsAbleToMove(false);

        yield return new WaitForSeconds(duration);

        SetIsAbleToMove(true);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsGroundedAfterJumpBack()
    {
        return isGroundedAfterJumpBack;
    }

    public bool StillMoreToWalk()
    {
        return stillMoreToWalk;
    }
}