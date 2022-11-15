using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollider : GeneralCollider
{
    protected BoxCollider2D boxCollider;

    protected float timeToCheck = 0;

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        timeToCheck -= Time.deltaTime;

        if (!isColliding || timeToCheck < 0)
        {
            timeToCheck = 1f;

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
