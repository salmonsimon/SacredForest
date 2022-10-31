using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    [SerializeField] private CircleCollider groundCheck;
    [SerializeField] protected ParticleSystem brokeParticles;

    private bool hasHitGround;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += Death;
    }

    private void Update()
    {
        if(!hasHitGround)
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

            hasHitGround = groundCheck.IsColliding();
            if (hasHitGround)
            {
                rigidBody.AddForce(-rigidBody.velocity);
                rigidBody.velocity = Vector2.zero;
                rigidBody.isKinematic = true;

                GameManager.instance.GetSFXManager().PlaySound(Config.ARROW_HIT_SFX);

                Destroy(GetComponent<BoxDamageDealer>());
                Destroy(GetComponent<DamageReceiver>());
                Destroy(GetComponent<BoxCollider2D>());

                GetComponent<Animator>().enabled = false;
            }
        }
    }

    private void Death()
    {
        brokeParticles.Play();

        Destroy(GetComponent<BoxDamageDealer>());
        Destroy(GetComponent<DamageReceiver>());
        Destroy(GetComponent<BoxCollider2D>());

        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(1f);

        Destroy(gameObject);
    }
}
