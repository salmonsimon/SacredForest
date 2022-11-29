using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : Mover
{
    private float addedFallGravity = Config.ADDED_FALL_GRAVITY;
    private float  addedGravityLowJump = Config.ADDED_GRAVITY_LOW_JUMP;

    private bool isAlive = true;

    private bool onlyAllowActions = false;

    public override void Reset()
    {
        base.Reset();

        isAlive = true;
        FirstRouteFinishedStateChange();
        SecondRouteFinishedStateChange();
    }

    private void Start()
    {
        FirstRouteFinishedStateChange();
        SecondRouteFinishedStateChange();

        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += AliveStatusChange;
        GetComponent<DamageReceiver>().OnCharacterDamaged += Damaged;

        GameManager.instance.GetCurrentProgressManager().OnFirstRouteFinishedStateChange += FirstRouteFinishedStateChange;
        GameManager.instance.GetCurrentProgressManager().OnSecondRouteFinishedStateChange += SecondRouteFinishedStateChange;
    }

    protected override void Update()
    {
        base.Update();

        if (!GameManager.instance.IsGamePaused() && !GameManager.instance.IsTeleporting() && isAlive)
        {
            jumpRememberTime -= Time.deltaTime;

            if (!onlyAllowActions)
            {
                movement.x = Input.GetAxisRaw(Config.MOVEMENT_ANIMATOR_HORIZONTAL);
                movement.y = Input.GetAxisRaw(Config.MOVEMENT_ANIMATOR_VERTICAL);
            }

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

    public bool IsJumpingUp()
    {
        return rigidBody.velocity.y > 0.1;
    }

    private void FirstRouteFinishedStateChange()
    {
        if (GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
            hasAbilityToDash = true;
        else
            hasAbilityToDash = false;
    }

    private void SecondRouteFinishedStateChange()
    {
        if (GameManager.instance.GetCurrentProgressManager().FinishedRoute2)
            hasAbilityToWallJump = true;
        else
            hasAbilityToWallJump = false;
    }

    public void OnlyAllowActions(bool value)
    {
        onlyAllowActions = value;
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

    protected virtual void Damaged()
    {
        if (isAlive)
        {
            StartCoroutine(GetComponent<DamageReceiver>().ImmuneCooldown());
        }
    }

    protected override void Death()
    {
        base.Death();

        isAlive = false;
        GameManager.instance.GetCurrentProgressManager().IncreaseDeathsCount();
    }

    protected override void Resurrection()
    {
        base.Resurrection();

        Reset();
    }
}
