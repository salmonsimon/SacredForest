using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(ArcherAttacks), typeof(DamageReceiver))]
public class Archer : MonoBehaviour
{
    private Animator animator;

    #region Controllers

    private EnemyMover enemyMover;
    private ArcherAttacks archerAttacks;
    private DamageReceiver damageReceiver;
    private PlayerDetection playerDetection;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider attackZoneCollider;
    [SerializeField] private BoxCollider dangerZoneCollider;

    #endregion

    #region Player

    private GameObject player;
    private bool isPlayerAlive = true;

    #endregion

    #region Logic Variables

    private bool onActionCooldown = false;

    private bool isStartled = false;
    private bool isAlive = true;

    #endregion

    #region Action Variables

    Vector2 movement = Vector2.zero;
    float relativePlayerPositionX = 0;

    private bool shootArrowAction = false;

    private bool jumpBackAction = false;

    #endregion

    #region Parameters

    [SerializeField] private float startleDuration = .5f;
    private float actionCooldownDuration = Config.ACTION_COOLDOWN_DURATION;
    private float attackDelay = 0f;

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();

        enemyMover = GetComponent<EnemyMover>();
        archerAttacks = GetComponent<ArcherAttacks>();
        damageReceiver = GetComponent<DamageReceiver>();
        playerDetection = GetComponent<PlayerDetection>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG);
    }

    private void Start()
    {
        damageReceiver.OnCharacterDeath += Death;
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
            if (!archerAttacks.OnAttackCooldown())
            {
                StartCoroutine(PickRandomAttackPattern());
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

        if (jumpBackAction && archerAttacks.AttackCoroutineOnCourse())
            archerAttacks.StopCurrentAttackCoroutine();

        enemyMover.UpdateMotor(movement, false, jumpBackAction);

        ResetActionBooleans();
    }

    // This one will only have the arrow attack for now
    // but could use it if we find other character with dagger attacks
    // to defend when player in close range
    private IEnumerator PickRandomAttackPattern()
    {
        StartCoroutine(archerAttacks.AttackCooldown());
        yield return new WaitForSeconds(attackDelay);

        shootArrowAction = true;

        if (shootArrowAction)
            archerAttacks.ArrowAttack(transform.position, player.transform.position);
    }

    private Vector2 WalksTowards()
    {
        Vector2 movement;

        if (relativePlayerPositionX > 0)
        {
            movement = Vector2.right;
        }
        else
        {
            movement = Vector2.left;
        }

        return movement;
    }

    private Vector2 WalksAway()
    {
        Vector2 movement;

        if (relativePlayerPositionX > 0)
        {
            movement = Vector2.left;
        }
        else
        {
            movement = Vector2.right;
        }

        return movement;
    }

    private void ResetActionBooleans()
    {
        jumpBackAction = false;
        shootArrowAction = false;
    }

    private IEnumerator ActionCooldown(float duration)
    {
        onActionCooldown = true;

        yield return new WaitForSeconds(duration);

        onActionCooldown = false;
    }

    private void PlayerDetected()
    {
        enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));
        StartCoroutine(playerDetection.AlertGroupAfterDetectingPlayer());

        StartCoroutine(Startled());
        StartCoroutine(enemyMover.MovementCooldown(startleDuration));

        GameManager.instance.ShowText("!", 1, Color.white, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Vector3.up * .05f, .5f);
    }

    private IEnumerator Startled()
    {
        isStartled = true;

        yield return new WaitForSeconds(startleDuration);

        isStartled = false;
    }

    private void Death()
    {
        isAlive = false;
    }
}
