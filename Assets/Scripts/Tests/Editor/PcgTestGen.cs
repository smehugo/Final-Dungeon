using System.Collections.Generic;
using UnityEngine;

// test generator
public static class PcgTestGen
{
    public struct Data
    {
        public int seed;
        public List<BSPNode> leaves;
        public List<DungeonRoom> rooms;
        public List<Vector2Int> roomCenters;
        public HashSet<Vector2Int> roomTiles;
        public HashSet<Vector2Int> corridorTiles;
        public HashSet<Vector2Int> floorTiles;
        public HashSet<Vector2Int> blockedTiles;
        public List<RoomEdge> allEdges;
        public List<RoomEdge> mstBaseEdges;
        public List<RoomEdge> mstEdges;
        public List<RectInt> corridors;
        public DungeonMapData mapData;
        public int roomsCreated;
        public int corridorFailures;
    }

    public static Data Generate(int seed)
    {
        return GenerateFresh(seed);
    }

    public static Data GenerateFresh(int seed)
    {
        var config = new DungeonPipeline.PipelineConfig
        {
            mapWidth = DungeonGenConfig.MapWidth,
            mapHeight = DungeonGenConfig.MapHeight,
            roomPadding = DungeonGenConfig.RoomPadding,
            roomCount = DungeonGenConfig.RoomCount,
            minRoomSize = DungeonGenConfig.MinRoomSize,
            maxDepth = DungeonGenConfig.MaxDepth,
            roomFillMin = DungeonGenConfig.RoomFillMin,
            roomFillMax = DungeonGenConfig.RoomFillMax,
            minZoneSize = DungeonGenConfig.MinZoneSize,
            maxZoneSize = DungeonGenConfig.MaxZoneSize,
            interiorDepthStep = DungeonGenConfig.InteriorDepthStep,
            interiorMaxDepth = DungeonGenConfig.InteriorMaxDepth,
            wallOpeningMin = DungeonGenConfig.WallOpeningMin,
            wallOpeningMax = DungeonGenConfig.WallOpeningMax,
            wallExtraHoleGamba = DungeonGenConfig.WallExtraHoleGamba,
            artifactZones = DungeonGenConfig.ArtifactZones
        };

        var result = DungeonPipeline.Run(config, seed);

        var corridorTiles = new HashSet<Vector2Int>();
        foreach (var c in result.corridors)
            for (int x = c.xMin; x < c.xMax; x++)
                for (int y = c.yMin; y < c.yMax; y++)
                    corridorTiles.Add(new Vector2Int(x, y));

        return new Data
        {
            seed = seed,
            leaves = result.leaves,
            rooms = result.rooms,
            roomCenters = result.roomCenters,
            roomTiles = result.roomTiles,
            corridorTiles = corridorTiles,
            floorTiles = result.floorTiles,
            blockedTiles = result.blockedTiles,
            allEdges = result.allEdges,
            mstBaseEdges = result.mstBaseEdges,
            mstEdges = result.mstEdges,
            corridors = result.corridors,
            mapData = new DungeonMapData(result.rooms, result.floorTiles, result.blockedTiles),
            roomsCreated = result.roomsCreated,
            corridorFailures = result.corridorFailures
        };
    }
}
