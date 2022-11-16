using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPortal : Portal
{
    private void Start()
    {
        if (GameManager.instance.GetCurrentProgressManager().FirstTimePlaying)
        {
            sceneName = Config.FIGHTING_MECANICS_SCENE_NAME;
            fightingRoute = FightingRoute.None;
        }
        else if (GameManager.instance.GetCurrentProgressManager().FinishedRoute1)
        {
            sceneName = Config.FOXES_HOME_SCENE_NAME;
            fightingRoute = FightingRoute.None;
        }
    }
}
