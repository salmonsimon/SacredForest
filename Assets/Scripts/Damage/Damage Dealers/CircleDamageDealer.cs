using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamageDealer : CircleCollider
{
    [SerializeField] int damage = 1;

    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[i]))
            {
                if (coll.GetComponent<DamageReceiver>().IsAlive)
                    coll.SendMessage("ReceiveDamage", damage);
            }
        }
    }
}
