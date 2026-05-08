using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSPGen : MonoBehaviour
{
    [SerializeField] private int width = 48;
    [SerializeField] private int height = 48;

    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [ContextMenu("Generate Dungeon")]

    private void GenerateDungeon()
    {

    }
}
