using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private DamageReceiver damageReceiver;

    private void Awake()
    {
        damageReceiver = GetComponent<DamageReceiver>();
    }

    /*
    private void Start()
    {
        GetComponent<DamageReceiver>().OnCharacterAliveStatusChange += AliveStatusChange;
        GetComponent<DamageReceiver>().OnCharacterDamaged += Damaged;
    }
    */

    public void SetHealth()
    {
        DifficultyLevel difficultyLevel = GameManager.instance.GetCurrentProgressManager().CurrentDifficultyLevel;

        switch (difficultyLevel)
        {
            case DifficultyLevel.Normal:
                damageReceiver.SetHealth(1);
                break;

            case DifficultyLevel.Easy:
                damageReceiver.SetHealth(2);
                break;

            case DifficultyLevel.Baby:
                damageReceiver.SetHealth(3);
                break;
        }
    }

    /*
    protected virtual void Damaged()
    {
        if (damageReceiver.IsAlive)
        {
            StartCoroutine(GetComponent<DamageReceiver>().ImmuneCooldown());
        }
    }
    */

}
