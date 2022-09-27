using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Mover
{
    private float addedFallGravity = 0.3f;
    private float  addedGravityLowJump = 1.8f;

    private void Update()
    {
        if (!GameManager.instance.IsGamePaused())
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement != Vector2.zero)
            {
                animator.SetFloat("Horizontal", movement.x);
            }

            animator.SetFloat("Vertical", movement.y);

            animator.SetFloat("Speed", Mathf.Abs(movement.x));

            if (Input.GetKeyDown(KeyCode.Z))
            {
                jumpAction = true;
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                dashAction = true;
            }

            movement = movement.normalized;
        }
    }

    protected override void FixedUpdate()
    {
        if (IsFalling() && !ExceedsFallVelocity())
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * addedFallGravity * Time.deltaTime;
        }
        else if (IsJumpingUp() && IsJumping() && !Input.GetKey(KeyCode.Z))
        {
            rigidBody.velocity += Vector2.up * Physics2D.gravity.y * addedGravityLowJump * Time.deltaTime;
        }

        base.FixedUpdate();

        UpdateMotor();

        jumpAction = false;
        dashAction = false;
    }

    private bool IsJumpingUp()
    {
        return rigidBody.velocity.y > 0.1;
    }
}
