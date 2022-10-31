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

    public void RollAction()
    {
        StartCoroutine(IsAttackingCooldown(Config.BIG_DELAY));
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
                    StartCoroutine(IsAttackingCooldown(.6f));
                    break;
                case 1:
                    StartCoroutine(IsAttackingCooldown(.9f));
                    break;
                case 2:
                    StartCoroutine(IsAttackingCooldown(1.4f));
                    break;
            }
        }
        else
        {
            attackAnimationClip = meleeAttackAnimationClips[attackPattern];

            switch (attackPattern)
            {
                case 0:
                    StartCoroutine(IsAttackingCooldown(.5f));
                    break;
                case 1:
                    StartCoroutine(IsAttackingCooldown(1.1f));
                    break;
                case 2:
                    StartCoroutine(IsAttackingCooldown(2.2f));
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
        StartCoroutine(IsAttackingCooldown(3f));

        StartCoroutine(PlayClip(Animator.StringToHash(projectileAnimationClip.name), 0));
        StartCoroutine(ShootProjectile(playerPosition));
    }

    private IEnumerator ShootProjectile(Vector3 playerPosition)
    {
        yield return new WaitForSeconds(shootingWaitingTime);

        EarthenHands newEarthenHands = Instantiate(projectilePrefab, new Vector3( playerPosition.x, transform.position.y - .02f, transform.position.z), Quaternion.identity);
        newEarthenHands.SetLookat(GameManager.instance.GetPlayer().transform);

        GameManager.instance.GetCinemachineShake().ShakeCamera(.5f, 2f);
    }

    public void TransformedSpecialAttack()
    {
        StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(1.2f));
        StartCoroutine(IsAttackingCooldown(1.2f));

        StartCoroutine(PlayClip(Animator.StringToHash(transformedSpecialAttack.name), 0));
    }
}
