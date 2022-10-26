using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(EnemyAttacks), typeof(DamageReceiver))]
public class Enemy : MonoBehaviour
{
    protected Animator animator;

    #region Controllers

    protected EnemyMover enemyMover;
    protected EnemyAttacks enemyAttacks;
    protected DamageReceiver damageReceiver;

    #endregion

    #region Player

    protected GameObject player;
    protected bool isPlayerAlive = true;

    #endregion

    #region Logic Variables

    protected bool onActionCooldown = false;

    protected bool isStartled = false;
    protected bool isAlive = true;

    #endregion

    #region Action Variables

    protected Vector2 movement = Vector2.zero;
    protected float relativePlayerPositionX = 0;

    #endregion

    #region Parameters

    protected float actionCooldownDuration = Config.ACTION_COOLDOWN_DURATION;

    #endregion

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        enemyMover = GetComponent<EnemyMover>();
        enemyAttacks = GetComponent<EnemyAttacks>();
        damageReceiver = GetComponent<DamageReceiver>();

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG);
    }

    protected virtual void Start()
    {
        StartCoroutine(Startled(1f));

        damageReceiver.OnCharacterAliveStatusChange += Death;
        damageReceiver.OnCharacterDamaged += Damaged;
    }

    protected void FlipTowardsPlayer()
    {
        enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));
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

    protected IEnumerator Startled(float duration)
    {
        isStartled = true;

        enemyMover.StayInPosition();
        yield return new WaitForSeconds(duration);

        isStartled = false;
    }

    protected virtual void Damaged()
    {
        if (isAlive)
        {
            onActionCooldown = false;
            enemyMover.StayInPosition();
            enemyAttacks.ResetIsAttacking();

            StartCoroutine(Startled(.5f));
            StartCoroutine(damageReceiver.ImmuneCooldown());
        }
    }

    protected virtual void Death()
    {
        isAlive = false;
    }
}
