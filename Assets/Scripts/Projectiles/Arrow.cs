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
        GetComponent<DamageReceiver>().OnCharacterDeath += Death;
    }

    private void Update()
    {
        if(!hasHitGround)
        {
            float angle = Mathf.Atan2(rigidBody.velocity.y, rigidBody.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            hasHitGround = groundCheck.IsColliding();
            if(hasHitGround)
            {
                rigidBody.velocity = Vector2.zero;
                rigidBody.isKinematic = true;

                Destroy(GetComponent<BoxDamageDealer>());
                GetComponent<Animator>().enabled = false;
            }
        }
    }

    private void Death()
    {
        brokeParticles.Play();
        Destroy(GetComponent<BoxDamageDealer>());
    }
}
