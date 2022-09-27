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

    private protected bool isAbleToDash = true;
    private protected bool isAbleToMove = true;

    private bool isGrounded;
    private bool isJumping;
    private bool isCollidingWithWall;
    private bool isWallSliding;


    #endregion

    #region Parameters

    private Vector3 originalScale;
    private Rigidbody2D rigidBody;

    private float dashForce = 25f;

    private float runVelocity = 3f;
    private float movementSmoothing = .05f;

    private float jumpForce = 200;
    private float wallJumpForceX = 400;
    private float wallJumpForceY = 160;


    [SerializeField] private float limitFallSpeed = 10f;
    private float wallSlidingVelocity = -1f;

    #endregion


    protected void Awake()
    {
        originalScale = transform.localScale;

        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }


    protected virtual void FixedUpdate()
    {
        UpdateFallingVelocity();

        bool wasGrounded = isGrounded;
        isGrounded = groundCheck.IsColliding();

        if (!wasGrounded && isGrounded)
            OnLanding();

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

            animator.SetFloat("SpeedY", rigidBody.velocity.y);

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

            Vector3 targetVelocity = new Vector2(movement.x * runVelocity, rigidBody.velocity.y);
            rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref refVelocity, movementSmoothing);

            if (jumpAction)
            {
                if (isGrounded)
                {
                    DoJumpAction();
                }
                else if (hasAbilityToWallJump && isWallSliding)
                {
                    DoWallJumpAction();
                }
            }
            
            if (isCollidingWithWall && !isGrounded && !isWallSliding)
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
                animator.SetBool("IsWallSliding", false);
            }
            else
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, wallSlidingVelocity);
            }
        }

        if (!isGrounded && IsFalling() && !isWallSliding)
        {
            if (rigidBody.velocity.y < -limitFallSpeed)
            {
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -limitFallSpeed);
                Debug.Log("Limiting falling speed");
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
        animator.SetBool("IsJumping", true);
        isJumping = true;

        animator.SetTrigger("Jump");

        rigidBody.AddForce(new Vector2(0f, jumpForce));

        particlesLand.Play();
        particlesJump.Play();
    }

    private void DoWallJumpAction()
    {
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);

        animator.SetTrigger("Jump");
        animator.SetBool("IsJumping", true);
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
        animator.SetBool("IsJumping", false);
        isJumping = false;

        animator.SetBool("IsWallSliding", false);
        isWallSliding = false;

        particlesLand.Play();
    }

    private void DoWallSlideAction()
    {
        isJumping = false;
        animator.SetBool("IsJumping", false);

        isWallSliding = true;
        animator.SetBool("IsWallSliding", true);
    }

    public void FinishWallSliding()
    {
        Flip();

        isCollidingWithWall = false;
        isWallSliding = false;
        animator.SetBool("IsWallSliding", false);
    }

    private IEnumerator DoDashAction()
    {
        animator.SetTrigger("Dash");
        animator.SetBool("IsDashing", true);

        rigidBody.velocity = new Vector2(transform.localScale.x * dashForce, 0);

        yield return new WaitForSeconds(.1f);
        animator.SetBool("IsDashing", false);


        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        isAbleToDash = false;

        yield return new WaitForSeconds(.5f);

        isAbleToDash = true;
    }

    public void Death()
    {
        isAbleToMove = false;
    }

    private bool IsFalling()
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
}
