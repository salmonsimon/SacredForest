using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundMonkAttacks))]
public class GroundMonk : Elemental
{
    #region Controllers

    private GroundMonkAttacks groundMonkAttacks;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider meleeZoneCollider;
    [SerializeField] private BoxCollider transformedMeleeZoneCollider;

    #endregion

    #region Logic Variables

    private bool onSpecialAttackCooldown = false;
    private float specialAttackCooldownDuration = 5f;

    #endregion

    protected override void Awake()
    {
        base.Awake();

        groundMonkAttacks = GetComponent<GroundMonkAttacks>();
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

    private IEnumerator SpecialAttackCooldown()
    {
        onSpecialAttackCooldown = true;

        yield return new WaitForSeconds(specialAttackCooldownDuration);

        onSpecialAttackCooldown = false;
    }

    public override void Transform()
    {
        if (GameManager.instance.GetCurrentProgressManager().FinishedRoute1 || 
            GameManager.instance.GetCurrentProgressManager().Route1BossHasTransformed)
        {
            base.Transform();

            meleeZoneCollider = transformedMeleeZoneCollider;
            enemyMover.AlterRunSpeed(1f);
        }
        else
        {
            StartCoroutine(damageReceiver.SetImmune(Config.STUN_DURATION));
            isTransformed = true;

            enabled = false;

            GameObject cutscene = GameObject.FindGameObjectWithTag("Cutscene");
            StartCoroutine(cutscene.GetComponent<Cutscene04>().PlayTransformScene());
        }
    }

    public void PlayMonsterStepSound()
    {
        GameManager.instance.GetSFXManager().PlayMonsterStepSound();
        SmallCameraShake();
    }

    public void SmallCameraShake()
    {
        GameManager.instance.GetCinemachineShake().ShakeCamera(2f, Config.SMALL_DELAY);
    }

    public void TransformationCameraShake()
    {
        GameManager.instance.GetCinemachineShake().ShakeCamera(3f, transformationDuration);
    }

    protected override void Death()
    {
        base.Death();

        if (!GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
        {
            GameObject cutscene = GameObject.FindGameObjectWithTag("Cutscene");
            StartCoroutine(cutscene.GetComponent<Cutscene04>().PlayDeathScene());
        }
    }
}
