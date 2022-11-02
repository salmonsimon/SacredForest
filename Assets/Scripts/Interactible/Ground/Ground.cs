using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Ground : MonoBehaviour
{
    [SerializeField] private GroundType groundType;
    private enum GroundType
    {
        Grass,
        Rock,
        Wood,
        Dirt
    }

    public string GetGroundType()
    {
        return groundType.ToString() + " Floor";
    }
}
