using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMonkAttacks : MonoBehaviour
{
    #region Animation

    [SerializeField] private AnimationClip projectileAnimationClip;

    [SerializeField] private AnimationClip[] meleeAttackAnimationClips;
    [SerializeField] private AnimationClip[] transformedMeleeAttackAnimationClips;
    private Animator animator;

    #endregion

    #region Logic Variables

    [SerializeField] private bool isAttacking = false;

    private bool onAttackCooldown = false;
    [SerializeField] private float attackCooldownDuration = 2f;

    private bool isAlive = true;

    #endregion

    #region Projectile

    [SerializeField] private EarthenHands projectilePrefab;
    [SerializeField] private float shootingWaitingTime = .4f;

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += Death;
    }

    public void MeleeAttack(Vector3 position, Vector3 playerPosition, int attackPattern, bool isTransformed)
    {
        float xDistance = playerPosition.x - position.x;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));

        AnimationClip attackAnimationClip;

        if (isTransformed)
        {
            attackAnimationClip = transformedMeleeAttackAnimationClips[attackPattern];
        }
        else
        {
            attackAnimationClip = meleeAttackAnimationClips[attackPattern];
        }

        StartCoroutine(PlayClip(Animator.StringToHash(attackAnimationClip.name), 0));
        StartCoroutine(IsAttackingCooldown(1f));
    }

    public void ProjectileAttack(Vector3 magePosition, Vector3 playerPosition)
    {

        float xDistance = playerPosition.x - magePosition.x;

        isAttacking = true;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
        StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(2.4f));
        StartCoroutine(IsAttackingCooldown(2.6f));

        StartCoroutine(PlayClip(Animator.StringToHash(projectileAnimationClip.name), 0));
        StartCoroutine(ShootProjectile(playerPosition));
    }

    private IEnumerator ShootProjectile(Vector3 playerPosition)
    {
        yield return new WaitForSeconds(shootingWaitingTime);

        EarthenHands newEarthenHands = Instantiate(projectilePrefab, new Vector3( playerPosition.x, transform.position.y - .13f, transform.position.z), Quaternion.identity);
        newEarthenHands.SetLookat(GameManager.instance.GetPlayer().transform);

        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }

    private IEnumerator PlayClip(int clipHash, float startTime)
    {
        yield return new WaitForSeconds(startTime);

        if (isAlive)
        {
            animator.Play(clipHash);
        }
    }

    private IEnumerator IsAttackingCooldown(float duration)
    {
        isAttacking = true;

        yield return new WaitForSeconds(duration);

        isAttacking = false;
    }

    public IEnumerator AttackCooldown()
    {
        onAttackCooldown = true;

        yield return new WaitForSeconds(attackCooldownDuration);

        onAttackCooldown = false;
    }

    public bool OnAttackCooldown()
    {
        return onAttackCooldown;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
    private void Death()
    {
        isAlive = false;
    }
}
