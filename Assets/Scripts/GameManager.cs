using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Logic Variables

    private bool isGamePaused;

    #endregion

    private void Awake()
    {
        if (GameManager.instance != null)
        {
            // Destroy persistent objects
        }
        else
        {
            instance = this;
        }
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }
}
