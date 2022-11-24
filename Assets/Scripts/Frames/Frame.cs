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
    }

    public void StartFrame()
    {
        CleanFrame();

        if (!player)
            player = GameManager.instance.GetPlayer();

        player.transform.position = playerSpawnPoint.position;

        enemiesTotalCount = randomEnemySpawners.Length;

        StartCoroutine(WaitAndSpawnEnemies(Config.SMALL_DELAY));

        if (enemiesKilledCount != enemiesTotalCount)
            frameChangeTrigger.gameObject.SetActive(false);

        CinemachineVirtualCamera cinemachineVirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = player.transform;

        GameManager.instance.GetCinemachineShake().SetVirtualCamera();

    }

    private IEnumerator WaitAndSpawnEnemies(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        foreach (RandomEnemySpawner randomEnemySpawner in randomEnemySpawners)
        {
            GameObject newEnemy = randomEnemySpawner.SpawnEnemy();

            enemies.Add(newEnemy);

            newEnemy.GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += EnemyKilled;
        }
    }

    public void RestartFrame()
    {
        CleanFrame();

        player.transform.position = playerSpawnPoint.position;
        StartCoroutine(WaitAndResurrectPlayer(Config.SMALL_DELAY));

        StartCoroutine(WaitAndSpawnEnemies(Config.SMALL_DELAY + 0.02f));
    }

    private IEnumerator WaitAndResurrectPlayer(float duration)
    {
        yield return new WaitForSeconds(duration);

        player.GetComponent<DamageReceiver>().Resurrect();
    }

    public void CleanFrame()
    {
        frameCleared = false;
        enemiesKilledCount = 0;

        ShowArrowUI();
        GameManager.instance.GetAnimationManager().ShowImageUI(Config.SPACE_KEY_GUI, false);
        StartCoroutine(GameManager.instance.GetLevelLoader().CinematicBracketsEnd());

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        enemies.Clear();

        if (projectileContainer)
        {
            Destroy(projectileContainer.gameObject);
            projectileContainer = new GameObject(Config.PROJECTILE_CONTAINER_NAME);
            projectileContainer.transform.SetParent(gameObject.transform);
            projectileContainer.tag = Config.PROJECTILE_CONTAINER_NAME;
        }
        else
        {
            projectileContainer = new GameObject(Config.PROJECTILE_CONTAINER_NAME);
            projectileContainer.transform.SetParent(gameObject.transform);
            projectileContainer.tag = Config.PROJECTILE_CONTAINER_NAME;
        }
        

        if (enemiesKilledCount != enemiesTotalCount)
            frameChangeTrigger.gameObject.SetActive(false);
    }

    private void EnemyKilled()
    {
        enemiesKilledCount++;
        GameManager.instance.GetCurrentProgressManager().IncreaseEnemiesKilledCount();

        if (enemiesKilledCount == enemiesTotalCount)
        {
            FrameCleared();
        }
    }

    private void FrameCleared()
    {
        frameCleared = true;
        frameChangeTrigger.gameObject.SetActive(true);
        GameManager.instance.GetSFXManager().PlaySound(Config.STAGE_CLEARED_SFX);

        ShowArrowUI();
    }

    private void ShowArrowUI()
    {
        if (rightSided)
        {
            GameManager.instance.GetAnimationManager().ShowImageUI(Config.RIGHT_ARROW_GUI, frameCleared);
        }
        else
        {
            GameManager.instance.GetAnimationManager().ShowImageUI(Config.LEFT_ARROW_GUI, frameCleared);
        }
    }

    public void ShowArrowUI(bool active)
    {
        if (rightSided)
        {
            GameManager.instance.GetAnimationManager().ShowImageUI(Config.RIGHT_ARROW_GUI, active);
        }
        else
        {
            GameManager.instance.GetAnimationManager().ShowImageUI(Config.LEFT_ARROW_GUI, active);
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
