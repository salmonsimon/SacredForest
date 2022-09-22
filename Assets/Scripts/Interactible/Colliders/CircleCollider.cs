using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollider : GeneralCollider
{
    private CircleCollider2D circleCollider;

    protected virtual void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    protected virtual void Update()
    {
        if (!isColliding)
        {
            circleCollider.OverlapCollider(filter, hits);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                OnCollide(hits[i]);

                hits[i] = null;
            }
        }
    }
}
