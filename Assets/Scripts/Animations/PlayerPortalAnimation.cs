using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPortalAnimation : MonoBehaviour
{
    private float portalUseTimer = 0f;
    private float portalUseTimerTotal = Config.BIG_DELAY;

    public void PlayerTeleport(float duration)
    {
        portalUseTimer = duration;
        portalUseTimerTotal = duration;
    }


    private void Update()
    {
        if (portalUseTimer > 0)
        {
            portalUseTimer -= Time.deltaTime;

            GameManager.instance.GetPlayer().transform.localScale *= Mathf.Lerp(0f, 1, portalUseTimer / portalUseTimerTotal);

            float playerZRotation = Mathf.Lerp(-450f, 0, portalUseTimer / portalUseTimerTotal);

            Quaternion playerRotation = GameManager.instance.GetPlayer().transform.rotation;
            GameManager.instance.GetPlayer().transform.rotation = Quaternion.Euler(playerRotation.x, playerRotation.y, playerZRotation);
        }
    }
}
