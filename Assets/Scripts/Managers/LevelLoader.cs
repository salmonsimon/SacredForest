using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator crossFade;

    private float startTransitionTime, endTransitionTime = .5f;
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
                crossFade.gameObject.SetActive(true);
                crossFade.SetTrigger("Start");
                break;
        }

        lastTransitionType = transitionType;

        yield return new WaitForSeconds(.5f);

        GameManager.instance.SetGamePaused(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public IEnumerator FinishTransition()
    {
        GameManager.instance.SetGamePaused(false);

        GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        switch (lastTransitionType)
        {
            case Config.CROSSFADE_TRANSITION:
                crossFade.SetTrigger("End");
                yield return new WaitForSeconds(endTransitionTime * 2);
                crossFade.gameObject.SetActive(false);
                break;
        }
    }
}
