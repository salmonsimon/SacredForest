using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region Logic Variables

    private bool isGamePaused;
    public bool IsGamePaused { get => isGamePaused; private set => isGamePaused = value; }

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


}
