using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : GeneralCollider
{
    private PolygonCollider2D polygonCollider;
    private Animator animator;

    PlayerPortalAnimation playerPortalAnimation;

    [SerializeField] private string sceneName;


    [SerializeField] private FightingRoute fightingRoute;

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

        GameManager.instance.GetCurrentProgressManager().SaveCurrentProgress();
        GameManager.instance.GetCurrentProgressManager().UpdateCurrentFightingRoute(fightingRoute);

        animator.SetTrigger("Close");
        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = false;

        playerPortalAnimation.PlayerTeleport(Config.BIG_DELAY * 2);

        yield return new WaitForSeconds(Config.BIG_DELAY);

        GameManager.instance.GetLevelLoader().LoadLevel(sceneName, Config.CROSSFADE_TRANSITION);
    }
}
