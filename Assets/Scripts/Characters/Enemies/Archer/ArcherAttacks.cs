using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAttacks : EnemyAttacks
{
    #region Animation

    [SerializeField] private AnimationClip arrowAnimationClip;

    #endregion

    #region Arrow - Projectile

    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private float shootingWaitingTime = .3f;

    private float[] shootingSpeeds = { 10f, 7f };
    private float shootingSpeed;
    private float shootingAngle;
    private Vector2 shootingDirection;
    private bool isGoingToShoot = false;

    private GameObject projectileContainer;

    #endregion

    protected override void Start()
    {
        base.Start();

        projectileContainer = GameObject.FindGameObjectWithTag("Projectile Container");
    }

    public void ArrowAttack(Vector3 archerPosition, Vector3 playerPosition)
    {

        float xDistance = playerPosition.x - archerPosition.x;
        float yDistance = playerPosition.y - archerPosition.y;

        CalculateShot(xDistance, yDistance);

        if (isGoingToShoot)
        {
            GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
            StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(shootingWaitingTime));
            StartCoroutine(IsAttackingCooldown(shootingWaitingTime + .2f));

            StartCoroutine(PlayClip(Animator.StringToHash(arrowAnimationClip.name), 0));
            StartCoroutine(ShootArrow(shootingSpeed, shootingDirection));
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

                if (Mathf.Abs(yDistance) < .5)
                    shootingAngle = Mathf.Atan(((shootingSpeed * shootingSpeed) - Mathf.Sqrt(sqrt)) / (yGravity * xDistance));
                else
                {
                    shootingAngle = Mathf.Atan(((shootingSpeed * shootingSpeed) + Mathf.Sqrt(sqrt)) / (yGravity * xDistance));

                    float shootingSpeedCorrection = CalculateShootingSpeedCorrection(yDistance);

                    if (yDistance > 0)
                    {
                        this.shootingSpeed += shootingSpeedCorrection;
                    }
                    else
                    {
                        this.shootingSpeed -= shootingSpeedCorrection;
                    }
                }

                shootingDirection = new Vector2(Mathf.Sign(xDistance) * Mathf.Cos(shootingAngle), -Mathf.Sign(xDistance) * Mathf.Sin(shootingAngle));

                isGoingToShoot = true;
            }
        }
    }

    private float CalculateShootingSpeedCorrection(float yDistance)
    {
        float speedCorrection = 0;

        float absoluteDifference = Mathf.Abs(yDistance);

        if (yDistance > 0)
        {
            if (absoluteDifference >= 1)
            {
                speedCorrection += 0.1f;

                absoluteDifference -= 1;

                int correctionFactor = Mathf.RoundToInt(absoluteDifference / 0.25f);

                speedCorrection += 0.1f * correctionFactor;
            }
        }
        else
        {
            speedCorrection += 0.7f;

            absoluteDifference -= 1;

            if (absoluteDifference > 2.5f)
            {
                speedCorrection += 0.6f;

                absoluteDifference -= 2.5f;

                int correctionFactor = Mathf.RoundToInt(absoluteDifference / 0.25f);

                speedCorrection += 0.15f * correctionFactor;
            }
            else
            {
                int correctionFactor = Mathf.RoundToInt(absoluteDifference / 0.25f);

                speedCorrection += 0.1f * correctionFactor;
            }
        }

        return speedCorrection;
    }

    private IEnumerator ShootArrow(float shootingForce, Vector2 direction)
    {
        yield return new WaitForSeconds(shootingWaitingTime);

        Arrow newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(transform.localScale.x * .16f, 0, 0), Quaternion.identity);
        newArrow.transform.SetParent(projectileContainer.transform);

        if (transform.localScale.x < 0)
        {
            newArrow.transform.localScale = new Vector3(newArrow.transform.localScale.x * -1f, newArrow.transform.localScale.y, newArrow.transform.localScale.z);
        }

        newArrow.GetComponent<Rigidbody2D>().velocity = direction * shootingForce;
    }
}
