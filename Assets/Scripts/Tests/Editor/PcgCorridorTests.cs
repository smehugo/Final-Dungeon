using NUnit.Framework;
using UnityEngine;

public class PcgCorridorTests : PcgTestBase
{
    // corridors
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Corridors_ExistWhenThereAreMultipleRooms(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");
        Assert.Greater(data.corridorTiles.Count, 0);
    }

    // inside
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void CorridorTiles_AreInsideTheMap(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var tile in data.corridorTiles)
            Assert.IsTrue(MapBounds.Contains(tile), $"corridor tile {tile} is outside the map");
    }

    // no room
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void CorridorTiles_NeverOverlapRoomTiles(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var tile in data.corridorTiles)
            Assert.IsFalse(data.roomTiles.Contains(tile), $"corridor tile {tile} sits inside a room");
    }

    // touch itself or room
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryCorridorTile_TouchesARoomOrAnotherCorridorTile(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var tile in data.corridorTiles)
        {
            bool touches = false;
            foreach (var dir in dirs)
            {
                var next = tile + dir;
                if (data.roomTiles.Contains(next) || data.corridorTiles.Contains(next))
                {
                    touches = true;
                    break;
                }
            }

            Assert.IsTrue(touches, $"corridor tile {tile} is isolated");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryMstEdge_BecameACorridor(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        Assert.AreEqual(0, data.corridorFailures,
            $"{data.corridorFailures} of {data.mstEdges.Count} edges failed to build a corridor");
    }

    // doors
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryRoom_HasAtLeastOneDoor(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");

        foreach (var room in data.rooms)
            Assert.Greater(room.doors.Count, 0, $"room {room.id} has no door");
    }

    // perimeter
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Doors_SitOnTheRoomPerimeter(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var door in room.doors)
            {
                Assert.IsTrue(room.bounds.Contains(door.position),
                    $"door {door.position} is outside room {room.id}");
                Assert.IsTrue(IsOnPerimeter(room.bounds, door.position),
                    $"door {door.position} is not on the perimeter of room {room.id}");
            }
    }

    // unique doors
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Doors_AreUnique(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            for (int i = 0; i < room.doors.Count; i++)
                for (int j = i + 1; j < room.doors.Count; j++)
                    Assert.AreNotEqual(room.doors[i].position, room.doors[j].position,
                        $"room {room.id} has two doors on the same tile");
    }

    // reserved tiles
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ReservedTiles_StayInsideTheRoomAndCoverEveryDoor(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
        {
            foreach (var tile in room.reservedTiles)
                Assert.IsTrue(room.bounds.Contains(tile),
                    $"reserved tile {tile} is outside room {room.id}");

            foreach (var door in room.doors)
                Assert.IsTrue(room.reservedTiles.Contains(door.position),
                    $"door {door.position} is not reserved in room {room.id}");
        }
    }

    // reserved <= 3
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ReservedTiles_AreAtMostThreePerDoor(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            Assert.LessOrEqual(room.reservedTiles.Count, room.doors.Count * 3,
                $"room {room.id} reserved more tiles than three per door");
    }
}
