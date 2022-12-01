using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMonkAttacks : EnemyAttacks
{
    #region Animation

    [SerializeField] private AnimationClip rollAnimationClip;

    [SerializeField] private AnimationClip projectileAnimationClip;

    [SerializeField] private AnimationClip[] meleeAttackAnimationClips;
    [SerializeField] private AnimationClip[] transformedMeleeAttackAnimationClips;

    [SerializeField] private AnimationClip transformedSpecialAttack;

    #endregion

    #region Projectile

    [SerializeField] private EarthenHands projectilePrefab;
    [SerializeField] private float shootingWaitingTime = .4f;

    #endregion

    private Coroutine isAttackingCooldownCoroutine = null;

    private GameObject projectileContainer;

    protected override void Start()
    {
        base.Start();

        projectileContainer = GameObject.FindGameObjectWithTag(Config.PROJECTILE_CONTAINER_NAME);
    }

    public void RollAction()
    {
        isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(Config.BIG_DELAY));
        StartCoroutine(GetComponent<DamageReceiver>().SetImmune(Config.BIG_DELAY));
        StartCoroutine(ToDashLayerCooldown(Config.BIG_DELAY));

        StartCoroutine(PlayClip(Animator.StringToHash(rollAnimationClip.name), 0));
    }

    private IEnumerator ToDashLayerCooldown(float duration)
    {
        gameObject.layer = LayerMask.NameToLayer(Config.DASH_LAYER);
        InvokeRepeating("MovementForRoll", 0, .01f);

        yield return new WaitForSeconds(duration);

        gameObject.layer = LayerMask.NameToLayer(Config.ENEMY_LAYER);
        CancelInvoke();

        GetComponent<EnemyMover>().StayInPosition();
    }

    private void MovementForRoll()
    {
        EnemyMover enemyMover = GetComponent<EnemyMover>();

        Vector2 direction = enemyMover.GetDirection();

        enemyMover.UpdateMotor(direction * 10f, false, false);
    }

    public void MeleeAttack(Vector3 position, Vector3 playerPosition, int attackPattern, bool isTransformed)
    {
        float xDistance = playerPosition.x - position.x;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));

        AnimationClip attackAnimationClip;

        if (isTransformed)
        {
            attackAnimationClip = transformedMeleeAttackAnimationClips[attackPattern];

            switch (attackPattern)
            {
                case 0:
                    isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(.75f));
                    break;
                case 1:
                    isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(1.2f));
                    break;
                case 2:
                    isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(2f));
                    break;
            }
        }
        else
        {
            attackAnimationClip = meleeAttackAnimationClips[attackPattern];

            switch (attackPattern)
            {
                case 0:
                    isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(.5f));
                    break;
                case 1:
                    isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(1.1f));
                    break;
                case 2:
                    isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(2.2f));
                    break;
            }
        }

        StartCoroutine(PlayClip(Animator.StringToHash(attackAnimationClip.name), 0));
    }

    public void ProjectileAttack(Vector3 magePosition, Vector3 playerPosition)
    {

        float xDistance = playerPosition.x - magePosition.x;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
        StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(2.4f));
        isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(3f));

        StartCoroutine(PlayClip(Animator.StringToHash(projectileAnimationClip.name), 0));
        StartCoroutine(ShootProjectile(playerPosition));
    }

    private IEnumerator ShootProjectile(Vector3 playerPosition)
    {
        yield return new WaitForSeconds(shootingWaitingTime);

        if (!GetComponent<GroundMonk>().IsTransforming)
        {
            EarthenHands newEarthenHands = Instantiate(projectilePrefab, new Vector3(playerPosition.x, transform.position.y - .02f, transform.position.z), Quaternion.identity);
            newEarthenHands.transform.parent = projectileContainer.transform;
            newEarthenHands.SetLookat(GameManager.instance.GetPlayer().transform);

            GameManager.instance.GetCinemachineShake().ShakeCamera(.5f, 2f);
        }
    }

    public void TransformedSpecialAttack()
    {
        StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(1.2f));
        isAttackingCooldownCoroutine = StartCoroutine(IsAttackingCooldown(1.2f));

        StartCoroutine(PlayClip(Animator.StringToHash(transformedSpecialAttack.name), 0));
    }

    protected override IEnumerator IsAttackingCooldown(float duration)
    {
        if (isAttackingCooldownCoroutine != null)
            StopCoroutine(isAttackingCooldownCoroutine);

        return base.IsAttackingCooldown(duration);
    }

    public void StopActions()
    {
        StopAllCoroutines();
        CancelInvoke();

        gameObject.layer = LayerMask.NameToLayer(Config.ENEMY_LAYER);
        StartCoroutine(GetComponent<DamageReceiver>().SetImmune(0));

        isAttacking = false;
        onAttackCooldown = false;
    }

    public void ClearProjectiles()
    {
        foreach (Transform child in projectileContainer.transform)
        {
            StartCoroutine(child.gameObject.GetComponent<EarthenHands>().WaitAndDestroy(0));
        }
    }
}
