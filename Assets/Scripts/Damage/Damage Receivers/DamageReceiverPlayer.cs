using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiverPlayer : DamageReceiver
{
    protected override void Death()
    {
        base.Death();

        attachedCharacter.GetComponent<PlayerMovementController>().Death();
        attachedCharacter.GetComponent<PlayerAttackController>().Death();
    }
}
