using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardAttacks : EnemyAttacks
{
    #region Animation

    [SerializeField] private AnimationClip projectileAnimationClip;
    [SerializeField] private AnimationClip meleeAttackAnimationClip;

    #endregion

    #region Magic Projectile

    [SerializeField] private MagicProjectile magicProjectilePrefab;
    [SerializeField] private float shootingForce = Config.MAGIC_PROJECTILE_SPEED;

    [SerializeField] private float shootingWaitingTime = .5f;

    #endregion

    public void MeleeAttack(Vector3 magePosition, Vector3 playerPosition)
    {
        float xDistance = playerPosition.x - magePosition.x;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
        StartCoroutine(PlayClip(Animator.StringToHash(meleeAttackAnimationClip.name), 0));
        StartCoroutine(IsAttackingCooldown(meleeAttackAnimationClip.length));
    }

    public void ProjectileAttack(Vector3 magePosition, Vector3 playerPosition)
    {

        float xDistance = playerPosition.x - magePosition.x;
        float yDistance = playerPosition.y - magePosition.y;

        Vector2 shootingDirection = new Vector2(xDistance, yDistance).normalized;

        GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
        StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(shootingWaitingTime));
        StartCoroutine(IsAttackingCooldown(shootingWaitingTime + Config.MEDIUM_DELAY));

        StartCoroutine(PlayClip(Animator.StringToHash(projectileAnimationClip.name), 0));
        StartCoroutine(ShootProjectile(shootingDirection));
    }

    private IEnumerator ShootProjectile(Vector2 direction)
    {
        yield return new WaitForSeconds(shootingWaitingTime);

        MagicProjectile newProjectile = Instantiate(magicProjectilePrefab, transform.position + new Vector3(transform.localScale.x * .16f, .1f, 0), Quaternion.identity);

        if (transform.localScale.x < 0)
        {
            newProjectile.transform.localScale = new Vector3(newProjectile.transform.localScale.x * -1f, newProjectile.transform.localScale.y, newProjectile.transform.localScale.z);
        }

        newProjectile.GetComponent<Rigidbody2D>().velocity = direction * shootingForce;
    }
}
