using System.Collections.Generic;
using UnityEngine;

// test generator
public static class PcgTestGen
{
    public struct Data
    {
        public List<BSPNode> leaves;
        public List<DungeonRoom> rooms;
        public HashSet<Vector2Int> roomTiles;
        public HashSet<Vector2Int> corridorTiles;
        public HashSet<Vector2Int> floorTiles;
        public HashSet<Vector2Int> blockedTiles;
        public List<RoomEdge> allEdges;
        public List<RoomEdge> mstEdges;
        public List<RectInt> corridors;
        public DungeonMapData mapData;
    }

    public static Data Generate(int seed)
    {
        Random.InitState(seed);

        // split rooms graph corridors assign
        var splitter = new BSPSplitter();
        var roomBuilder = new BSPRooms();
        var graphBuilder = new GraphBuilder();
        var corridorBuilder = new CorridorBuilder();
        var finStart = new AssignFinStartRooms();

        var root = new BSPNode(new RectInt(0, 0, DungeonGenConfig.MapWidth, DungeonGenConfig.MapHeight));
        int leafCount = 1;
        splitter.Split(root, 0, DungeonGenConfig.MaxDepth, DungeonGenConfig.RoomCount, DungeonGenConfig.MinLeafSize, ref leafCount);

        var leaves = new List<BSPNode>();
        splitter.GetLeaves(root, leaves);

        int roomsCreated = 0;
        roomBuilder.MakeRooms(leaves, DungeonGenConfig.RoomPadding, DungeonGenConfig.MinRoomSize, DungeonGenConfig.RoomFillMin, DungeonGenConfig.RoomFillMax, ref roomsCreated);

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
        roomBuilder.GetRoomCenters(root, roomCenters, rooms, tileToRoom);

        var allEdges = new List<RoomEdge>();
        var mstEdges = new List<RoomEdge>();
        var corridors = new List<RectInt>();
        graphBuilder.BuildEdgeList(rooms, roomCenters, allEdges);
        graphBuilder.BuildMST(allEdges, roomCenters, mstEdges);
        // skip 2nd-pass
        corridorBuilder.BuildCorridors(mstEdges, rooms, roomTiles, corridors);
        // assign start/final
        finStart.AssignSpecialRooms(rooms, DungeonGenConfig.ArtifactZones);

        var corridorTiles = new HashSet<Vector2Int>();
        var floorTiles = new HashSet<Vector2Int>(roomTiles);
        foreach (var c in corridors)
        {
            for (int x = c.xMin; x < c.xMax; x++)
                for (int y = c.yMin; y < c.yMax; y++)
                {
                    var tile = new Vector2Int(x, y);
                    corridorTiles.Add(tile);
                    floorTiles.Add(tile);
                }
        }

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

        return new Data
        {
            leaves = leaves,
            rooms = rooms,
            roomTiles = roomTiles,
            corridorTiles = corridorTiles,
            floorTiles = floorTiles,
            blockedTiles = blocked,
            allEdges = allEdges,
            mstEdges = mstEdges,
            corridors = corridors,
            mapData = new DungeonMapData(rooms, floorTiles, blocked)
        };
    }
}
