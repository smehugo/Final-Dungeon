using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class BSPGen : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int MapWidth = 48;
    [SerializeField] private int MapHeight = 48;
    [SerializeField] private int roomPadding;
    [SerializeField] private int roomCount = 10;

    // TODO: clean this up when done!!!
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase corridorTile;

    [Header("Floor Theme Tiles")]
    [SerializeField] private TileBase stoneTile;
    [SerializeField] private TileBase woodTile;
    [SerializeField] private TileBase metalTile;
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase carpetTile;
    [SerializeField] private TileBase demonicTile;

    private HashSet<Vector2Int> finalFloorTiles = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> finalCorridorTiles = new();
    private HashSet<Vector2Int> finalWallTiles = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> blockedTiles = new HashSet<Vector2Int>();

    [SerializeField] private int minRoomSize = 8;
    [SerializeField] private int maxDepth = 5;
    [SerializeField] private float roomFillMin = 0.5f;
    [SerializeField] private float roomFillMax = 0.9f;

    [Header("Interior Settings")]
    [SerializeField] private int minZoneSize = 8;
    [SerializeField] private int maxZoneSize = 20;
    [SerializeField] private int interiorDepthStep = 12;
    [SerializeField] private int interiorMaxDepth = 4;
    [SerializeField] private int wallOpeningMin = 3;
    [SerializeField] private float wallOpeningMax = 0.3f;
    [SerializeField] private float wallExtraHoleGamba = 0.05f;
    [SerializeField] private int artifactZones = 5;

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

    private Dictionary<Vector2Int, int> tileToRoom = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, FloorTheme> floorThemeByTile = new();
    // private HashSet<Vector2Int> debugLastFlood = new HashSet<Vector2Int>();

    [ContextMenu("Generate Dungeon")]

    private void GenerateDungeon()
    {
        rootNode = new BSPNode(new RectInt(0, 0, MapWidth, MapHeight));
        currentLeafCount = 1;
        Split(rootNode, 0);

        //collect the final rects
        List<BSPNode> leaves = new List<BSPNode>();
        GetLeaves(rootNode, leaves);
        // Debug.Log($"leaves: {leaves.Count}");
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
        BuildReservedTiles();
        BuildRoomInteriors();
        BuildAllTM();
    }
}
