using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(GroundMonkAttacks), typeof(DamageReceiver))]
public class GroundMonk : MonoBehaviour
{
    private Animator animator;

    #region Controllers

    private EnemyMover enemyMover;
    private GroundMonkAttacks groundMonkAttacks;
    private DamageReceiver damageReceiver;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider meleeZoneCollider;

    #endregion

    #region Player

    private GameObject player;
    private bool isPlayerAlive = true;

    #endregion

    #region Logic Variables

    private bool isTransformed = false;

    private bool onActionCooldown = false;

    private bool onSpecialAttackCooldown = false;
    private float specialAttackCooldownDuration = 3f;

    private bool isStartled = false;
    private bool isAlive = true;

    #endregion

    #region Action Variables

    Vector2 movement = Vector2.zero;
    float relativePlayerPositionX = 0;

    #endregion

    #region Parameters

    private float actionCooldownDuration = Config.ACTION_COOLDOWN_DURATION;

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();

        enemyMover = GetComponent<EnemyMover>();
        groundMonkAttacks = GetComponent<GroundMonkAttacks>();
        damageReceiver = GetComponent<DamageReceiver>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG);
    }

    private void Start()
    {
        StartCoroutine(Startled(1f));
        StartCoroutine(enemyMover.MovementCooldown(1f));

        damageReceiver.OnCharacterAliveStatusChange += Death;
    }

    private void FixedUpdate()
    {
        if (isPlayerAlive)
            isPlayerAlive = player.GetComponent<DamageReceiver>().IsAlive;

        if (!isAlive || !isPlayerAlive || isStartled || groundMonkAttacks.IsAttacking())
        {
            enemyMover.UpdateMotor(Vector2.zero, false, false);
            return;
        }

        movement = Vector2.zero;

        relativePlayerPositionX = player.transform.position.x - transform.position.x;

        if (!enemyMover.IsWalkingAway())
            enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));

        if (!groundMonkAttacks.OnAttackCooldown())
        {
            PickRandomAttackPattern();
            StartCoroutine(ActionCooldown(actionCooldownDuration));
        }
        else if (enemyMover.IsWalkingAway() && enemyMover.StillMoreToWalk() && !groundMonkAttacks.IsAttacking())
        {
            movement = WalksAway();
        }
        else if (!onActionCooldown && !groundMonkAttacks.IsAttacking() && meleeZoneCollider.IsColliding())
        {
            //do roll action
            StartCoroutine(ActionCooldown(actionCooldownDuration));
        }

        else if (!groundMonkAttacks.IsAttacking())
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

        enemyMover.UpdateMotor(movement, false, false);
    }

    private void PickRandomAttackPattern()
    {
        StartCoroutine(groundMonkAttacks.AttackCooldown());

        if (meleeZoneCollider.IsColliding())
        {
            float randomPercentage = Random.Range(0f, 1f);
            int attackPattern = 0;

            if (randomPercentage > .15f && randomPercentage <= .7f)
            {
                attackPattern = 1;
            }
            else if (randomPercentage > .7f)
            {
                attackPattern = 2;
            }

            if (isAlive)
                groundMonkAttacks.MeleeAttack(transform.position, player.transform.position, attackPattern, isTransformed);
        }
        else
        {
            if (!onSpecialAttackCooldown)
            {
                if (isAlive)
                {
                    groundMonkAttacks.ProjectileAttack(transform.position, player.transform.position);
                    StartCoroutine(SpecialAttackCooldown());
                }
            }
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

    private IEnumerator ActionCooldown(float duration)
    {
        onActionCooldown = true;

        yield return new WaitForSeconds(duration);

        onActionCooldown = false;
    }

    private IEnumerator Startled(float duration)
    {
        isStartled = true;

        yield return new WaitForSeconds(duration);

        isStartled = false;
    }

    private IEnumerator SpecialAttackCooldown()
    {
        onSpecialAttackCooldown = true;

        yield return new WaitForSeconds(specialAttackCooldownDuration);

        onSpecialAttackCooldown = false;
    }

    private void Death()
    {
        isAlive = false;
    }
}
