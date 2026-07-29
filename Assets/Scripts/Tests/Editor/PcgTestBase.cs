using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

// helpers used by the special-room tests
public class PcgTestBase
{
    protected static RectInt MapBounds
    {
        get { return new RectInt(0, 0, DungeonGenConfig.MapWidth, DungeonGenConfig.MapHeight); }
    }


    protected static DungeonRoom StartRoom(PcgTestGen.Data data)
    {
        foreach (var room in data.rooms)
        {
            if (room.isStartRoom)
            {
                return room;
            }
        }
        return null;
    }

    protected static DungeonRoom FinalRoom(PcgTestGen.Data data)
    {
        foreach (var room in data.rooms)
        {
            if (room.isFinalRoom)
            {
                return room;
            }
        }
        return null;
    }

    protected static bool TryGetWalkaleTile(PcgTestGen.Data data, DungeonRoom room, out Vector2Int result)
    {
        for (int x = room.bounds.xMin; x < room.bounds.xMax; x++)
            for (int y = room.bounds.yMin; y < room.bounds.yMax; y++)
            {
                var tile = new Vector2Int(x, y);
                if (data.mapData.IsWalkable(tile))
                {
                    result = tile;
                    return true;
                }
            }

        result = default;
        return false;
    }

    protected static HashSet<Vector2Int> ReachableFromStart(PcgTestGen.Data data)
    {
        var start = StartRoom(data);
        if (start == null) return new HashSet<Vector2Int>();
        if (!TryGetWalkaleTile(data, start, out var tile)) return new HashSet<Vector2Int>();
        return Reachability.FloodFill(data.mapData, tile);
    }

    protected static void AssertPathConnected(List<Vector2Int> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            var a = path[i - 1];
            var b = path[i];
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            Assert.IsTrue((dx == 1 && dy == 0) || (dx == 0 && dy == 1),
                $"step not connected {a} -> {b}");
        }
    }

    // returns how many components the rooms fall into, union based
    protected static int CountComponents(int roomCount, List<RoomEdge> edges)
    {
        if (roomCount == 0) return 0;

        var parent = new int[roomCount];
        for (int i = 0; i < roomCount; i++) parent[i] = i;

        foreach (var edge in edges)
        {
            int rootA = FindRoot(parent, edge.a);
            int rootB = FindRoot(parent, edge.b);
            if (rootA != rootB) parent[rootA] = rootB;
        }

        int components = 0;
        for (int i = 0; i < roomCount; i++)
            if (FindRoot(parent, i) == i) components++;

        return components;
    }

    protected static bool HasCycle(int roomCount, List<RoomEdge> edges)
    {
        var parent = new int[roomCount];
        for (int i = 0; i < roomCount; i++) parent[i] = i;

        foreach (var edge in edges)
        {
            int rootA = FindRoot(parent, edge.a);
            int rootB = FindRoot(parent, edge.b);
            if (rootA == rootB) return true;
            parent[rootA] = rootB;
        }

        return false;
    }

    private static int FindRoot(int[] parent, int i)
    {
        while (parent[i] != i) i = parent[i];
        return i;
    }

    protected static bool SameEdge(RoomEdge x, RoomEdge y)
    {
        return (x.a == y.a && x.b == y.b) || (x.a == y.b && x.b == y.a);
    }

    protected static bool IsOnPerimeter(RectInt bounds, Vector2Int tile)
    {
        return tile.x == bounds.xMin || tile.x == bounds.xMax - 1
            || tile.y == bounds.yMin || tile.y == bounds.yMax - 1;
    }
}
