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
    [SerializeField] private BoxCollider transformedMeleeZoneCollider;

    #endregion

    #region Player

    private GameObject player;
    private bool isPlayerAlive = true;

    #endregion

    #region Logic Variables

    private bool isTransformed = false;

    [SerializeField] private bool onActionCooldown = false;

    private bool onSpecialAttackCooldown = false;
    private float specialAttackCooldownDuration = 5f;

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
        damageReceiver.OnCharacterDamaged += Damaged;
    }

    private void FixedUpdate()
    {
        if (isPlayerAlive)
            isPlayerAlive = player.GetComponent<DamageReceiver>().IsAlive;

        relativePlayerPositionX = player.transform.position.x - transform.position.x;

        if (!isAlive || !isPlayerAlive || isStartled || groundMonkAttacks.IsAttacking())
        {
            enemyMover.StayInPosition();
            return;
        }

        movement = Vector2.zero;

        FlipTowardsPlayer();

        if (meleeZoneCollider.IsColliding() && !isStartled)
        {
            if (!groundMonkAttacks.OnAttackCooldown() && !groundMonkAttacks.IsAttacking() && !isStartled)
            {
                PickRandomAttackPattern();
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
            else if (!isTransformed && !onActionCooldown && !groundMonkAttacks.IsAttacking() && !isStartled)
            {
                groundMonkAttacks.RollAction();
            }
        }
        else if (!groundMonkAttacks.OnAttackCooldown() && !groundMonkAttacks.IsAttacking() && !onSpecialAttackCooldown && !isStartled)
        {
            PickRandomAttackPattern();
            StartCoroutine(ActionCooldown(actionCooldownDuration));
        }
        else if (!groundMonkAttacks.IsAttacking() && !isStartled)
        {
            if (enemyMover.StillMoreToWalk())
            {
                movement = WalksTowards();
            }
        }

        enemyMover.UpdateMotor(movement, false, false);
    }

    private void FlipTowardsPlayer()
    {
        enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));
    }

    private void PickRandomAttackPattern()
    {
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

            if (isAlive && !isStartled)
            {
                StartCoroutine(groundMonkAttacks.AttackCooldown());

                groundMonkAttacks.MeleeAttack(transform.position, player.transform.position, attackPattern, isTransformed);
            }
        }
        else
        {
            if (!onSpecialAttackCooldown)
            {
                if (isAlive && !isStartled)
                {
                    StartCoroutine(groundMonkAttacks.AttackCooldown());
                    StartCoroutine(SpecialAttackCooldown());

                    if (!isTransformed)
                        groundMonkAttacks.ProjectileAttack(transform.position, player.transform.position);
                    else
                        groundMonkAttacks.TransformedSpecialAttack();
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

        enemyMover.StayInPosition();
        yield return new WaitForSeconds(duration);

        isStartled = false;
    }

    private IEnumerator SpecialAttackCooldown()
    {
        onSpecialAttackCooldown = true;

        yield return new WaitForSeconds(specialAttackCooldownDuration);

        onSpecialAttackCooldown = false;
    }

    private void Damaged()
    {
        if (isAlive)
        {
            onActionCooldown = false;
            groundMonkAttacks.ResetIsAttacking();
            enemyMover.StayInPosition();

            CheckDamageToTransform();
        }
    }

    private void CheckDamageToTransform()
    {
        bool isTransforming = false;

        if (!isTransformed)
        {
            if (damageReceiver.CurrentHitPoints <= (float)(damageReceiver.MaxHitPoints / 2f))
            {
                Transform();
                isTransforming = true;
            }
        }

        if (!isTransforming)
        {
            StartCoroutine(Startled(.5f));
            StartCoroutine(damageReceiver.ImmuneCooldown());
        }
            
    }

    public void Transform()
    {
        StartCoroutine(damageReceiver.SetImmune(3.5f));

        StartCoroutine(Startled(3.5f));

        isTransformed = true;
        meleeZoneCollider = transformedMeleeZoneCollider;

        StartCoroutine(WaitAndSetTransform());

        enemyMover.AlterRunSpeed(1f);
    }

    private IEnumerator WaitAndSetTransform()
    {
        yield return new WaitForSeconds(.1f);

        animator.SetBool("IsTransformed", true);
        animator.SetTrigger("Transform");
    }

    private void Death()
    {
        isAlive = false;
    }
}
