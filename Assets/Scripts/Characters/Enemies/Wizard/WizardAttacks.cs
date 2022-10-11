using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAttacks : MonoBehaviour
{
    #region Animation

    [SerializeField] private AnimationClip projectileAnimationClip;
    [SerializeField] private AnimationClip meleeAttackAnimationClip;
    private Animator animator;

    private Coroutine currentAttackCoroutine = null;

    #endregion

    #region Logic Variables

    private bool isAttacking = false;

    private bool onAttackCooldown = false;
    [SerializeField] private float attackCooldownDuration = 3f;

    private bool isAlive = true;

    #endregion

    [SerializeField] private MagicProjectile magicProjectilePrefab;
    [SerializeField] private float shootingForce = Config.MAGIC_PROJECTILE_SPEED;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GetComponent<DamageReceiver>().OnCharacterDeath += Death;
    }

    public void MeleeAttack(Vector3 magePosition, Vector3 playerPosition)
    {
        float xDistance = playerPosition.x - magePosition.x;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
        StartCoroutine(PlayClip(Animator.StringToHash(meleeAttackAnimationClip.name), 0));
    }

    public void ProjectileAttack(Vector3 magePosition, Vector3 playerPosition)
    {

        float xDistance = playerPosition.x - magePosition.x;
        float yDistance = playerPosition.y - magePosition.y;

        Vector2 shootingDirection = new Vector2(xDistance, yDistance);

        isAttacking = true;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
        StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(.5f));

        StartCoroutine(PlayClip(Animator.StringToHash(projectileAnimationClip.name), 0));
        StartCoroutine(ShootProjectile(shootingDirection));
    }

    private IEnumerator ShootProjectile(Vector2 direction)
    {
        yield return new WaitForSeconds(.5f);

        MagicProjectile newProjectile = Instantiate(magicProjectilePrefab, transform.position + new Vector3(transform.localScale.x * .16f, 0, 0), Quaternion.identity);

        if (transform.localScale.x < 0)
        {
            newProjectile.transform.localScale = new Vector3(newProjectile.transform.localScale.x * -1f, newProjectile.transform.localScale.y, newProjectile.transform.localScale.z);
        }

        newProjectile.GetComponent<Rigidbody2D>().velocity = direction * shootingForce;

        yield return new WaitForSeconds(.2f);
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

    public bool AttackCoroutineOnCourse()
    {
        return currentAttackCoroutine != null;
    }

    public void StopCurrentAttackCoroutine()
    {
        StopCoroutine(currentAttackCoroutine);
    }

    private void Death()
    {
        isAlive = false;

        if (currentAttackCoroutine != null)
            StopCurrentAttackCoroutine();
    }
}
