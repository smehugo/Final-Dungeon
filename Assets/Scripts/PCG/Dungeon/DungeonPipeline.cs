using UnityEngine;
using System.Collections.Generic;

// shared generation pipeline merged from BSPGen and PcgTestGen so both run same pipeline
public static class DungeonPipeline
{
    [System.Serializable]
    public class PipelineConfig
    {
        public int mapWidth;
        public int mapHeight;
        public int roomPadding;
        public int roomCount;
        public int minRoomSize;
        public int maxDepth;
        public float roomFillMin;
        public float roomFillMax;
        public int minZoneSize;
        public int maxZoneSize;
        public int interiorDepthStep;
        public int interiorMaxDepth;
        public int wallOpeningMin;
        public float wallOpeningMax;
        public float wallExtraHoleGamba;
        public int artifactZones;
    }

    public class PipelineResult
    {
        public BSPNode rootNode;
        public List<BSPNode> leaves;
        public List<DungeonRoom> rooms;
        public List<Vector2Int> roomCenters;
        public HashSet<Vector2Int> roomTiles;
        public List<RoomEdge> allEdges;
        public List<RoomEdge> mstBaseEdges;
        public List<RoomEdge> mstEdges;
        public List<RectInt> corridors;
        public int corridorFailures;
        public int roomsCreated;
        public HashSet<Vector2Int> floorTiles;
        public HashSet<Vector2Int> blockedTiles;
    }

    public static PipelineResult Run(PipelineConfig config, int seed)
    {
        Random.InitState(seed);

        var splitter = new BSPSplitter();
        var roomBuilder = new BSPRooms();
        var graphBuilder = new GraphBuilder();
        var corridorBuilder = new CorridorBuilder();
        var finStartAssigner = new AssignFinStartRooms();
        var interiorGenerator = new RoomInteriorGen();

        int minLeafSize = config.minRoomSize + 2 * config.roomPadding;
        var rootNode = new BSPNode(new RectInt(0, 0, config.mapWidth, config.mapHeight));
        int leafCount = 1;
        splitter.Split(rootNode, 0, config.maxDepth, config.roomCount, minLeafSize, ref leafCount);

        var leaves = new List<BSPNode>();
        splitter.GetLeaves(rootNode, leaves);

        int roomsCreated = 0;
        roomBuilder.MakeRooms(leaves, config.roomPadding, config.minRoomSize, config.roomFillMin, config.roomFillMax, ref roomsCreated);

        var roomTiles = new HashSet<Vector2Int>();
        foreach (var leaf in leaves)
        {
            if (!leaf.hasRoom) continue;
            for (int x = leaf.roomRect.xMin; x < leaf.roomRect.xMax; x++)
                for (int y = leaf.roomRect.yMin; y < leaf.roomRect.yMax; y++)
                    roomTiles.Add(new Vector2Int(x, y));
        }

        var roomCenters = new List<Vector2Int>();
        var rooms = new List<DungeonRoom>();
        var tileToRoom = new Dictionary<Vector2Int, int>();
        roomBuilder.GetRoomCenters(rootNode, roomCenters, rooms, tileToRoom);

        var allEdges = new List<RoomEdge>();
        var mstEdges = new List<RoomEdge>();
        var corridors = new List<RectInt>();
        graphBuilder.BuildEdgeList(rooms, roomCenters, allEdges);
        graphBuilder.BuildMST(allEdges, roomCenters, mstEdges);

        var mstBaseEdges = new List<RoomEdge>(mstEdges);
        graphBuilder.BuildMSTSecondPass(roomCenters, mstEdges, allEdges);

        int corridorFailures = corridorBuilder.BuildCorridors(mstEdges, rooms, roomTiles, corridors);
        corridorBuilder.BuildReservedTiles(rooms);
        finStartAssigner.AssignSpecialRooms(rooms, config.artifactZones);

        interiorGenerator.BuildRoomInteriors(
            rooms,
            config.minZoneSize,
            config.maxZoneSize,
            config.interiorDepthStep,
            config.interiorMaxDepth,
            config.wallOpeningMin,
            config.wallOpeningMax,
            config.wallExtraHoleGamba,
            config.artifactZones);

        HashSet<Vector2Int> floorTiles = DeriveFloorTiles(roomTiles, corridors);
        HashSet<Vector2Int> blockedTiles = DeriveBlockedTiles(rooms, floorTiles);

        return new PipelineResult
        {
            rootNode = rootNode,
            leaves = leaves,
            rooms = rooms,
            roomCenters = roomCenters,
            roomTiles = roomTiles,
            allEdges = allEdges,
            mstBaseEdges = mstBaseEdges,
            mstEdges = mstEdges,
            corridors = corridors,
            corridorFailures = corridorFailures,
            roomsCreated = roomsCreated,
            floorTiles = floorTiles,
            blockedTiles = blockedTiles
        };
    }

    private static HashSet<Vector2Int> DeriveFloorTiles(HashSet<Vector2Int> roomTiles, List<RectInt> corridors)
    {
        var floorTiles = new HashSet<Vector2Int>(roomTiles);
        foreach (var c in corridors)
            for (int x = c.xMin; x < c.xMax; x++)
                for (int y = c.yMin; y < c.yMax; y++)
                    floorTiles.Add(new Vector2Int(x, y));

        return floorTiles;
    }

    private static HashSet<Vector2Int> DeriveBlockedTiles(List<DungeonRoom> rooms, HashSet<Vector2Int> floorTiles)
    {
        var blocked = new HashSet<Vector2Int>();
        foreach (var room in rooms)
            foreach (var wall in room.interiorWalls)
                foreach (var tile in wall.tiles)
                    blocked.Add(tile);

        foreach (var tile in floorTiles)
            for (int x = tile.x - 1; x <= tile.x + 1; x++)
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    var w = new Vector2Int(x, y);
                    if (!floorTiles.Contains(w)) blocked.Add(w);
                }

        return blocked;
    }
}