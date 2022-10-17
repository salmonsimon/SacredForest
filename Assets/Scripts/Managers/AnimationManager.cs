using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] GameObject rightArrow;
    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject spaceKey;

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
            case "Right Arrow":
                rightArrow.SetActive(active);
                break;

            case "Left Arrow":
                leftArrow.SetActive(active);
                break;

            case "Space Key":
                spaceKey.SetActive(active);
                break;
        }
    }
}
