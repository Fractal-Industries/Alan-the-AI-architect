using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Piece",menuName = "Piece")]
public class PieceDef : ScriptableObject
{
    public Vector3[] LocalPoint;
    public Vector3[] Normal;
}
