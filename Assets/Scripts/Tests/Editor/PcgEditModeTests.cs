using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// pcg tests
public class PcgEditModeTests
{
    [Test]
    public void RoomsCreated()
    {
        var data = PcgTestGen.Generate(1);
        Assert.Greater(data.rooms.Count, 0);
    }

    [Test]
    public void RoomsDontOverlap()
    {
        var data = PcgTestGen.Generate(1);
        for (int i = 0; i < data.rooms.Count; i++)
            for (int j = i + 1; j < data.rooms.Count; j++)
                Assert.IsFalse(data.rooms[i].bounds.Overlaps(data.rooms[j].bounds));
    }

    [Test]
    public void RoomsFitInLeaves()
    {
        var data = PcgTestGen.Generate(1);
        foreach (var leaf in data.leaves)
        {
            if (!leaf.hasRoom) continue;
            var inner = new RectInt(
                leaf.rect.xMin + DungeonGenConfig.RoomPadding,
                leaf.rect.yMin + DungeonGenConfig.RoomPadding,
                leaf.rect.width - 2 * DungeonGenConfig.RoomPadding,
                leaf.rect.height - 2 * DungeonGenConfig.RoomPadding);

            Assert.GreaterOrEqual(leaf.roomRect.xMin, inner.xMin);
            Assert.GreaterOrEqual(leaf.roomRect.yMin, inner.yMin);
            Assert.LessOrEqual(leaf.roomRect.xMax, inner.xMax);
            Assert.LessOrEqual(leaf.roomRect.yMax, inner.yMax);
        }
    }

    [Test]
    public void CorridorsGoodMultRooms()
    {
        var data = PcgTestGen.Generate(1);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");
        Assert.Greater(data.corridorTiles.Count, 0);
    }

    [Test]
    public void StartToFinPathExists()
    {
        var data = PcgTestGen.Generate(1);
        var start = FindStartRoom(data);
        var final = FindFinalRoom(data);
        Assert.IsNotNull(start);
        Assert.IsNotNull(final);

        var bounds = new RectInt(0, 0, DungeonGenConfig.MapWidth, DungeonGenConfig.MapHeight);
        var path = AStar.FindPath(data.mapData, start.center, final.center, bounds);
        Assert.IsNotNull(path);
        AssertPathAdj(path);
    }

    [Test]
    public void AStarFindPath()
    {
        var floor = new HashSet<Vector2Int>();
        var blocked = new HashSet<Vector2Int>();
        for (int x = 0; x < 5; x++)
            for (int y = 0; y < 5; y++)
                floor.Add(new Vector2Int(x, y));

        blocked.Add(new Vector2Int(2, 0));
        blocked.Add(new Vector2Int(2, 1));
        blocked.Add(new Vector2Int(2, 2));
        blocked.Add(new Vector2Int(2, 3));

        var map = new DungeonMapData(new List<DungeonRoom>(), floor, blocked);
        var path = AStar.FindPath(map, new Vector2Int(0, 0), new Vector2Int(4, 0), new RectInt(0, 0, 5, 5));
        Assert.IsNotNull(path);
        AssertPathAdj(path);
    }

    // helpers
    DungeonRoom FindStartRoom(PcgTestGen.Data data)
    {
        foreach (var room in data.rooms) if (room.isStartRoom) return room;
        return null;
    }

    DungeonRoom FindFinalRoom(PcgTestGen.Data data)
    {
        foreach (var room in data.rooms) if (room.isFinalRoom) return room;
        return null;
    }

    void AssertPathAdj(List<Vector2Int> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            int dist = Mathf.Abs(path[i].x - path[i - 1].x) + Mathf.Abs(path[i].y - path[i - 1].y);
            Assert.AreEqual(1, dist);
        }
    }
}
