using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mover : MonoBehaviour
{
    #region Animation

    protected Animator animator;
    [SerializeField] protected ParticleSystem particlesJump;
    [SerializeField] protected ParticleSystem particlesLand;

    #endregion

    #region GameObjects

    [SerializeField] protected CircleCollider groundCheck;
    [SerializeField] protected CircleCollider wallCheck;

    #endregion

    #region Action Variables

    protected Vector2 movement;
    protected Vector2 direction;
    protected Vector3 refVelocity = Vector3.zero;
    protected bool jumpAction;
    protected bool dashAction;

    #endregion

    #region Logic Variables

    [SerializeField] protected bool hasAbilityToDash;
    [SerializeField] protected bool hasAbilityToWallJump;

    protected bool isAbleToDash = true;
    protected bool isAbleToMove = true;

    private bool isGrounded;
    private bool isJumping;
    private bool isCollidingWithWall;
    private bool isWallSliding;


    #endregion

    #region Parameters

    private Vector3 originalScale;
    protected Rigidbody2D rigidBody;

    private float dashForce = Config.DASH_FORCE;

    private float runSpeed = Config.RUN_SPEED;
    private float movementSmoothing = Config.MOVEMENT_SMOOTHING;

    private float jumpForce = Config.JUMP_FORCE;
    private float wallJumpForceX = Config.WALL_JUMP_FORCE_X;
    private float wallJumpForceY = Config.WALL_JUMP_FORCE_Y;

    private float fallSpeedLimit = Config.FALL_SPEED_LIMIT;
    private float wallSlidingVelocity = Config.WALL_SLIDING_VELOCITY;

    #endregion

    private float groundedRememberDuration = .1f;
    private float groundedRememberTime = 0;

    protected float jumpRememberDuration = .1f;
    protected float jumpRememberTime = 0;


    protected void Awake()
    {
        originalScale = transform.localScale;

        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (!GameManager.instance.IsGamePaused() && !GameManager.instance.IsTeleporting())
        {
            groundedRememberTime -= Time.deltaTime;

            if (isGrounded)
            {
                groundedRememberTime = groundedRememberDuration;
            }
        }
    }


    protected virtual void FixedUpdate()
    {
        UpdateFallingVelocity();

        bool wasGrounded = isGrounded;
        isGrounded = groundCheck.IsColliding();

        if (!wasGrounded && isGrounded)
            OnLanding();
        /*
        if (wasGrounded && isGrounded && isJumping)
        {
            animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, false);
            isJumping = false;
        } 
        */

        isCollidingWithWall = false;

        if (!isGrounded && wallCheck.IsColliding())
        {
            isCollidingWithWall = true;
        }   
    }

    protected void UpdateMotor()
    {
        if (isAbleToMove)
        {
            UpdateDirection(movement);

            animator.SetFloat(Config.MOVEMENT_ANIMATOR_SPEED_Y, rigidBody.velocity.y);

            if (hasAbilityToDash && isAbleToDash && dashAction)
            {
                if (isWallSliding)
                {
                    FinishWallSliding();
                }

                StartCoroutine(DoDashAction());

                if (isGrounded || isWallSliding)
                {
                    particlesLand.Play();
                    particlesJump.Play();
                }

            }

            Vector3 targetVelocity = new Vector2(movement.x * runSpeed, rigidBody.velocity.y);
            rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref refVelocity, movementSmoothing);

            //if (jumpAction && !isJumping)
            if (jumpRememberTime > 0 && !isJumping)
            {
                if (groundedRememberTime > 0)
                {
                    jumpRememberTime = 0;
                    DoJumpAction();
                }
                else if (hasAbilityToWallJump && isWallSliding)
                {
                    jumpRememberTime = 0;
                    DoWallJumpAction();
                }
            }
            
            if (hasAbilityToWallJump && isCollidingWithWall && !isGrounded && !isWallSliding)
            {
                if (Mathf.Abs(rigidBody.velocity.x) > 0.1)
                {
                    DoWallSlideAction();
                }
            }
        }
    }

    private void UpdateFallingVelocity()
    {
        if (isWallSliding)
        {
            if (!isCollidingWithWall)
            {
                isWallSliding = false;
                animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_WALL_SLIDING, false);
            }
            else
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, wallSlidingVelocity);
            }
        }

        if (!isGrounded && IsFalling() && !isWallSliding)
        {
            if (ExceedsFallVelocity())
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -fallSpeedLimit);
            }
        }
    }

    private void UpdateDirection(Vector2 movement)
    {
        if (movement != Vector2.zero)
        {
            direction = movement.normalized;
        }

        if (movement.x > 0)
        {
            transform.localScale = originalScale;
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y * 1f, originalScale.z * 1f);
        }
    }

    private void Flip()
    {
        Vector2 wallCheckDirection = (wallCheck.transform.position - transform.position).normalized;

        if (wallCheckDirection.x > 0)
        {
            transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y * 1f, originalScale.z * 1f);
        }
        else if (wallCheckDirection.x < 0)
        {
            transform.localScale = originalScale;
        }
    }

    private void DoJumpAction()
    {
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, true);
        isJumping = true;

        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_JUMP_TRIGGER);

        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        rigidBody.AddForce(new Vector2(0f, jumpForce));

        if (isGrounded)
        {
            particlesLand.Play();
            particlesJump.Play();
        }
        
    }

    private void DoWallJumpAction()
    {
        isWallSliding = false;
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_WALL_SLIDING, false);

        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_JUMP_TRIGGER);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, true);
        isJumping = true;

        float flippedWallJumpForceX;

        Vector2 wallCheckDirection = (wallCheck.transform.position - transform.position).normalized;

        if (wallCheckDirection.x > 0)
        {
            flippedWallJumpForceX = -wallJumpForceX;
        }
        else
        {
            flippedWallJumpForceX = wallJumpForceX;
        }
        

        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(flippedWallJumpForceX, wallJumpForceY));

        Flip();

        particlesLand.Play();
        particlesJump.Play();
    }

    public void OnLanding()
    {
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, false);
        isJumping = false;

        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_WALL_SLIDING, false);
        isWallSliding = false;

        if(isGrounded)
            particlesLand.Play();
    }

    private void DoWallSlideAction()
    {
        isJumping = false;
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, false);

        isWallSliding = true;
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_WALL_SLIDING, true);
    }

    public void FinishWallSliding()
    {
        Flip();

        isCollidingWithWall = false;
        isWallSliding = false;
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_WALL_SLIDING, false);
    }

    private IEnumerator DoDashAction()
    {
        Cinemachine.CinemachineVirtualCamera vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

        float lookaheadSmoothing = vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_LookaheadSmoothing;
        vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_LookaheadSmoothing = 20;

        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_DASH_TRIGGER);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_DASHING, true);

        rigidBody.AddForce(new Vector2(transform.localScale.x * dashForce, 0));

        yield return new WaitForSeconds(Config.DASH_DURATION);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_DASHING, false);

        vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_LookaheadSmoothing = lookaheadSmoothing;

        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        isAbleToDash = false;

        yield return new WaitForSeconds(Config.DASH_COOLDOWN);

        isAbleToDash = true;
    }

    protected bool IsFalling()
    {
        return rigidBody.velocity.y < 0.1;
    }

    public Vector2 GetMovement()
    {
        return movement;
    }

    public bool IsJumping()
    {
        return isJumping;
    }

    public bool IsWallSliding()
    {
        return isWallSliding;
    }

    public void SetIsAbleToMove(bool value)
    {
        isAbleToMove = value;
    }

    public void BlockMovement(bool value)
    {
        if (value)
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public bool ExceedsFallVelocity()
    {
        return rigidBody.velocity.y < -fallSpeedLimit;
    }

    protected virtual void Death()
    {
        OnLanding();

        isAbleToMove = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    protected virtual void Resurrection()
    {
        OnLanding();

        isAbleToMove = true;

        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
