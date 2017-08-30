using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName ="2D Level/TileSet")]
public class TileSet: ScriptableObject
{
    [SerializeField]
    public List<GameObject> Tiles;
}