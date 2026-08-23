using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BSPGen : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] private int MapWidth = DungeonGenConfig.MapWidth;
    [SerializeField] private int MapHeight = DungeonGenConfig.MapHeight;
    [SerializeField] private int roomPadding = DungeonGenConfig.RoomPadding;
    [SerializeField] private int roomCount = DungeonGenConfig.RoomCount;

    // TODO: clean this up when done!!!
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase corridorTile;

    [Header("Seed Settings")]
    [SerializeField] private int seed = 1;
    public int CurrentSeed { get; private set; }

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

    [SerializeField] private int minRoomSize = DungeonGenConfig.MinRoomSize;
    [SerializeField] private int maxDepth = DungeonGenConfig.MaxDepth;
    [SerializeField] private float roomFillMin = DungeonGenConfig.RoomFillMin;
    [SerializeField] private float roomFillMax = DungeonGenConfig.RoomFillMax;

    [Header("Interior Settings")]
    [SerializeField] private int minZoneSize = DungeonGenConfig.MinZoneSize;
    [SerializeField] private int maxZoneSize = DungeonGenConfig.MaxZoneSize;
    [SerializeField] private int interiorDepthStep = DungeonGenConfig.InteriorDepthStep;
    [SerializeField] private int interiorMaxDepth = DungeonGenConfig.InteriorMaxDepth;
    [SerializeField] private int wallOpeningMin = DungeonGenConfig.WallOpeningMin;
    [SerializeField] private float wallOpeningMax = DungeonGenConfig.WallOpeningMax;
    [SerializeField] private float wallExtraHoleGamba = DungeonGenConfig.WallExtraHoleGamba;
    [SerializeField] private int artifactZones = DungeonGenConfig.ArtifactZones;

    [Header("Map Data")]
    [SerializeField] private DungeonContentPlacer contentPlacer;
    [SerializeField] private FinalRoomSeal finalRoomSeal;

    // private int MinLeafSize => minRoomSize + 2 * roomPadding;
    // private int currentLeafCount;

    private BSPNode rootNode;
    private List<RoomEdge> allEdges = new List<RoomEdge>();
    private List<Vector2Int> roomCenterPoints = new List<Vector2Int>();
    private List<RoomEdge> mstEdges = new List<RoomEdge>();
    private List<RectInt> corridors = new List<RectInt>();
    private HashSet<Vector2Int> roomTiles = new HashSet<Vector2Int>();

    private List<DungeonRoom> dungeonRooms = new List<DungeonRoom>();

    // private Dictionary<Vector2Int, int> tileToRoom = new Dictionary<Vector2Int, int>();
    private Dictionary<Vector2Int, FloorTheme> floorThemeByTile = new();
    // private HashSet<Vector2Int> debugLastFlood = new HashSet<Vector2Int>();

    public int roomsCreated = 0;

    // private BSPSplitter splitter = new BSPSplitter();
    // private BSPRooms roomBuilder = new BSPRooms();
    // private GraphBuilder graphBuilder = new GraphBuilder();
    // private CorridorBuilder corridorBuilder = new CorridorBuilder();
    // private RoomInteriorGen interiorGenerator = new RoomInteriorGen();
    private BuildTilemap tilemapBuilder = new BuildTilemap();
    // private AssignFinStartRooms finStartAssigner = new AssignFinStartRooms();

    // debug getters for DebugObject
    public BSPNode DebugRootNode => rootNode;
    public List<DungeonRoom> DebugDungeonRooms => dungeonRooms;
    public List<Vector2Int> DebugRoomCenters => roomCenterPoints;
    public List<RoomEdge> DebugAllEdges => allEdges;
    public List<RoomEdge> DebugMstEdges => mstEdges;
    public List<RectInt> DebugCorridors => corridors;
    public HashSet<Vector2Int> DebugFinalFloorTiles => finalFloorTiles;
    public HashSet<Vector2Int> DebugFinalCorridorTiles => finalCorridorTiles;
    public HashSet<Vector2Int> DebugFinalWallTiles => finalWallTiles;
    public HashSet<Vector2Int> DebugBlockedTiles => blockedTiles;

    public DungeonMapData MapData { get; private set; }

    private List<Vector2Int> debugPath = new();

    public List<Vector2Int> DebugPath => debugPath;

    private void Start()
    {
        GenerateDungeon();
    }

    [ContextMenu("Generate Dungeon")]

    private void GenerateDungeon()
    {
        if (RunConfig.UseCustomGen)
        {
            MapWidth = RunConfig.MapSize;
            MapHeight = RunConfig.MapSize;
            roomCount = RunConfig.RoomCount;
            artifactZones = RunConfig.Artifacts;
            roomFillMin = RunConfig.RoomFill;
        }

        if (RunConfig.UseFixedSeed)
        {
            seed = RunConfig.Seed;
        }
        else
        {
            seed = Random.Range(0, int.MaxValue);
        }

        Random.InitState(seed);
        CurrentSeed = seed;
        Debug.Log($"seed {seed} difficulty {RunConfig.DiffName} map {MapWidth} rooms {roomCount} artifacts {artifactZones}");

        var pipelineConfig = new DungeonPipeline.PipelineConfig
        {
            mapWidth = MapWidth,
            mapHeight = MapHeight,
            roomPadding = roomPadding,
            roomCount = roomCount,
            minRoomSize = minRoomSize,
            maxDepth = maxDepth,
            roomFillMin = roomFillMin,
            roomFillMax = roomFillMax,
            minZoneSize = minZoneSize,
            maxZoneSize = maxZoneSize,
            interiorDepthStep = interiorDepthStep,
            interiorMaxDepth = interiorMaxDepth,
            wallOpeningMin = wallOpeningMin,
            wallOpeningMax = wallOpeningMax,
            wallExtraHoleGamba = wallExtraHoleGamba,
            artifactZones = artifactZones
        };

        DungeonPipeline.PipelineResult result = DungeonPipeline.Run(pipelineConfig, seed);

        rootNode = result.rootNode;
        roomsCreated = result.roomsCreated;
        dungeonRooms = result.rooms;
        roomCenterPoints = result.roomCenters;
        allEdges = result.allEdges;
        mstEdges = result.mstEdges;
        corridors = result.corridors;
        roomTiles = result.roomTiles;
        int corridorFailures = result.corridorFailures;

        tilemapBuilder.BuildAllTM(floorTilemap, wallTilemap, floorTile, wallTile, corridorTile, stoneTile, woodTile, metalTile, dirtTile, carpetTile, demonicTile, roomTiles, corridors, dungeonRooms, finalFloorTiles, finalCorridorTiles, finalWallTiles, blockedTiles, floorThemeByTile);

        MapData = new DungeonMapData(dungeonRooms, finalFloorTiles, blockedTiles);
        if (contentPlacer != null)
        {
            contentPlacer.PlaceContent(MapData);
        }

        {
            finalRoomSeal.BuildSeal(MapData);
        }

        ValidateLayout(MapData, corridorFailures);

        UpdateDebugPath();
    }

    // global validation: flood from start, every room/door reached, no corridor failures
    private void ValidateLayout(DungeonMapData mapData, int corridorFailures)
    {
        if (corridorFailures > 0)
            Debug.LogError($"seed {seed} validation corridor failed {corridorFailures}");

        DungeonRoom start = null;
        foreach (var room in mapData.rooms)
        {
            if (room.isStartRoom)
            {
                start = room;
                break;
            }
        }

        if (start == null)
        {
            Debug.LogError($"seed {seed} no start room");
            return;
        }

        if (!GetWalkableTile(mapData, start, out Vector2Int startTile))
        {
            Debug.LogError($"seed {seed} start room blocked");
            return;
        }

        HashSet<Vector2Int> reached = Reachability.FloodFill(mapData, startTile);

        foreach (var room in mapData.rooms)
        {
            if (!GetWalkableTile(mapData, room, out Vector2Int tile))
            {
                Debug.LogError($"seed {seed} room {room.id} blocked");
                continue;
            }

            if (!reached.Contains(tile))
            {
                string tag = room.hasArtifact ? "artifact room" : "room";
                Debug.LogError($"seed {seed} {tag} {room.id} cant reach from start");
            }

            foreach (var door in room.doors)
            {
                if (!reached.Contains(door.position))
                    Debug.LogError($"seed {seed} door {door.position} in room {room.id} cant reach from start");
            }
        }
    }

    private bool GetWalkableTile(DungeonMapData mapData, DungeonRoom room, out Vector2Int result)
    {
        for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
        {
            for (int y = room.bounds.yMin; y < room.bounds.yMax; y++)
            {
                Vector2Int tile = new Vector2Int(x, y);
                if (mapData.IsWalkable(tile))
                {
                    result = tile;
                    return true;
                }
            }
        }

        result = default;
        return false;
    }

    // for gizmo path draw
    private void UpdateDebugPath()
    {
        debugPath.Clear();
        if (MapData == null || MapData.rooms.Count < 2 || MapData.rooms[1].doors.Count == 0)
            return;

        var path = BFS.FindPath(MapData, MapData.rooms[1].center, MapData.rooms[1].doors[0].position, MapData.rooms[1].bounds);
        if (path != null)
            debugPath.AddRange(path);
    }
}