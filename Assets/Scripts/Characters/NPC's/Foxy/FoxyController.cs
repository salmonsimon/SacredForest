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

        if (isBeingFollowed && isMoving || PlayerIsCloserToGoal())
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

        if (!isBeingFollowed && !isMoving && !PlayerIsCloserToGoal())
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

    public void OnLanding()
    {
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

    private bool PlayerIsCloserToGoal()
    {
        return player.transform.position.x - transform.position.x > 0;
    }
}
