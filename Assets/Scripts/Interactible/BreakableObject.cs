using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageReceiver), typeof(Animator))]
public class BreakableObject : MonoBehaviour
{
    private Animator animator;

    [SerializeField] float breakAnimationDuration = .6f;
    [SerializeField] bool destroyAfterBreaking = true;

    private void Start()
    {
        animator = GetComponent<Animator>();

        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += Death;
    }

    private void Death()
    {
        animator.SetTrigger(Config.ANIMATOR_DEATH_TRIGGER);
         
        if (destroyAfterBreaking)
            StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(breakAnimationDuration);

        Destroy(gameObject);
    }
}
