using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class BSPGen : MonoBehaviour
{
    [SerializeField] private int MapWidth = 48;
    [SerializeField] private int MapHeight = 48;
    [SerializeField] private int roomPadding;
    [SerializeField] private int roomCount = 10;


    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [SerializeField] private int minRoomSize = 8;
    [SerializeField] private int maxDepth = 5;
    [SerializeField] private float roomFillMin = 0.5f;
    [SerializeField] private float roomFillMax = 0.9f;

    private int MinLeafSize => minRoomSize + 2 * roomPadding;
    private int currentLeafCount;

    private BSPNode rootNode;
    private List<RoomEdge> allEdges = new List<RoomEdge>();
    private List<Vector2Int> roomCenterPoints = new List<Vector2Int>();
    private List<RoomEdge> mstEdges = new List<RoomEdge>();
    private int[] MSTparent;
    private int[] MSTrank;
    private List<RectInt> corridors = new List<RectInt>();
    private HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();

    private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

    [ContextMenu("Generate Dungeon")]

    private void GenerateDungeon()
    {
        rootNode = new BSPNode(new RectInt(0, 0, MapWidth, MapHeight));
        currentLeafCount = 1;
        Split(rootNode, 0);

        //collect the final rects
        List<BSPNode> leaves = new List<BSPNode>();
        GetLeaves(rootNode, leaves);
        Debug.Log($"leaves: {leaves.Count}");
        MakeRooms(leaves);

        roomTiles.Clear();
        foreach (var leaf in leaves)
        {
            if (leaf.hasRoom)
            {
                for (int x = leaf.roomRect.xMin; x < leaf.roomRect.xMax; x++)
                {
                    for (int y = leaf.roomRect.yMin; y < leaf.roomRect.yMax; y++)
                    {
                        roomTiles.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        //MST
        GetRoomCenters();
        BuildEdgeList();
        BuildMST();
        BuildMSTSecondPass();
        BuildCorridors();
    }
}
