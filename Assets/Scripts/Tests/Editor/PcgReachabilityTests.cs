using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

// dungeon can be traversed
public class PcgReachabilityTests : PcgTestBase
{
    // start
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void StartRoom_HasAWalkableTile(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var start = StartRoom(data);
        Assert.IsNotNull(start, $"no start room seed {seed}");

        Assert.IsTrue(TryGetWalkaleTile(data, start, out _), "the start room blocked");
    }

    // all rooms
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryRoom_HasAWalkableTile(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            Assert.IsTrue(TryGetWalkaleTile(data, room, out _),
                $"room {room.id} blocked");
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void PathExists_FromStartToFinal(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        var start = StartRoom(data);
        var final = FinalRoom(data);
        Assert.IsNotNull(start, $"no start room seed {seed}");
        Assert.IsNotNull(final, $"no final room seed {seed}");

        Assert.IsTrue(TryGetWalkaleTile(data, start, out var from), "the start room blocked");
        Assert.IsTrue(TryGetWalkaleTile(data, final, out var to), "the final room blocked");

        var path = AStar.FindPath(data.mapData, from, to, MapBounds);
        Assert.IsNotNull(path, $"no path from start to the final room seed {seed}");
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void PathToTheFinalRoom_IsWalkableStepByStep(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        var start = StartRoom(data);
        var final = FinalRoom(data);
        Assert.IsNotNull(start, $"no start room seed {seed}");
        Assert.IsNotNull(final, $"no final room seed {seed}");

        Assert.IsTrue(TryGetWalkaleTile(data, start, out var from), "the start room blocked");
        Assert.IsTrue(TryGetWalkaleTile(data, final, out var to), "the final room blocked");

        var path = AStar.FindPath(data.mapData, from, to, MapBounds);
        Assert.IsNotNull(path, "no path");

        AssertPathConnected(path);
        foreach (var tile in path)
            Assert.IsTrue(data.mapData.IsWalkable(tile), $"path crosses blocked tile {tile}");
    }

    // all rooms reachable
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryRoom_IsReachableFromTheStartRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var reached = ReachableFromStart(data);

        foreach (var room in data.rooms)
        {
            Assert.IsTrue(TryGetWalkaleTile(data, room, out var tile),
                $"room {room.id} is fully blocked up");
            Assert.IsTrue(reached.Contains(tile),
                $"room {room.id} no reachable from the start");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryArtifactRoom_IsReachableFromTheStartRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var reached = ReachableFromStart(data);

        foreach (var room in data.rooms)
        {
            if (!room.hasArtifact) continue;

            Assert.IsTrue(TryGetWalkaleTile(data, room, out var tile),
                $"artifact room {room.id} blocked");
            Assert.IsTrue(reached.Contains(tile),
                $"artifact room {room.id} no reachable from the start");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryDoor_IsReachableFromTheStartRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var reached = ReachableFromStart(data);

        foreach (var room in data.rooms)
            foreach (var door in room.doors)
                Assert.IsTrue(reached.Contains(door.position),
                    $"door {door.position} in room {room.id} cannot be reached from the start room");
    }

    // A* flood fill
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void FloodFillAndPathfinder_AllRooms(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        var start = StartRoom(data);
        Assert.IsNotNull(start, $"no start room for seed {seed}");
        Assert.IsTrue(TryGetWalkaleTile(data, start, out var from), "the start room is fully blocked");

        var reached = Reachability.FloodFill(data.mapData, from);

        foreach (var room in data.rooms)
        {
            if (!TryGetWalkaleTile(data, room, out var tile)) continue;

            bool flooded = reached.Contains(tile);
            List<Vector2Int> path = AStar.FindPath(data.mapData, from, tile, MapBounds);

            Assert.AreEqual(flooded, path != null,
                $"flood fill and pathfinder not compatible room {room.id}");
        }
    }
}
