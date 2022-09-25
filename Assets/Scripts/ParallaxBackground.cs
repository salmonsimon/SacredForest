using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{

    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float parallaxEffect;

    private float startPosition, lenght;


    private void Start()
    {
        cameraTransform = Camera.main.transform;

        startPosition = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        float deltaFromCamera = (cameraTransform.position.x * (1 - parallaxEffect));
        float deltaPosition = cameraTransform.position.x * parallaxEffect;

        transform.position = new Vector3(startPosition + deltaPosition, transform.position.y, transform.position.z);

        if (deltaFromCamera > startPosition + lenght)
            startPosition += lenght;
        else if(deltaFromCamera < startPosition - lenght)
            startPosition -= lenght;
    }
}
