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

    [SerializeField] private bool rightSided = true;
    private bool frameCleared = false;

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
        frameCleared = false;
        enemiesKilledCount = 0;

        ShowArrowUI();
        GameManager.instance.GetAnimationManager().ShowImageUI("Space Key", false);

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
        frameCleared = true;
        frameChangeTrigger.gameObject.SetActive(true);

        ShowArrowUI();
    }

    private void ShowArrowUI()
    {
        if (rightSided)
        {
            GameManager.instance.GetAnimationManager().ShowImageUI("Right Arrow", frameCleared);
        }
        else
        {
            GameManager.instance.GetAnimationManager().ShowImageUI("Left Arrow", frameCleared);
        }
    }

    public void ShowArrowUI(bool active)
    {
        if (rightSided)
        {
            GameManager.instance.GetAnimationManager().ShowImageUI("Right Arrow", active);
        }
        else
        {
            GameManager.instance.GetAnimationManager().ShowImageUI("Left Arrow", active);
        }
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
