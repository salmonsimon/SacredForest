using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDamageDealer : CircleCollider
{
    [SerializeField] int damage = 1;

    [SerializeField] private DamageType damageType;
    private enum DamageType
    {
        Sword,
        Default,
        Bludgeoning,
        Fire
    }

    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[i]))
            {
                if (coll.GetComponent<DamageReceiver>().IsAlive)
                {
                    Damage damage = new Damage
                    {
                        damage = this.damage,
                        damageType = this.damageType.ToString()
                    };

                    coll.SendMessage("ReceiveDamage", damage);
                }
            }
        }
    }
}
