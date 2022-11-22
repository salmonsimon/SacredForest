using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneFightingMechanics : Cutscene
{
    private GameObject player;
    [SerializeField] private GameObject enemy;

    [SerializeField] BoxCollider arrowCollider;
    private bool arrowHasCollided = false;

    [SerializeField] private FloatingUI slashDownKeys;

    [SerializeField] OneWayPlatform shortJumpPlatform;
    [SerializeField] OneWayPlatform longJumpPlatform;

    protected override void Start()
    {
        base.Start();

        player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerMovementController>().enabled = false;
        player.GetComponent<PlayerAttackController>().enabled = false;
    }

    private void Update()
    {
        if (!arrowHasCollided && arrowCollider.IsColliding())
        {
            arrowHasCollided = true;

            StartCoroutine(Play());
        }
    }

    private IEnumerator Play()
    {
        GameObject arrow = GameObject.FindGameObjectWithTag("Arrow");
        arrow.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        GameManager.instance.GetAnimationManager().ShowImageUIWorldSpace(Config.X_KEY_GUI,
            new Vector3(arrow.transform.position.x, arrow.transform.position.y + .16f, arrow.transform.position.z));

        enemy.GetComponent<Archer>().enabled = false;

        yield return null;

        player.GetComponent<PlayerAttackController>().enabled = true;

        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return null;
        }

        player.GetComponent<PlayerAttackController>().enabled = false;

        GameManager.instance.GetAnimationManager().ClearWorldSpaceCanvas();

        yield return new WaitForSeconds(Config.BIG_DELAY);

        #region First Cutscene - First Attack

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Akate Animations", player);
        Bind(playableDirector, "Akate Movement", player);

        playableDirector.Play();

        while (playableDirector.state == PlayState.Playing)
        {
            yield return null;
        }

        #endregion

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameManager.instance.GetAnimationManager().ShowImageUIWorldSpace(Config.X_KEY_GUI,
            new Vector3(enemy.transform.position.x, enemy.transform.position.y + .32f, enemy.transform.position.z));

        yield return null;

        player.GetComponent<PlayerAttackController>().enabled = true;

        yield return null;

        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return null;
        }

        GameManager.instance.GetAnimationManager().ClearWorldSpaceCanvas();

        player.GetComponent<PlayerAttackController>().enabled = false;

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        #region Second Cutscene - Jump Back and Wait

        playableDirector.playableAsset = timelines[1];

        Bind(playableDirector, "Akate Animations", player);
        Bind(playableDirector, "Akate Movement", player);

        playableDirector.Play();

        while (playableDirector.state == PlayState.Playing)
        {
            yield return null;
        }

        #endregion

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        GameManager.instance.GetDialogueManager().RunScreenOverlayDialogue(dialogues[0], 0, 2);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        StartCoroutine(enemy.GetComponent<DamageReceiver>().SetImmune(0));

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        #region Check If Jumped Both Heights

        ActivatePlayer(player);
        player.GetComponent<PlayerAttackController>().enabled = false;
        GameManager.instance.GetPlayer().GetComponent<PlayerMovementController>().OnlyAllowActions(true);

        shortJumpPlatform.transform.parent.gameObject.SetActive(true);

        while (!(shortJumpPlatform.IsActive && player.GetComponent<Mover>().IsGrounded() && 
            !player.GetComponent<PlayerMovementController>().IsJumpingUp()))
            yield return null;

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        shortJumpPlatform.transform.parent.gameObject.SetActive(false);

        while (!player.GetComponent<Mover>().IsGrounded())
            yield return null;

        longJumpPlatform.transform.parent.gameObject.SetActive(true);

        while (!(longJumpPlatform.IsActive && player.GetComponent<Mover>().IsGrounded() &&
            !player.GetComponent<PlayerMovementController>().IsJumpingUp()))
            yield return null;

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        longJumpPlatform.transform.parent.gameObject.SetActive(false);

        DeactivatePlayer(player);

        while (!player.GetComponent<Mover>().IsGrounded())
            yield return null;

        #endregion

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        GameManager.instance.GetDialogueManager().RunScreenOverlayDialogue(dialogues[0], 3, 3);

        while (GameManager.instance.GetDialogueManager().IsRunning)
        {
            yield return null;
        }

        yield return new WaitForSeconds(Config.LARGE_DELAY);

        #region Third Cutscene - Jump and Wait to Attack

        playableDirector.playableAsset = timelines[2];

        Bind(playableDirector, "Akate Animations", player);
        Bind(playableDirector, "Akate Movement", player);

        playableDirector.Play();

        #endregion

        yield return new WaitForSeconds(.7f);

        GameManager.instance.GetAnimationManager().ShowImageUIWorldSpace(slashDownKeys,
            new Vector3(player.transform.position.x + .075f, player.transform.position.y + .4f, player.transform.position.z));

        while (!(Input.GetKeyDown(KeyCode.X) && Input.GetKey(KeyCode.DownArrow)))
        {
            yield return null;
        }

        GameManager.instance.GetAnimationManager().ClearWorldSpaceCanvas();

        #region Fourth Cutscene - Enemy Killed and Leaving

        playableDirector.playableAsset = timelines[3];

        Bind(playableDirector, "Akate Animations", player);
        Bind(playableDirector, "Akate Movement", player);

        playableDirector.Play();

        #endregion
    }
}
