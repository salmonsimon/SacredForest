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
                crossFade.gameObject.SetActive(true);
                crossFade.SetTrigger(Config.CROSSFADE_START_TRIGGER);
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

    public IEnumerator FinishTransition()
    {
        if (lastTransitionType != null)
        {
            GameManager.instance.SetGamePaused(false);
            GameManager.instance.SetIsTeleporting(false);
            
            GameManager.instance.GetPlayer().GetComponent<Animator>().enabled = true;
            GameManager.instance.GetPlayer().transform.localScale = Vector3.one;
            GameManager.instance.GetPlayer().transform.rotation = Quaternion.identity;


            GameManager.instance.GetPlayer().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

            switch (lastTransitionType)
            {
                case Config.CROSSFADE_TRANSITION:
                    crossFade.SetTrigger(Config.CROSSFADE_END_TRIGGER);
                    yield return new WaitForSeconds(endTransitionDuration);
                    crossFade.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
