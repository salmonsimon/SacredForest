using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SwordsmanAttacks), typeof(PlayerDetection))]
public class Swordsman : Enemy
{
    #region Controllers

    private SwordsmanAttacks swordsmanAttacks;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider attackZoneCollider;

    #endregion

    #region Action Variables

    private bool firstAttackAction = false;
    private bool secondAttackAction = false;
    private bool thirdAttackAction = false;

    private bool jumpBackAction = false;

    #endregion

    #region Parameters

    [SerializeField] private float attackDelay = Config.ATTACK_DELAY;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        swordsmanAttacks = GetComponent<SwordsmanAttacks>();
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

        if (attackZoneCollider.IsColliding())
        {
            if (!playerDetection.DetectedPlayer)
            {
                playerDetection.DetectedPlayer = true;
                return;
            }

            if (!swordsmanAttacks.OnAttackCooldown())
            {
                StartCoroutine(PickRandomAttackPattern(swordsmanAttacks.HasSecondAttack(), swordsmanAttacks.HasThirdAttack()));
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
            else if (enemyMover.IsWalkingAway())
            {
                movement = WalksAway();
            }
            else if (!onActionCooldown && !swordsmanAttacks.IsAttacking())
            {
                jumpBackAction = enemyMover.PickNonAttackAction(enemyMover.IsGroundedAfterJumpBack());
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
        }
        else if (playerDetection.DetectedPlayer && (!swordsmanAttacks.IsAttacking() || enemyMover.MovesWhileAttacking()))
        {
            if (enemyMover.StillMoreToWalk())
            {
                if (enemyMover.IsWalkingAway())
                {
                    movement = WalksAway();
                }
                else
                {
                    movement = WalksTowards();
                }
            }
        }

        if (jumpBackAction)
            swordsmanAttacks.StopCurrentAttackCoroutine();

        enemyMover.UpdateMotor(movement, false, jumpBackAction);

        ResetActionBooleans();
    }

    private IEnumerator PickRandomAttackPattern(bool hasSecondAttack, bool hasThirdAttack)
    {
        StartCoroutine(swordsmanAttacks.AttackCooldown());
        yield return new WaitForSeconds(attackDelay);

        firstAttackAction = true;

        if (hasThirdAttack)
        {
            float randomPercentage = Random.Range(0f, 1f);

            if (randomPercentage > .15f && randomPercentage <= .5f)
            {
                secondAttackAction = true;
            }
            else
            {
                secondAttackAction = true;
                thirdAttackAction = true;
            }
        }
        else if (hasSecondAttack)
        {
            if (Random.Range(0f,1f) <= .75f)
            {
                secondAttackAction = true;
            }
        }

        if (isAlive)
            swordsmanAttacks.AttackPattern(firstAttackAction, secondAttackAction, thirdAttackAction);
    }

    private void ResetActionBooleans()
    {
        jumpBackAction = false;
        firstAttackAction = false;
        secondAttackAction = false;
        thirdAttackAction = false;
    }
}
