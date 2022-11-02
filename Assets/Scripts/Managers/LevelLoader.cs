using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator crossFade;

    private float startTransitionDuration = Config.START_TRANSITION_DURATION;
    private float endTransitionDuration = Config.END_TRANSITION_DURATION;
    private string lastTransitionType;

    public void LoadLevel(string sceneName, string transitionType)
    {
        StartCoroutine(LoadLevelAndAnimate(sceneName, transitionType));
    }

    private IEnumerator LoadLevelAndAnimate(string sceneName, string transitionType)
    {
        switch (transitionType)
        {
            case Config.CROSSFADE_TRANSITION:
                CrossfadeStart();
                break;
        }

        lastTransitionType = transitionType;

        yield return new WaitForSeconds(startTransitionDuration);

        GameManager.instance.SetGamePaused(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void FinishTransition()
    {
        if (lastTransitionType != null)
        {
            GameManager.instance.SetGamePaused(false);

            switch (lastTransitionType)
            {
                case Config.CROSSFADE_TRANSITION:
                    StartCoroutine(CrossfadeEnd());
                    break;
            }
        }
    }

    public void CrossfadeStart()
    {
        crossFade.gameObject.SetActive(true);
        crossFade.SetTrigger(Config.CROSSFADE_START_TRIGGER);
    }

    public IEnumerator CrossfadeEnd()
    {
        crossFade.SetTrigger(Config.CROSSFADE_END_TRIGGER);

        SetPlayerVariablesAfterTransition();

        yield return new WaitForSeconds(endTransitionDuration);

        crossFade.gameObject.SetActive(false);

        yield return new WaitForSeconds(Config.MEDIUM_DELAY);

        GameManager.instance.SetIsTeleporting(false);
    }

    private void SetPlayerVariablesAfterTransition()
    {
        GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = true;
        GameManager.instance.GetPlayer().transform.localScale = Vector3.one;
        GameManager.instance.RestartPlayer();

        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
