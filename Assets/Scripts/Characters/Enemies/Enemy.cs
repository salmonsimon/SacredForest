using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover), typeof(DamageReceiver))]
public class Enemy : MonoBehaviour
{
    protected Animator animator;

    #region Controllers

    protected EnemyMover enemyMover;
    protected DamageReceiver damageReceiver;
    protected PlayerDetection playerDetection;

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
    [SerializeField] protected float startleDuration = .5f;

    #endregion

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();

        enemyMover = GetComponent<EnemyMover>();
        damageReceiver = GetComponent<DamageReceiver>();

        if (TryGetComponent(out PlayerDetection playerDetection)) {
            this.playerDetection = playerDetection;
        }

        player = GameObject.FindGameObjectWithTag(Config.PLAYER_TAG);
    }

    protected virtual void Start()
    {
        StartCoroutine(Startled(1f));

        damageReceiver.OnCharacterAliveStatusChange += Death;
        damageReceiver.OnCharacterDamaged += Damaged;
    }

    protected void PlayerDetected()
    {
        enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));
        StartCoroutine(playerDetection.AlertGroupAfterDetectingPlayer());

        StartCoroutine(Startled(startleDuration));

        GameManager.instance.ShowText("!", 1, Color.white, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Vector3.up * .05f, Config.STUN_DURATION, transform);
    }

    protected void FlipTowardsPlayer()
    {
        enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));
    }

    protected Vector2 WalksAway()
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

    protected Vector2 WalksTowards()
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

    protected IEnumerator ActionCooldown(float duration)
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
            
            // see where we put this
            //enemyAttacks.ResetIsAttacking();

            StartCoroutine(Startled(Config.STUN_DURATION));
            StartCoroutine(damageReceiver.ImmuneCooldown());
        }
    }

    protected virtual void Death()
    {
        isAlive = false;
    }
}
