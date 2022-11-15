using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneFightingMecanics : Cutscene
{
    private GameObject player;
    [SerializeField] private GameObject enemy;

    [SerializeField] BoxCollider arrowCollider;

    protected override void Start()
    {
        base.Start();

        player = GameManager.instance.GetPlayer();
    }

    private void Update()
    {
        if (arrowCollider.IsColliding())
        {
            // freeze arrow position
            // deactivate archer
            // show x button

            // after pressing x show cutscene killing the enemy
            // and going into the portal
        }
    }
}
