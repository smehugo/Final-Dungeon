using NUnit.Framework;
using UnityEngine;

public class PcgInteriorTests : PcgTestBase
{
    // zones in rooms
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryRoom_HasAtLeastOneZone(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            Assert.Greater(room.zones.Count, 0, $"room {room.id} has no zones");
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Zones_StayInsideTheirRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var zone in room.zones)
            {
                Assert.GreaterOrEqual(zone.bounds.xMin, room.bounds.xMin);
                Assert.GreaterOrEqual(zone.bounds.yMin, room.bounds.yMin);
                Assert.LessOrEqual(zone.bounds.xMax, room.bounds.xMax);
                Assert.LessOrEqual(zone.bounds.yMax, room.bounds.yMax);
            }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Zones_DoNotOverlap(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            for (int i = 0; i < room.zones.Count; i++)
                for (int j = i + 1; j < room.zones.Count; j++)
                    Assert.IsFalse(room.zones[i].bounds.Overlaps(room.zones[j].bounds),
                        $"zones {i} and {j} overlap in room {room.id}");
    }

    // full coverage
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Zones_CoverTheWholeRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
        {
            int area = 0;
            foreach (var zone in room.zones)
                area += zone.bounds.width * zone.bounds.height;

            Assert.AreEqual(room.bounds.width * room.bounds.height, area,
                $"zones do not cover room {room.id}");
        }
    }

    // min size on zones
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void Zones_AreAtLeastMinZoneSize(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var zone in room.zones)
            {
                Assert.GreaterOrEqual(zone.bounds.width, DungeonGenConfig.MinZoneSize);
                Assert.GreaterOrEqual(zone.bounds.height, DungeonGenConfig.MinZoneSize);
            }
    }

    // no double walls
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void InteriorWalls_AreOneTileThick(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var wall in room.interiorWalls)
                Assert.IsTrue(wall.bounds.width == 1 || wall.bounds.height == 1,
                    $"wall {wall.bounds} in room {room.id} is thicker than one tile");
    }

    // inside
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void InteriorWalls_StayInsideTheirRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var wall in room.interiorWalls)
                foreach (var tile in wall.tiles)
                    Assert.IsTrue(room.bounds.Contains(tile),
                        $"wall tile {tile} is outside room {room.id}");
    }

    // openings
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void InteriorWalls_LongEnoughForAnOpening_HaveOne(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var wall in room.interiorWalls)
            {
                int length = wall.bounds.width * wall.bounds.height;
                if (length < DungeonGenConfig.WallOpeningMin) continue;

                Assert.Less(wall.tiles.Count, length,
                    $"wall {wall.bounds} in room {room.id} has no opening");
            }
    }

    // validation
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryRoom_PassesRoomValidation(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var validator = new RoomValidaton();

        foreach (var room in data.rooms)
            Assert.IsTrue(validator.IsRoomValid(room), $"room {room.id} failed validation");
    }

    // walkable if on centre
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void RoomsWithACentreOnAWall_StillHaveAWalkableTile(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
        {
            if (data.mapData.IsWalkable(room.center)) continue;

            Assert.IsTrue(TryGetWalkaleTile(data, room, out _),
                $"room {room.id} has a blocked centre and no walkable tile at all");
        }
    }

    // not empty if no zone
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void NoZone_IsLeftAsTheEmptyType(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var zone in room.zones)
                Assert.AreNotEqual(ZoneType.Empty, zone.type,
                    $"a zone in room {room.id} was never assigned a type");
    }

    // theme matching on zone
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ZoneThemes_MatchTheirZoneType(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var zone in room.zones)
                Assert.AreEqual(ExpectedTheme(room, zone.type), zone.theme,
                    $"wrong theme for a {zone.type} zone in room {room.id}");
    }

    // artifact in artifact room only
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ArtifactZones_OnlyAppearInArtifactRooms(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            foreach (var zone in room.zones)
                if (zone.type == ZoneType.Artifact)
                    Assert.IsTrue(room.hasArtifact,
                        $"room {room.id} has an artifact zone but is not an artifact room");
    }

    // mirrors SetFloorTheme
    private static FloorTheme ExpectedTheme(DungeonRoom room, ZoneType type)
    {
        if (room.isStartRoom) return FloorTheme.Stone;
        if (room.isFinalRoom) return FloorTheme.Demonic;

        switch (type)
        {
            case ZoneType.Enemy: return FloorTheme.Dirt;
            case ZoneType.Treasure: return FloorTheme.Carpet;
            case ZoneType.Artifact: return FloorTheme.Carpet;
            default: return FloorTheme.Default;
        }
    }
}
