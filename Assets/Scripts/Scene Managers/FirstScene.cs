using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstScene : MonoBehaviour
{
    [SerializeField] GameObject foxyIntro;
    [SerializeField] GameObject foxyNPC;


    private void Start()
    {
        if (!GameManager.instance.GetCurrentProgressManager().FirstTimePlaying)
        {
            foxyNPC.SetActive(true);
            foxyIntro.SetActive(false);
        }
    }
}
