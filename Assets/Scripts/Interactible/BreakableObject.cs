using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageReceiver), typeof(Animator))]
public class BreakableObject : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += Death;
    }

    private void Death()
    {
        animator.SetTrigger(Config.ANIMATOR_DEATH_TRIGGER);
         
        StartCoroutine(WaitToDestroy());
    }

    private IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(.6f);

        Destroy(gameObject);
    }
}
