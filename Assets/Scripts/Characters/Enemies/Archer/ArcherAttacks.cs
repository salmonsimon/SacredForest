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

    [SerializeField] private float[] shootingSpeeds = { 7f };

    private float shootingSpeed;
    private float shootingAngle;
    private Vector2 shootingDirection;
    private bool isGoingToShoot = false;

    private GameObject projectileContainer;

    #endregion

    protected override void Start()
    {
        base.Start();

        projectileContainer = GameObject.FindGameObjectWithTag(Config.PROJECTILE_CONTAINER_NAME);
    }

    public void ArrowAttack(Transform archerTransform, Transform playerTransform)
    {
        isGoingToShoot = false;

        float xDistance = playerTransform.position.x - archerTransform.position.x;
        float yDistance = playerTransform.position.y - archerTransform.position.y;

        CalculateShot(xDistance, yDistance);

        if (isGoingToShoot)
        {
            GetComponent<EnemyMover>().Flip(new Vector2(xDistance, 0));
            StartCoroutine(GetComponent<EnemyMover>().MovementCooldown(shootingWaitingTime + Config.MEDIUM_DELAY));
            StartCoroutine(IsAttackingCooldown(shootingWaitingTime + Config.MEDIUM_DELAY));

            StartCoroutine(PlayClip(Animator.StringToHash(arrowAnimationClip.name), 0));
            StartCoroutine(ShootArrow(archerTransform, playerTransform));
        }
    }

    private void CalculateShot(float xDistance, float yDistance)
    {
        float distance = new Vector2(xDistance, yDistance).magnitude;

        if (distance > 6)
            return;
        else if (distance > 2)
            CalculateLongRangeShot(xDistance, yDistance);
        else
            CalculateShortRangeShot(xDistance, yDistance);
    }

    private void CalculateLongRangeShot(float xDistance, float yDistance) 
    {
        float[] shootingSpeedsToCheck = shootingSpeeds;

        if (yDistance < -0.5f)
            shootingSpeedsToCheck[0] = 10f;

        float yGravity = Physics2D.gravity.y;

        foreach (float shootingSpeed in shootingSpeedsToCheck)
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
                
                if (yDistance < 1 && yDistance > 0)
                    this.shootingDirection += new Vector2(0, 0.1f);

                isGoingToShoot = true;
            }
        }
    }

    private void CalculateShortRangeShot(float xDistance, float yDistance)
    {
        this.shootingDirection = new Vector2(xDistance, yDistance);

        Vector2 horizontal = Vector2.right;

        if (xDistance < 0)
            horizontal = Vector2.left;

        float shootingAngle = Vector2.Angle(horizontal, shootingDirection);

        if (yDistance > -0.5)
        {
            if (shootingAngle > 50 && shootingAngle <= 80)
                this.shootingSpeed = 12f;
            else if (shootingAngle > 80 && shootingAngle < 100)
                this.shootingSpeed = 17f;
            else
                this.shootingSpeed = 7f;
        }
        else
        {
            this.shootingSpeed = 4f;
        }

        this.shootingDirection += new Vector2(0, 0.2f);

        isGoingToShoot = true;
    }

    private float CalculateShootingSpeedCorrection(float yDistance)
    {
        float speedCorrection = 0;

        float absoluteDifference = Mathf.Abs(yDistance);

        if (yDistance > 0)
        {
            if (absoluteDifference > 1)
            {
                speedCorrection += 0.6f;

                absoluteDifference -= 1f;

                int correctionFactor = Mathf.RoundToInt(absoluteDifference / 0.25f);

                speedCorrection += 0.2f * correctionFactor;
            }
        }
        else
        {
            if (absoluteDifference > 1)
                speedCorrection += 0.7f;
            else
                speedCorrection += 0.9f;

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

    private IEnumerator ShootArrow(Transform archerTransform, Transform playerTransform)
    {
        yield return new WaitForSeconds(shootingWaitingTime);

        if (isAlive)
        {
            float xDistance = playerTransform.position.x - archerTransform.position.x;
            float yDistance = playerTransform.position.y - archerTransform.position.y;

            CalculateShot(xDistance, yDistance);

            Arrow newArrow = Instantiate(arrowPrefab, transform.position + new Vector3(transform.localScale.x * .16f, 0, 0), Quaternion.identity);
            newArrow.transform.SetParent(projectileContainer.transform);

            if (transform.localScale.x < 0)
            {
                newArrow.transform.localScale = new Vector3(newArrow.transform.localScale.x * -1f, newArrow.transform.localScale.y, newArrow.transform.localScale.z);
            }

            newArrow.GetComponent<Rigidbody2D>().velocity = shootingDirection * shootingSpeed;
        }
    }
}
