using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttacks : MonoBehaviour
{
    #region Animation

    [SerializeField] private AnimationClip arrowAnimationClip;
    private Animator animator;

    private Coroutine currentAttackCoroutine = null;

    #endregion

    #region Logic Variables

    private bool isAttacking = false;

    private bool onAttackCooldown = false;
    [SerializeField] private float attackCooldownDuration = 3f;

    private bool isAlive = true;

    #endregion

    [SerializeField] private Arrow arrowPrefab;

    private float[] shootingSpeeds = { 7f, 10f, 12f };
    private float shootingSpeed;
    private float shootingAngle;
    private Vector2 shootingDirection;
    private bool isGoingToShoot = false;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GetComponent<DamageReceiver>().OnCharacterDeath += Death;
    }

    public void ArrowAttack(Vector3 archerPosition, Vector3 playerPosition)
    {

        float xDistance = playerPosition.x - archerPosition.x;
        float yDistance = playerPosition.y - archerPosition.y;

        CalculateShot(xDistance, yDistance);

        if (isGoingToShoot)
        {
            isAttacking = true;
            GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
            StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(.3f));

            StartCoroutine(PlayClip(Animator.StringToHash(arrowAnimationClip.name), 0));
            StartCoroutine(ShotArrow(shootingSpeed, shootingDirection, shootingAngle));
        }
    }

    private void CalculateShot(float xDistance, float yDistance) 
    {
        float yGravity = Physics2D.gravity.y;

        foreach (float shootingSpeed in shootingSpeeds)
        {
            float sqrt = (shootingSpeed * shootingSpeed * shootingSpeed * shootingSpeed) -
            (yGravity * (yGravity * (xDistance * xDistance) + 2 * yDistance * (shootingSpeed * shootingSpeed)));

            if (sqrt < 0)
            {
                continue;
            }
            else
            {
                this.shootingSpeed = shootingSpeed;
                shootingAngle = Mathf.Atan(((shootingSpeed * shootingSpeed) - Mathf.Sqrt(sqrt)) / (yGravity * xDistance));
                shootingDirection = new Vector2(Mathf.Sign(xDistance) * Mathf.Cos(shootingAngle), -Mathf.Sign(xDistance) * Mathf.Sin(shootingAngle));

                isGoingToShoot = true;
            }
        }
    }

    private IEnumerator ShotArrow(float shootingForce, Vector2 direction, float angle)
    {
        yield return new WaitForSeconds(.3f);

        Arrow newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(transform.localScale.x * .16f, 0, 0), Quaternion.identity);

        if (transform.localScale.x < 0)
        {
            newArrow.transform.localScale = new Vector3(newArrow.transform.localScale.x * -1f, newArrow.transform.localScale.y, newArrow.transform.localScale.z);
        }

        newArrow.GetComponent<Rigidbody2D>().velocity = direction * shootingForce;

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
