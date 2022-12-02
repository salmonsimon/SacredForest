using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementController), typeof(PlayerAttackController), typeof(DamageReceiver))]
public class Player : MonoBehaviour
{
    private DamageReceiver damageReceiver;

    private PlayerAttackController playerAttackController;
    private PlayerMovementController playerMovementController;

    private void Awake()
    {
        damageReceiver = GetComponent<DamageReceiver>();

        playerAttackController = GetComponent<PlayerAttackController>();
        playerMovementController = GetComponent<PlayerMovementController>();
    }

    private void OnEnable()
    {
        StartCoroutine(GetComponent<DamageReceiver>().SetImmune(0f));

        damageReceiver.OnCharacterAliveStatusChange += AliveStatusChange;
        damageReceiver.OnCharacterDamaged += Damaged;
    }

    private void OnDisable()
    {
        damageReceiver.OnCharacterAliveStatusChange -= AliveStatusChange;
        damageReceiver.OnCharacterDamaged -= Damaged;
    }

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

    protected virtual void Damaged()
    {
        if (damageReceiver.IsAlive)
        {
            StartCoroutine(GetComponent<DamageReceiver>().ImmuneCooldown());
        }
    }

    private void AliveStatusChange()
    {
        if (damageReceiver.IsAlive)
        {
            playerAttackController.Resurrection();
            playerMovementController.Resurrection();
        }
        else
        {
            GameManager.instance.GetCurrentProgressManager().IncreaseDeathsCount();

            playerMovementController.Death();

            StartCoroutine(GameManager.instance.GetDialogueManager().ShowRandomDeathDialogue());
        }
    }

}
