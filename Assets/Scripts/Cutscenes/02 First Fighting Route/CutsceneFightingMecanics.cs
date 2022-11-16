using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneFightingMecanics : Cutscene
{
    private GameObject player;
    [SerializeField] private GameObject enemy;

    [SerializeField] BoxCollider arrowCollider;
    private bool arrowHasCollided = false;

    protected override void Start()
    {
        base.Start();

        player = GameManager.instance.GetPlayer();
        player.GetComponent<PlayerMovementController>().enabled = false;
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

        while (!Input.GetKeyDown(KeyCode.X))
        {
            yield return null;
        }

        GameManager.instance.GetAnimationManager().ClearWorldSpaceCanvas();

        yield return new WaitForSeconds(Config.BIG_DELAY);

        #region End Cutscene

        playableDirector.playableAsset = timelines[0];

        Bind(playableDirector, "Akate Animations", player);
        Bind(playableDirector, "Akate Movement", player);

        playableDirector.Play();

        while (playableDirector.state == PlayState.Playing)
        {
            yield return null;
        }

        #endregion

        player.GetComponent<PlayerMovementController>().enabled = true;

    }
}
