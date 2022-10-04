using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    private bool detectedPlayer = false;
    public bool DetectedPlayer
    {
        get { return detectedPlayer; }
        set
        {
            if (detectedPlayer == value) return;

            detectedPlayer = value;
            if (OnDetectedPlayer != null && detectedPlayer == true)
                OnDetectedPlayer();
        }
    }

    public delegate void OnDetectedPlayerDelegate();
    public event OnDetectedPlayerDelegate OnDetectedPlayer;
}
