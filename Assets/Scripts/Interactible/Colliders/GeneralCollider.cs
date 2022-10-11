using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCollider : MonoBehaviour
{
    [SerializeField] protected bool isColliding;
    [SerializeField] protected List<string> tagsToCheck;

    protected ContactFilter2D filter;
    protected Collider2D[] hits = new Collider2D[10];

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckCollision(collision, false);
    }

    protected void OnCollide(Collider2D collision)
    {
        CheckCollision(collision, true);
    }

    protected virtual void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (coll.gameObject.CompareTag(tagsToCheck[i]))
            {
                isColliding = boolToAssing;
                break;
            }
        }
    }

    public bool IsColliding()
    {
        return isColliding;
    }

    public void ChangeTagsToCheck(List<string> tagsToCheck)
    {
        this.tagsToCheck = tagsToCheck;
    }
}
