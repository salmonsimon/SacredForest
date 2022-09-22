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

    [SerializeField] protected CircleCollisionCheck groundCheck;
    [SerializeField] protected CircleCollisionCheck wallCheck;

    #endregion

    #region Action Variables

    protected Vector2 movement;
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
    private bool isCollidingWithWall;
    private bool isWallSliding;
    

    #endregion

    #region Parameters

    private Vector3 originalScale;
    private Rigidbody2D rigidBody;

    [SerializeField] private float dashForce = 400f;

    private float runVelocity = 3f;
    private float movementSmoothing = .05f;

    private float jumpForce = 200;
    private float wallJumpForceX = -400;
    private float wallJumpForceY = 160;


    [SerializeField] private float limitFallSpeed = 20f;
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

            UpdateFallingVelocity();
            animator.SetFloat("SpeedY", rigidBody.velocity.y);

            if (hasAbilityToDash && isAbleToDash && dashAction)
            {
                if (isWallSliding)
                {
                    Flip();

                    isCollidingWithWall = false;
                    isWallSliding = false;
                    animator.SetBool("IsWallSliding", false);
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
                if (Mathf.Abs(movement.x) > 0.1)
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
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, -limitFallSpeed);
        }
    }

    private void UpdateDirection(Vector2 movement)
    {
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
        transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y * 1f, originalScale.z * 1f);
    }

    private void DoJumpAction()
    {
        animator.SetBool("IsJumping", true);
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

        Flip();
        rigidBody.velocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(wallJumpForceX, wallJumpForceY));

        particlesLand.Play();
        particlesJump.Play();
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsWallSliding", false);

        isWallSliding = false;

        particlesLand.Play();
    }

    private void DoWallSlideAction()
    {
        isWallSliding = true;

        animator.SetBool("IsWallSliding", true);
    }

    private IEnumerator DoDashAction()
    {
        animator.SetTrigger("Dash");

        rigidBody.velocity = new Vector2(transform.localScale.x * dashForce, 0);

        yield return new WaitForSeconds(.1f);

        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        isAbleToDash = false;

        yield return new WaitForSeconds(.5f);

        isAbleToDash = true;
    }

    private bool IsFalling()
    {
        return rigidBody.velocity.y < 0.1;
    }
}
