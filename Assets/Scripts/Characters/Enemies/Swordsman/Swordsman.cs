using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyMover), typeof(SwordsmanAttacks), typeof(DamageReceiver))]
public class Swordsman : MonoBehaviour
{
    private Animator animator;

    #region Controllers

    private EnemyMover enemyMover;
    private SwordsmanAttacks swordsmanAttacks;
    private DamageReceiver damageReceiver;
    private PlayerDetection playerDetection;

    #endregion

    #region Check Colliders

    [SerializeField] private BoxCollider attackZoneCollider;
    [SerializeField] private CircleCollider stillMoreToWalkCheck;
    [SerializeField] private CircleCollider jumpBackGroundCheck;

    [SerializeField] private BoxCollider2D visionFieldCollider;
    [SerializeField] private BoxCollider2D groupAlertCollider;


    #endregion

    #region Player

    [SerializeField] private LayerMask playerLayer;
    private GameObject player;
    private bool isPlayerAlive = true;

    #endregion

    #region Logic Variables

    private bool groundedAfterJumpBack;
    private bool stillMoreToWalk;

    private bool onActionCooldown = false;

    private bool isStartled = false;
    private bool isAlive = true;

    [SerializeField] private LayerMask enemyLayer;
    private RaycastHit2D[] enemyGroup;

    #endregion

    #region Action Variables

    Vector2 movement = Vector2.zero;
    float relativePlayerPositionX = 0;

    private bool firstAttackAction = false;
    private bool secondAttackAction = false;
    private bool thirdAttackAction = false;

    private bool jumpBackAction = false;

    #endregion

    #region Parameters

    [SerializeField] private float startleDuration = .5f;
    private float actionCooldownDuration = Config.ACTION_COOLDOWN_DURATION;
    private float alertGroupDelay = Config.ALERT_GROUP_DELAY;
    private float attackDelay = Config.ATTACK_DELAY;
    

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();

        enemyMover = GetComponent<EnemyMover>();
        swordsmanAttacks = GetComponent<SwordsmanAttacks>();
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

        groundedAfterJumpBack = jumpBackGroundCheck.IsColliding();
        stillMoreToWalk = stillMoreToWalkCheck.IsColliding();

        relativePlayerPositionX = player.transform.position.x - transform.position.x;

        if (!enemyMover.IsWalkingAway() && playerDetection.DetectedPlayer)
            enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));

        if (!playerDetection.DetectedPlayer)
            CheckForPlayer();

        if (attackZoneCollider.IsColliding())
        {
            if (!playerDetection.DetectedPlayer)
            {
                playerDetection.DetectedPlayer = true;
                return;
            }

            if (!swordsmanAttacks.OnAttackCooldown())
            {
                StartCoroutine(PickRandomAttackPattern(swordsmanAttacks.HasSecondAttack(), swordsmanAttacks.HasThirdAttack(), enemyMover.IsAbleToJumpBack()));
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
            else if (enemyMover.IsWalkingAway())
            {
                movement = WalksAway();
            }
            else if (!onActionCooldown && !swordsmanAttacks.IsAttacking())
            {
                jumpBackAction = enemyMover.PickNonAttackAction(groundedAfterJumpBack);
                StartCoroutine(ActionCooldown(actionCooldownDuration));
            }
        }
        else if (playerDetection.DetectedPlayer && (!swordsmanAttacks.IsAttacking() || enemyMover.MovesWhileAttacking()))
        {
            if (stillMoreToWalk)
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

    private IEnumerator PickRandomAttackPattern(bool hasSecondAttack, bool hasThirdAttack, bool isAbleToJumpBack)
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

    private void CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(visionFieldCollider.bounds.center, visionFieldCollider.bounds.size, 0, Vector2.left, 0, playerLayer);

        if (hit.collider)
            playerDetection.DetectedPlayer = true;
    }

    private void PlayerDetected()
    {
        enemyMover.Flip(new Vector2(relativePlayerPositionX, 0));
        StartCoroutine(AlertGroupAfterDetectingPlayer());

        StartCoroutine(Startled());
        StartCoroutine(enemyMover.MovementCooldown(startleDuration));

        GameManager.instance.ShowText("!", 32, Color.white, new Vector3(transform.position.x, transform.position.y + 0.32f, 0), Vector3.up * 40, 1f);
    }

    private IEnumerator AlertGroupAfterDetectingPlayer()
    {
        yield return new WaitForSeconds(alertGroupDelay);

        enemyGroup = Physics2D.BoxCastAll(groupAlertCollider.bounds.center, groupAlertCollider.bounds.size, 0, Vector2.left, 0, enemyLayer);

        if (enemyGroup != null)
        {
            foreach (RaycastHit2D enemy in enemyGroup)
            {
                enemy.transform.gameObject.GetComponent<PlayerDetection>().DetectedPlayer = true;
            }
        }
    }

    private IEnumerator Startled()
    {
        isStartled = true;

        yield return new WaitForSeconds(startleDuration);

        isStartled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(visionFieldCollider.bounds.center, visionFieldCollider.bounds.size);
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
        firstAttackAction = false;
        secondAttackAction = false;
        thirdAttackAction = false;
    }

    private IEnumerator ActionCooldown(float duration)
    {
        onActionCooldown = true;

        yield return new WaitForSeconds(duration);

        onActionCooldown = false;
    }

    private void Death()
    {
        isAlive = false;
    }
}
