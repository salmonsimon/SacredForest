using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeCheck : CircleCollider
{
    [SerializeField] private string groundType;

    protected override void Awake()
    {
        base.Awake();

        tagsToCheck.Add("Ground Type");
    }

    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[i]))
            {
                isColliding = boolToAssing;

                if (isColliding)
                    groundType = coll.GetComponent<Ground>().GetGroundType();
                //else
                    //groundType = null;

                break;
            }
        }
    }

    public string GetGroundType()
    {
        return groundType;
    }
}
