using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Mover
{
    private float addedFallGravity = Config.ADDED_FALL_GRAVITY;
    private float  addedGravityLowJump = Config.ADDED_GRAVITY_LOW_JUMP;

    private bool isAlive = true;

    private void Start()
    {
        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += AliveStatusChange;
    }

    protected override void Update()
    {
        base.Update();

        if (!GameManager.instance.IsGamePaused() && !GameManager.instance.IsTeleporting() && isAlive)
        {
            jumpRememberTime -= Time.deltaTime;

            movement.x = Input.GetAxisRaw(Config.MOVEMENT_ANIMATOR_HORIZONTAL);
            movement.y = Input.GetAxisRaw(Config.MOVEMENT_ANIMATOR_VERTICAL);

            if (movement != Vector2.zero)
            {
                animator.SetFloat(Config.MOVEMENT_ANIMATOR_HORIZONTAL, movement.x);
            }

            animator.SetFloat(Config.MOVEMENT_ANIMATOR_VERTICAL, movement.y);

            animator.SetFloat(Config.MOVEMENT_ANIMATOR_SPEED, Mathf.Abs(movement.x));

            if (Input.GetKeyDown(KeyCode.Z))
            {
                jumpRememberTime = jumpRememberDuration;
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
        if (!GameManager.instance.IsGamePaused() && !GameManager.instance.IsTeleporting() && isAlive)
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

            dashAction = false;
        }
    }

    private bool IsJumpingUp()
    {
        return rigidBody.velocity.y > 0.1;
    }

    private void AliveStatusChange()
    {
        if (GetComponent<DamageReceiver>().IsAlive)
        {
            Resurrection();
        }
        else
        {
            Death();
        }
    }

    protected override void Death()
    {
        base.Death();

        isAlive = false;
    }

    protected override void Resurrection()
    {
        base.Resurrection();

        isAlive = true;
    }
}
