using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomEnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] possibleEnemies;
    [SerializeField] private bool facingRight;

    public GameObject SpawnEnemy()
    {
        int randomIndex = Random.Range(0, possibleEnemies.Length);

        GameObject newEnemy = Instantiate(possibleEnemies[randomIndex], transform.position, Quaternion.identity);

        if (!facingRight)
        {
            newEnemy.transform.localScale = new Vector3(newEnemy.transform.localScale.x * -1f, newEnemy.transform.localScale.y, newEnemy.transform.localScale.z);
        }

        return newEnemy;
    }
}