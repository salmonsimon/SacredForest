using UnityEngine;
using UnityEngine.UI;

public class FloatingText
{

    public bool active;
    public GameObject go;
    public Text txt;
    public Vector3 motion;
    public float duration;
    public float lastShown;
    public Transform lookAt;
    public Vector3 originalDisplacement; 

    public void Show()
    {
        active = true;
        lastShown = Time.time;

        go.SetActive(active);
    }

    public void Hide()
    {
        active = false;

        go.SetActive(active);
    }

    public void UpdateFloatingText()
    {
        if (!active)
            return;

        if (Time.time - lastShown > duration)
        {
            Hide();
        }

        if (lookAt)
        {
            Vector3 displacement = lookAt.position - go.transform.position + originalDisplacement;

            go.transform.position += displacement;
        }

        originalDisplacement += motion * Time.deltaTime;
        go.transform.position += motion * Time.deltaTime;
    }
}
