using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxyGoal : MonoBehaviour
{
    [SerializeField] private GameObject cutscenesObject;
    [SerializeField] BoxCollider2D portalBlock;

    private BoxCollider arrivedCheck;
    private bool hasArrived = false;

    private void Start()
    {
        arrivedCheck = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (!hasArrived && arrivedCheck.IsColliding())
        {
            hasArrived = true;

            GameObject.FindGameObjectWithTag("Foxy").GetComponent<FoxyController>().FlipTowardsPlayer();
            cutscenesObject.GetComponent<Cutscene02>().enabled = true;

            portalBlock.enabled = false;
            portalBlock.gameObject.SetActive(false);

            this.gameObject.SetActive(false);
        }
    }
}
