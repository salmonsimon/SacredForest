using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class OneWayPlatform : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    private CapsuleCollider2D playerCollider;

    private float upperBorderYPosition;

    [SerializeField] private bool isActive = false;
    public bool IsActive { get { return isActive; } private set { isActive = value; } }

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
        if (PlayerFeetPosition() > upperBorderYPosition)
        {
            boxCollider.enabled = true;
            IsActive = true;
        }
        else
        {
            boxCollider.enabled = false;
            IsActive = false;
        }
    }

    public void DeactivatePlatform()
    {
        boxCollider.enabled = false;
        IsActive = false;
    }
}
