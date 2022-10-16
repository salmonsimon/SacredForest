using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Frame : MonoBehaviour
{
    private int enemiesTotalCount;
    private int enemiesKilledCount = 0;

    [SerializeField] private RandomEnemySpawner[] randomEnemySpawners;
    private List<GameObject> enemies = new List<GameObject>();

    [SerializeField] private BoxCollider frameChangeTrigger;

    private GameObject player;
    [SerializeField] private Transform playerSpawnPoint;

    GameObject projectileContainer;

    private void Start()
    {
        player = GameManager.instance.GetPlayer();

        projectileContainer = new GameObject("Projectile Container");
        projectileContainer.transform.SetParent(gameObject.transform);
        projectileContainer.tag = "Projectile Container";
    }

    public void StartFrame()
    {
        CleanFrame();

        if (!player)
            player = GameManager.instance.GetPlayer();

        player.transform.position = playerSpawnPoint.position;

        enemiesTotalCount = randomEnemySpawners.Length;

        foreach (RandomEnemySpawner randomEnemySpawner in randomEnemySpawners)
        {
            GameObject newEnemy = randomEnemySpawner.SpawnEnemy();

            enemies.Add(newEnemy);

            newEnemy.GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += EnemyKilled;
        }

        if (enemiesKilledCount != enemiesTotalCount)
            frameChangeTrigger.gameObject.SetActive(false);

        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;

        GameManager.instance.GetCinemachineShake().SetVirtualCamera();

    }

    public void RestartFrame()
    {
        CleanFrame();

        player.GetComponent<DamageReceiver>().Resurrect();
        player.transform.position = playerSpawnPoint.position;

        StartCoroutine(SpawnEnemiesAfterWait());
    }

    private IEnumerator SpawnEnemiesAfterWait()
    {
        yield return new WaitForSeconds(.1f);

        foreach (RandomEnemySpawner randomEnemySpawner in randomEnemySpawners)
        {
            GameObject newEnemy = randomEnemySpawner.SpawnEnemy();

            enemies.Add(newEnemy);

            newEnemy.GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += EnemyKilled;
        }
    }

    public void CleanFrame()
    {
        enemiesKilledCount = 0;

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        enemies.Clear();

        if (projectileContainer)
        {
            Destroy(projectileContainer.gameObject);
            projectileContainer = new GameObject("Projectile Container");
            projectileContainer.transform.SetParent(gameObject.transform);
            projectileContainer.tag = "Projectile Container";
        }
        

        if (enemiesKilledCount != enemiesTotalCount)
            frameChangeTrigger.gameObject.SetActive(false);
    }

    private void EnemyKilled()
    {
        enemiesKilledCount++;

        if (enemiesKilledCount == enemiesTotalCount)
        {
            FrameCleared();
        }
    }

    private void FrameCleared()
    {
        // print arrow indicating the direction to go

        frameChangeTrigger.gameObject.SetActive(true);
    }

    public bool FrameChangeTriggered()
    {
        if (frameChangeTrigger)
            return frameChangeTrigger.IsColliding();
        else
            return false;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}
