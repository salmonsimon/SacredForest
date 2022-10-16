using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    #region Colliders

    [SerializeField] private BoxCollider2D visionFieldCollider;
    [SerializeField] private BoxCollider2D groupAlertCollider;

    #endregion

    #region Layers

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask playerLayer;

    #endregion

    #region Parameters

    private float alertGroupDelay = Config.ALERT_GROUP_DELAY;

    #endregion

    #region Player Detected Property and Event

    private bool detectedPlayer = false;
    public bool DetectedPlayer
    {
        get { return detectedPlayer; }
        set
        {
            if (detectedPlayer == value) return;

            detectedPlayer = value;
            if (OnDetectedPlayer != null && detectedPlayer == true)
                OnDetectedPlayer();
        }
    }

    public delegate void OnDetectedPlayerDelegate();
    public event OnDetectedPlayerDelegate OnDetectedPlayer;

    #endregion

    public void CheckForPlayer()
    {
        RaycastHit2D hit = Physics2D.BoxCast(visionFieldCollider.bounds.center, visionFieldCollider.bounds.size, 0, Vector2.left, 0, playerLayer);

        if (hit.collider)
        {
            visionFieldCollider.enabled = false;
            DetectedPlayer = true;
        }
    }

    public IEnumerator AlertGroupAfterDetectingPlayer()
    {
        yield return new WaitForSeconds(alertGroupDelay);

        RaycastHit2D[] enemyGroup = Physics2D.BoxCastAll(groupAlertCollider.bounds.center, groupAlertCollider.bounds.size, 0, Vector2.left, 0, enemyLayer);

        if (enemyGroup != null)
        {
            foreach (RaycastHit2D enemy in enemyGroup)
            {
                if (enemy.transform.gameObject.GetComponent<DamageReceiver>().IsAlive)
                    enemy.transform.gameObject.GetComponent<PlayerDetection>().DetectedPlayer = true;
            }
        }

        groupAlertCollider.enabled = false;
    }
    private void OnDrawGizmos()
    {
        if (!DetectedPlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(visionFieldCollider.bounds.center, visionFieldCollider.bounds.size);
            Gizmos.DrawWireCube(groupAlertCollider.bounds.center, groupAlertCollider.bounds.size);
        }
   
    }
}
