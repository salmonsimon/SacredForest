using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(WizardAttacks), typeof(DamageReceiver))]
public class Wizard : MonoBehaviour
{
    private Animator animator;

    #region Controllers

    private EnemyMover enemyMover;
    private WizardAttacks wizardAttacks;
    private DamageReceiver damageReceiver;
    private PlayerDetection playerDetection;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider attackZoneCollider;
    [SerializeField] private BoxCollider meleeZoneCollider;
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
        wizardAttacks = GetComponent<WizardAttacks>();
        damageReceiver = GetComponent<DamageReceiver>();
        playerDetection = GetComponent<PlayerDetection>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG);
    }

    private void Start()
    {
        StartCoroutine(Startled(1f));
        StartCoroutine(enemyMover.MovementCooldown(1f));

        damageReceiver.OnCharacterAliveStatusChange += Death;
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
                StartCoroutine(PickRandomAttackPattern());
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

    private IEnumerator PickRandomAttackPattern()
    {
        StartCoroutine(wizardAttacks.AttackCooldown());
        yield return new WaitForSeconds(attackDelay);

        if (meleeZoneCollider.IsColliding())
        {
            wizardAttacks.MeleeAttack(transform.position, player.transform.position);
        }
        else
        {
            wizardAttacks.ProjectileAttack(transform.position, player.transform.position);
        }
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

        GameManager.instance.ShowText("!", 1, Color.white, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Vector3.up * .05f, .5f, transform);
    }

    private IEnumerator Startled()
    {
        isStartled = true;

        yield return new WaitForSeconds(startleDuration);

        isStartled = false;
    }

    private IEnumerator Startled(float duration)
    {
        isStartled = true;

        yield return new WaitForSeconds(duration);

        isStartled = false;
    }

    private void Death()
    {
        isAlive = false;
    }
}
