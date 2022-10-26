using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacks : MonoBehaviour
{
    protected bool isAttacking = false;

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }
}
