using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArcherAttacks), typeof(PlayerDetection))]
public class Archer : Enemy
{
    #region Controllers

    private ArcherAttacks archerAttacks;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider attackZoneCollider;
    [SerializeField] private BoxCollider dangerZoneCollider;

    #endregion

    #region Action Variables

    private bool shootArrowAction = false;

    private bool jumpBackAction = false;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        archerAttacks = GetComponent<ArcherAttacks>();
    }

    protected override void Start()
    {
        base.Start();

        playerDetection.OnDetectedPlayer += PlayerDetected;
    }

    private void FixedUpdate()
    {
        if (isPlayerAlive)
            isPlayerAlive = player.GetComponent<DamageReceiver>().IsAlive;

        if (!isAlive || !isPlayerAlive || isStartled)
        {
            enemyMover.StayInPosition();
            return;
        }

        movement = Vector2.zero;

        relativePlayerPositionX = player.transform.position.x - transform.position.x;

        if (!enemyMover.IsWalkingAway() && playerDetection.DetectedPlayer)
            enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));

        if (!playerDetection.DetectedPlayer)
        {
            playerDetection.CheckForPlayer();

            if (isStartled)
                return;
        }

        if (playerDetection.DetectedPlayer)
        {
            if (!archerAttacks.OnAttackCooldown())
            {
                PickRandomAttackPattern();
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
            else if (enemyMover.IsWalkingAway() && enemyMover.StillMoreToWalk() && !archerAttacks.IsAttacking())
            {
                movement = WalksAway();
            }
            else if (!onActionCooldown && !archerAttacks.IsAttacking() && dangerZoneCollider.IsColliding())
            {
                jumpBackAction = enemyMover.PickNonAttackAction(enemyMover.IsGroundedAfterJumpBack(), 4, 6);
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
        }
        else if (playerDetection.DetectedPlayer && (!archerAttacks.IsAttacking() || enemyMover.MovesWhileAttacking()))
        {
            if (enemyMover.StillMoreToWalk())
            {
                if (enemyMover.IsWalkingAway())
                {
                    movement = WalksAway();
                }
            }
        }

        enemyMover.UpdateMotor(movement, false, jumpBackAction);

        ResetActionBooleans();
    }

    private void PickRandomAttackPattern()
    {
        StartCoroutine(archerAttacks.AttackCooldown());

        shootArrowAction = true;

        if (shootArrowAction)
            archerAttacks.ArrowAttack(transform, player.transform);
    }

    private void ResetActionBooleans()
    {
        jumpBackAction = false;
        shootArrowAction = false;
    }

    protected override void Death()
    {
        base.Death();

        enabled = false;
    }
}
