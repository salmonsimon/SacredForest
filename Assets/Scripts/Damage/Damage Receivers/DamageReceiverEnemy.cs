using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiverEnemy : DamageReceiver
{
    protected override void Death()
    {
        base.Death();

        attachedCharacter.GetComponent<EnemyMover>().Death();
    }
}
