using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    private void Update()
    {
        if (!GameManager.instance.IsGamePaused)
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
        base.FixedUpdate();

        UpdateMotor();

        jumpAction = false;
        dashAction = false;
    }
}
