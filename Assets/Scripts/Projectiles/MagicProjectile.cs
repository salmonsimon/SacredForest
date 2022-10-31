using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rigidBody;
    private CircleDamageDealer damageDealer;

    [SerializeField] private CircleCollider swordCollisionCheck;
    [SerializeField] private CircleCollider collisionCheck;
    private bool hasHit;

    [SerializeField] private bool isDestroyable = false;

    private bool hasBeenReflected = false;

    [SerializeField] private float hitAnimationDuration = .6f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        damageDealer = GetComponent<CircleDamageDealer>();

        if (isDestroyable)
            GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += Death;
    }

    private void Start()
    {
        if (rigidBody.velocity.x > 0)
        {
            float angle = Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            float angle = Mathf.Atan2(rigidBody.velocity.y, -rigidBody.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }
    }

    private void Update()
    {
        if (!hasHit)
        {
            if (!hasBeenReflected && swordCollisionCheck.IsColliding())
            {
                hasBeenReflected = true;

                transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);

                Vector2 originalVelocity = rigidBody.velocity;

                rigidBody.velocity = Vector2.zero;
                rigidBody.velocity = -originalVelocity;

                List<string> tagsToCheckCollision = new List<string> 
                {
                    Config.GROUND_TAG, Config.ENEMY_TAG
                };

                List<string> tagsToCheckDamage = new List<string>
                {
                    Config.ENEMY_TAG
                };

                collisionCheck.ChangeTagsToCheck(tagsToCheckCollision);
                damageDealer.ChangeTagsToCheck(tagsToCheckDamage);

                return;
            }

            hasHit = collisionCheck.IsColliding();

            if (hasHit)
            {
                Hit();
            }
        }
    }

    private IEnumerator WaitAndDestroy(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        Destroy(gameObject);
    }

    private void Hit()
    {
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;

        animator.SetTrigger(Config.PROJECTILE_ANIMATOR_HIT_TRIGGER);

        Destroy(GetComponent<BoxDamageDealer>());

        StartCoroutine(WaitAndDestroy(hitAnimationDuration));
    }

    private void Death()
    {
        Hit();
    }
}
