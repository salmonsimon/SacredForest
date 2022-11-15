using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public Transform lookAt;
    public Vector3 originalDisplacement;
    public float duration = -1;

    private float timeAlive = 0;
    public float TimeAlive { get { return timeAlive; } }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if (duration > 0 && timeAlive >= duration)
            Destroy(gameObject);

        UpdateDialogueBubble();
    }

    public void UpdateDialogueBubble()
    {
        if (lookAt)
        {
            Vector3 displacement = lookAt.position - transform.position + originalDisplacement;

            transform.position += displacement;
        }
    }
}
