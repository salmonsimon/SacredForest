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
}
