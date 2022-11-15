using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Vector3 originalScale;

    protected virtual void Start()
    {
        originalScale = transform.localScale;
    }

    protected virtual void Update()
    {
        float relativePlayerPositionX = GameManager.instance.GetPlayer().transform.position.x - transform.position.x;
        Flip(new Vector2(relativePlayerPositionX, 0));
    }

    public void Flip(Vector2 movement)
    {
        if (movement.x > 0)
        {
            if (originalScale.x > 0)
            {
                transform.localScale = originalScale;
            }
            else
            {
                transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z);
            }
        }
        else if (movement.x < 0)
        {
            if (originalScale.x > 0)
            {
                transform.localScale = new Vector3(originalScale.x * -1f, originalScale.y, originalScale.z);
            }
            else
            {
                transform.localScale = originalScale;
            }
        }
    }
}
