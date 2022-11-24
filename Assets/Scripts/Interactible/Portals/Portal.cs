using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : GeneralCollider
{
    private PolygonCollider2D polygonCollider;
    private Animator animator;

    PlayerPortalAnimation playerPortalAnimation;

    [SerializeField] protected string sceneName;


    [SerializeField] protected FightingRoute fightingRoute;

    protected virtual void Awake()
    {
        polygonCollider = GetComponent<PolygonCollider2D>();
        animator = GetComponent<Animator>();
        playerPortalAnimation = GetComponent<PlayerPortalAnimation>();
    }

    protected override void CheckCollision(Collider2D coll, bool boolToAssing)
    {
        base.CheckCollision(coll, boolToAssing);

        if (isColliding && !GameManager.instance.IsTeleporting())
        {
            StartCoroutine(UsePortal());
        }
    }

    private IEnumerator UsePortal()
    {
        GameManager.instance.SetIsTeleporting(true);

        GameManager.instance.GetDialogueManager().ClearDialogues();

        CurrentProgressManager currentProgressManager = GameManager.instance.GetCurrentProgressManager();

        if (currentProgressManager.FirstTimePlaying)
            currentProgressManager.FirstTimePlaying = false;

        currentProgressManager.SaveCurrentProgress();
        currentProgressManager.UpdateCurrentFightingRoute(fightingRoute);

        if (SceneManager.GetActiveScene().name == Config.MAIN_SCENE_NAME)
            ZSerializer.ZSerialize.SaveScene();

        animator.SetTrigger("Close");
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = false;

        StartCoroutine(SmoothPlayerPlacement());

        yield return new WaitForSeconds(Config.SMALL_DELAY);

        playerPortalAnimation.PlayerTeleport(Config.BIG_DELAY * 2);

        yield return new WaitForSeconds(Config.BIG_DELAY);

        GameManager.instance.GetLevelLoader().LoadLevel(sceneName, Config.CROSSFADE_TRANSITION);
    }

    private IEnumerator SmoothPlayerPlacement()
    {
        float duration = .2f;

        float startTime = Time.time;
        float endTime = startTime + duration;

        GameObject player = GameManager.instance.GetPlayer();

        Vector3 playerOriginalPosition = player.transform.position;
        Vector3 playerFinalPosition = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);

        while (Time.time < endTime)
        {
            player.transform.position = Vector3.Lerp(playerOriginalPosition, playerFinalPosition, (Time.time - startTime) / duration);

            yield return null;
        }
    }
}
