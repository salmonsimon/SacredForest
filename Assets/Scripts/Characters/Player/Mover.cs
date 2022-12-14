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
    [SerializeField] protected GroundTypeCheck groundTypeCheck;
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

    private float dashingLookaheadSmoothing = 20;

    #endregion

    private float groundedRememberDuration = Config.SMALL_DELAY;
    private float groundedRememberTime = 0;

    protected float jumpRememberDuration = Config.SMALL_DELAY;
    protected float jumpRememberTime = 0;

    public virtual void Reset()
    {
        isGrounded = false;
        isJumping = false;
        isCollidingWithWall = false;
        isWallSliding = false;

        isAbleToDash = true;
        isAbleToMove = true;

        groundedRememberTime = 0;
        jumpRememberTime = 0;
    }

    protected virtual void Awake()
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
        {
            OnLanding();
        }

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

        GetComponent<CharacterSFX>().PlayRandomJumpAudioClip();

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

        GetComponent<CharacterSFX>().PlayRandomJumpAudioClip();

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

    

    private void ResetJumpAndWallSlide()
    {
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, false);
        isJumping = false;

        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_WALL_SLIDING, false);
        isWallSliding = false;
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
        StartCoroutine(GetComponent<DamageReceiver>().SetImmune(Config.MEDIUM_DELAY));
        StartCoroutine(ToDashLayerCooldown(Config.MEDIUM_DELAY));

        Cinemachine.CinemachineVirtualCamera vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

        float lookaheadSmoothing = vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_LookaheadSmoothing;
        vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_LookaheadSmoothing = dashingLookaheadSmoothing;

        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_DASH_TRIGGER);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_DASHING, true);

        rigidBody.AddForce(new Vector2(transform.localScale.x * dashForce, 0));

        yield return new WaitForSeconds(Config.DASH_DURATION);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_DASHING, false);

        vcam.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>().m_LookaheadSmoothing = lookaheadSmoothing;

        StartCoroutine(DashCooldown());
    }

    private IEnumerator ToDashLayerCooldown(float duration)
    {
        gameObject.layer = LayerMask.NameToLayer(Config.DASH_LAYER);

        yield return new WaitForSeconds(duration);

        gameObject.layer = LayerMask.NameToLayer(Config.PLAYER_LAYER);
    }

    public void OnLanding()
    {
        ResetJumpAndWallSlide();

        particlesLand.Play();
        PlayLandingSound();
    }

    private void PlayLandingSound()
    {
        string floorType = groundTypeCheck.GetGroundType();

        switch (floorType)
        {
            case null:
                GameManager.instance.GetSFXManager().PlayGrassLandingSound();
                break;

            case Config.GRASS_FLOOR:
                GameManager.instance.GetSFXManager().PlayGrassLandingSound();
                break;

            case Config.ROCK_FLOOR:
                GameManager.instance.GetSFXManager().PlayRockLandingSound();
                break;

            case Config.WOOD_FLOOR:
                GameManager.instance.GetSFXManager().PlayWoodLandingSound();
                break;

            case Config.DIRT_FLOOR:
                GameManager.instance.GetSFXManager().PlayDirtLandingSound();
                break;
        }
    }

    public void PlayStepSound()
    {
        if (isGrounded)
        {
            string floorType = groundTypeCheck.GetGroundType();

            switch (floorType)
            {
                case null:
                    GameManager.instance.GetSFXManager().PlayGrassStepSound();
                    break;

                case Config.GRASS_FLOOR:
                    GameManager.instance.GetSFXManager().PlayGrassStepSound();
                    break;

                case Config.ROCK_FLOOR:
                    GameManager.instance.GetSFXManager().PlayRockStepSound();
                    break;

                case Config.WOOD_FLOOR:
                    GameManager.instance.GetSFXManager().PlayWoodStepSound();
                    break;

                case Config.DIRT_FLOOR:
                    GameManager.instance.GetSFXManager().PlayDirtStepSound();
                    break;
            }
        }
    }

    private IEnumerator DashCooldown()
    {
        isAbleToDash = false;

        yield return new WaitForSeconds(Config.DASH_COOLDOWN);

        isAbleToDash = true;
    }

    public void StayInPosition()
    {
        animator.SetFloat(Config.MOVEMENT_ANIMATOR_SPEED, 0);

        rigidBody.velocity = Vector2.zero;
        movement = Vector2.zero;
        UpdateMotor();
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

    public bool IsGrounded()
    {
        return groundCheck.IsColliding();
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

    public virtual void Death()
    {
        ResetJumpAndWallSlide();

        isAbleToMove = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX;
    }

    public virtual void Resurrection()
    {
        ResetJumpAndWallSlide();

        isAbleToMove = true;

        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
