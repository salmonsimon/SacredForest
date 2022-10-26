using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WizardAttacks), typeof(PlayerDetection))]
public class Wizard : Enemy
{
    #region Controllers

    private WizardAttacks wizardAttacks;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider attackZoneCollider;
    [SerializeField] private BoxCollider meleeZoneCollider;
    [SerializeField] private BoxCollider dangerZoneCollider;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        wizardAttacks = GetComponent<WizardAttacks>();
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
            enemyMover.UpdateMotor(Vector2.zero, false, false);
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
            if (!wizardAttacks.OnAttackCooldown())
            {
                PickRandomAttackPattern();
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
            else if (enemyMover.IsWalkingAway() && enemyMover.StillMoreToWalk() && !wizardAttacks.IsAttacking())
            {
                movement = WalksAway();
            }
            else if (!onActionCooldown && !wizardAttacks.IsAttacking() && dangerZoneCollider.IsColliding())
            {
                enemyMover.PickNonAttackAction(enemyMover.IsGroundedAfterJumpBack(), 0, 5);
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
        }
        else if (playerDetection.DetectedPlayer && (!wizardAttacks.IsAttacking() || enemyMover.MovesWhileAttacking()))
        {
            if (enemyMover.StillMoreToWalk())
            {
                if (enemyMover.IsWalkingAway())
                {
                    movement = WalksAway();
                }
            }
        }

        enemyMover.UpdateMotor(movement, false, false);
    }

    private void PickRandomAttackPattern()
    {
        StartCoroutine(wizardAttacks.AttackCooldown());

        if (meleeZoneCollider.IsColliding())
        {
            wizardAttacks.MeleeAttack(transform.position, player.transform.position);
        }
        else
        {
            wizardAttacks.ProjectileAttack(transform.position, player.transform.position);
        }
    }
}
