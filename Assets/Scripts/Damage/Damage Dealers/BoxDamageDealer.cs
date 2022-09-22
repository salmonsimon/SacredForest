using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxDamageDealer : BoxCollider
{
    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[i]))
            {
                Debug.Log("Damaged " + coll.name);
                coll.SendMessage("ReceiveDamage");
            }
        }
    }
}
