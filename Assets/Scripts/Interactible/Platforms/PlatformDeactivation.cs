using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformDeactivation : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private CapsuleCollider2D playerCollider;

    private float upperBorderYPosition;

    [SerializeField] SpriteRenderer parentSpriteRenderer;
    [SerializeField] OneWayPlatform oneWayPlatform;
    private bool waitingToReactivate = false;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        playerCollider = GameManager.instance.GetPlayer().GetComponent<CapsuleCollider2D>();

        CalculateUpperBorderYPosition();

        boxCollider.enabled = false;
    }

    private void CalculateUpperBorderYPosition()
    {
        upperBorderYPosition = boxCollider.bounds.max.y;
    }

    private float PlayerFeetPosition()
    {
        return playerCollider.bounds.min.y + 0.03f;
    }

    private void Update()
    {
        if (oneWayPlatform.gameObject.activeSelf)
        {
            if (PlayerFeetPosition() > upperBorderYPosition)
            {
                oneWayPlatform.gameObject.SetActive(false);
                oneWayPlatform.DeactivatePlatform();
                parentSpriteRenderer.enabled = false;
            }
        }
        else
        {
            if (!waitingToReactivate)
            {
                StartCoroutine(WaitUntilGroundedToReactivate(playerCollider.gameObject));
            }
        }
    }

    protected IEnumerator WaitUntilGroundedToReactivate(GameObject player)
    {
        waitingToReactivate = true;

        while (!player.GetComponent<Mover>().IsGrounded())
        {
            yield return null;
        }

        oneWayPlatform.gameObject.SetActive(true);
        parentSpriteRenderer.enabled = true;
        waitingToReactivate = false;
    }
}
