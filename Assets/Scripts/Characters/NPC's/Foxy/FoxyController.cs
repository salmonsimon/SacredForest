using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxyController : MonoBehaviour
{
    #region Animator

    private Animator animator;
    [SerializeField] protected ParticleSystem particlesJump;
    [SerializeField] protected ParticleSystem particlesLand;

    #endregion

    #region Components

    private Rigidbody2D rigidBody;
    [SerializeField] Transform goalPoint;

    #endregion

    #region Check Colliders

    [SerializeField] private CircleCollider groundCheck;
    [SerializeField] private CircleCollider hasToJumpCheck;
    [SerializeField] private BoxCollider followCheck;

    #endregion

    #region Parameters

    private Vector3 originalScale;

    private Vector3 refVelocity = Vector3.zero;
    private float movementSmoothing = Config.MOVEMENT_SMOOTHING;

    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float movingTime = 1f;

    #endregion

    #region Logic Variables

    private bool isGrounded = false;
    private bool isBeingFollowed = false;
    private bool isMoving = false;
    private bool hasArrived = false;

    private Vector2 movement;
    private GameObject player;

    private float relativePositionToGoalX;

    #endregion

    private Coroutine isMovingCoroutine;
 
    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();

        player = GameManager.instance.GetPlayer();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        bool wasGrounded = isGrounded;
        isGrounded = groundCheck.IsColliding();

        if (!wasGrounded && isGrounded)
            OnLanding();

        isBeingFollowed = followCheck.IsColliding();

        if (isBeingFollowed && isMoving)
        {
            StopCoroutine(isMovingCoroutine);
            isMovingCoroutine = StartCoroutine(IsMovingCooldown());
        }

        relativePositionToGoalX = goalPoint.position.x - transform.position.x;

        if (relativePositionToGoalX < .1f)
        {
            hasArrived = true;
            StayInPosition();
        }
    }

    private void FixedUpdate()
    {
        if (isMoving && !hasArrived)
            UpdateMotor(movement);

        movement = Vector2.zero;

        if (!isBeingFollowed && !isMoving)
        {
            StayInPosition();
            FlipTowardsPlayer();
        }
        else
        {
            if (!hasArrived)
            {
                movement = WalksTowardsGoal();
            }
        }
    }

    public void UpdateMotor(Vector2 movement)
    {
        if (!hasArrived)
        {
            animator.SetFloat(Config.MOVEMENT_ANIMATOR_SPEED, Mathf.Abs(movement.x));

            if (isGrounded && !hasToJumpCheck.IsColliding())
            {
                StartCoroutine(JumpAction());
            }

            Vector3 targetVelocity = new Vector2(movement.x * runSpeed, rigidBody.velocity.y);
            rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref refVelocity, movementSmoothing);
        }
    }

    protected Vector2 WalksTowardsGoal()
    {
        Vector2 movement;
        Flip(new Vector2(relativePositionToGoalX, 0));

        if (relativePositionToGoalX > 0)
        {
            movement = Vector2.right;
        }
        else
        {
            movement = Vector2.left;
        }

        isMovingCoroutine = StartCoroutine(IsMovingCooldown());
        return movement;
    }

    private IEnumerator IsMovingCooldown()
    {
        isMoving = true;

        yield return new WaitForSeconds(movingTime);

        isMoving = false;
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

    public void FlipTowardsPlayer()
    {
        float relativePlayerPositionX = player.transform.position.x - transform.position.x;
        Flip(new Vector2(relativePlayerPositionX, 0));
    }

    private IEnumerator JumpAction()
    {
        yield return null;

        /*
        StartCoroutine(MovementCooldown(Config.MOVEMENT_AFTER_JUMP_BACK_COOLDOWN));

        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING_BACK, true);
        animator.SetTrigger(Config.MOVEMENT_ANIMATOR_JUMP_BACK_TRIGGER);

        GetComponent<CharacterSFX>().PlayRandomJumpAudioClip();

        rigidBody.AddForce(new Vector2(-transform.localScale.x * (jumpBackForce / 2f), jumpBackForce / 2f));

        particlesLand.Play();
        particlesJump.Play();

        yield return new WaitForSeconds(Config.JUMP_BACK_DURATION);
        animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING_BACK, false);

        animator.SetFloat(Config.MOVEMENT_ANIMATOR_SPEED, 0);

        isAbleToJumpBack = false;

        yield return new WaitForSeconds(Config.JUMP_BACK_COOLDOWN);

        isAbleToJumpBack = true;
        */
    }

    public void OnLanding()
    {
        //animator.SetBool(Config.MOVEMENT_ANIMATOR_IS_JUMPING, false);

        if (isGrounded)
        {
            rigidBody.velocity *= .1f;
            particlesLand.Play();
        }
    }

    public void StayInPosition()
    {
        animator.SetFloat(Config.MOVEMENT_ANIMATOR_SPEED, 0);

        rigidBody.velocity = Vector2.zero;
        UpdateMotor(Vector2.zero);
    }
}
