using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MainMenuCameraMovement : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;
    private Vector3 directionToMove = Vector3.right;

    [SerializeField] float cameraSpeed = 0.01f;

    private bool waitingToMove = true;

    private void Start()
    {
        vcam = GameObject.FindGameObjectWithTag(Config.CINEMACHINE_CAMERA_TAG).GetComponent<CinemachineVirtualCamera>();

        StartCoroutine(WaitToMove());
    }

    private IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(1f);

        waitingToMove = false;
    }

    private void LateUpdate()
    {
        if (!waitingToMove)
            vcam.transform.position += directionToMove * cameraSpeed;
    }
}
