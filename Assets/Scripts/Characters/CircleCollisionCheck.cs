using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollisionCheck : MonoBehaviour
{
    [SerializeField] private bool isColliding;
    [SerializeField] List<string> tagsToCheck;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (collision.gameObject.CompareTag(tagsToCheck[i]))
            {
                isColliding = true;
                break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        for (int i = 0; i < tagsToCheck.Count; i++)
        {
            if (collision.gameObject.CompareTag(tagsToCheck[i]))
            {
                isColliding = false;
                break;
            }
        }
    }

    public bool IsColliding()
    {
        return isColliding;
    }
}
