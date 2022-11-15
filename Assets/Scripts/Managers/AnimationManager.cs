using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [Header("Screen Overlay")]

    [SerializeField] GameObject rightArrow;
    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject spaceKey;

    [Space(10)]
    [Header("World Space")]
    [Space(2)]

    [SerializeField] GameObject worldspaceCanvas;
    [SerializeField] FloatingUI zKeyPrefab;
    [SerializeField] FloatingUI xKeyPrefab;
    [SerializeField] FloatingUI cKeyPrefab;
    [SerializeField] FloatingUI movementKeysPrefab;

    private void Start()
    {
        rightArrow.SetActive(false);
        leftArrow.SetActive(false);
        spaceKey.SetActive(false);
    }

    public void ShowImageUI(string imageName , bool active)
    {
        switch(imageName)
        {
            case Config.RIGHT_ARROW_GUI:
                rightArrow.SetActive(active);
                break;

            case Config.LEFT_ARROW_GUI:
                leftArrow.SetActive(active);
                break;

            case Config.SPACE_KEY_GUI:
                spaceKey.SetActive(active);
                break;
        }
    }

    public void ShowImageUIWorldSpace(FloatingUI prefab, Vector3 position, Transform lookAt = null, float duration = -1)
    {
        FloatingUI floatingUI = Instantiate(prefab, worldspaceCanvas.transform);

        floatingUI.transform.position = position;

        if (lookAt != null)
        {
            floatingUI.lookAt = lookAt;
            floatingUI.originalDisplacement = position - lookAt.position;
        }

        if (duration > 0)
        {
            floatingUI.duration = duration;
        }
    }

    public void ShowImageUIWorldSpace(string imageName, Vector3 position, Transform lookAt = null, float duration = -1)
    {
        switch (imageName)
        {
            case Config.Z_KEY_GUI:
                FloatingUI zKey = Instantiate(zKeyPrefab, worldspaceCanvas.transform);

                zKey.transform.position = position;

                if (lookAt != null)
                {
                    zKey.lookAt = lookAt;
                    zKey.originalDisplacement = position - lookAt.position;
                }

                if (duration > 0)
                {
                    zKey.duration = duration;
                }

                break;

            case Config.X_KEY_GUI:
                FloatingUI xKey = Instantiate(xKeyPrefab, worldspaceCanvas.transform);

                xKey.transform.position = position;

                if (lookAt != null)
                {
                    xKey.lookAt = lookAt;
                    xKey.originalDisplacement = position - lookAt.position;
                }

                if (duration > 0)
                {
                    xKey.duration = duration;
                }

                break;

            case Config.C_KEY_GUI:
                FloatingUI cKey = Instantiate(cKeyPrefab, worldspaceCanvas.transform);

                cKey.transform.position = position;

                if (lookAt != null)
                {
                    cKey.lookAt = lookAt;
                    cKey.originalDisplacement = position - lookAt.position;
                }

                if (duration > 0)
                {
                    cKey.duration = duration;
                }

                break;

            case Config.MOVEMENT_KEYS_GUI:
                FloatingUI movementKeys = Instantiate(movementKeysPrefab, worldspaceCanvas.transform);

                movementKeys.transform.position = position;

                if (lookAt != null)
                {
                    movementKeys.lookAt = lookAt;
                    movementKeys.originalDisplacement = position - lookAt.position;
                }

                if (duration > 0)
                {
                    movementKeys.duration = duration;
                }

                break;
        }
    }

    public void ClearWorldSpaceCanvas()
    {
        foreach (Transform child in worldspaceCanvas.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearScreenOverlayCanvas()
    {
        ShowImageUI(Config.SPACE_KEY_GUI, false);
        ShowImageUI(Config.RIGHT_ARROW_GUI, false);
        ShowImageUI(Config.LEFT_ARROW_GUI, false);
    }

    public void ClearCanvases()
    {
        ClearWorldSpaceCanvas();
        ClearScreenOverlayCanvas();
    }
}
