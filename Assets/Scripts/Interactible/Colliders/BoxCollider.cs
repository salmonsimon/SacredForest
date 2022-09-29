using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollider : GeneralCollider
{
    protected BoxCollider2D boxCollider;

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        if (!isColliding)
        {
            boxCollider.OverlapCollider(filter, hits);

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
