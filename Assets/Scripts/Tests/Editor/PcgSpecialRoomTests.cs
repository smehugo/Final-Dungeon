using NUnit.Framework;
using UnityEngine;

// AssignFinStartRooms
public class PcgSpecialRoomTests : PcgTestBase
{
    // start
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ExactlyOneStartRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        int count = 0;
        foreach (var room in data.rooms) if (room.isStartRoom) count++;

        Assert.AreEqual(1, count);
    }

    // final
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ExactlyOneFinalRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        int count = 0;
        foreach (var room in data.rooms) if (room.isFinalRoom) count++;

        Assert.AreEqual(1, count);
    }

    // startfin rooms are different
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void StartRoom_IsNotTheFinalRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");

        foreach (var room in data.rooms)
            Assert.IsFalse(room.isStartRoom && room.isFinalRoom,
                $"room {room.id} is both start and final");
    }

    // origin
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void StartRoom_HasTheLowestCentreSum(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var start = StartRoom(data);
        Assert.IsNotNull(start, $"no start room for seed {seed}");

        int startSum = start.center.x + start.center.y;
        foreach (var room in data.rooms)
            Assert.GreaterOrEqual(room.center.x + room.center.y, startSum,
                $"room {room.id} is closer to the origin than the start room");
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void FinalRoom_IsTheFarthestFromTheStartRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        if (data.rooms.Count < 2) Assert.Inconclusive("not enough rooms");

        var start = StartRoom(data);
        var final = FinalRoom(data);
        Assert.IsNotNull(start, $"no start room for seed {seed}");
        Assert.IsNotNull(final, $"no final room for seed {seed}");

        float finalDist = Vector2Int.Distance(start.center, final.center);
        foreach (var room in data.rooms)
        {
            if (room == start) continue;
            Assert.LessOrEqual(Vector2Int.Distance(start.center, room.center), finalDist,
                $"room {room.id} is farther from the start than the final room");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ArtifactRoomCount_MatchesTheConfiguredNumber(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        int count = 0;
        foreach (var room in data.rooms) if (room.hasArtifact) count++;

        int candidates = Mathf.Max(0, data.rooms.Count - 2);
        int expected = Mathf.Min(DungeonGenConfig.ArtifactZones, candidates);

        Assert.AreEqual(expected, count);
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void ArtifactRooms_AreNeverTheStartOrFinalRoom(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
            if (room.hasArtifact)
            {
                Assert.IsFalse(room.isStartRoom, $"room {room.id} is start and holds an artifact");
                Assert.IsFalse(room.isFinalRoom, $"room {room.id} is final and holds an artifact");
            }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void EveryArtifactRoom_HasExactlyOneArtifactZone(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
        {
            if (!room.hasArtifact) continue;

            int zones = 0;
            foreach (var zone in room.zones)
                if (zone.type == ZoneType.Artifact) zones++;

            Assert.AreEqual(1, zones, $"room {room.id} has {zones} artifact zones");
        }
    }

    // start and final rooms are open so nothing is placed in them
    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void StartAndFinalRoomZones_AreAllOpen(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        foreach (var room in data.rooms)
        {
            if (!room.isStartRoom && !room.isFinalRoom) continue;

            foreach (var zone in room.zones)
                Assert.AreEqual(ZoneType.Open, zone.type,
                    $"room {room.id} has {zone.type} zone");
        }
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void StartRoom_IsTierZero(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var start = StartRoom(data);
        Assert.AreEqual(0, start.difficultyTier);
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void FinalRoom_IsTheHighestTier(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var final = FinalRoom(data);
        Assert.AreEqual(DungeonGenConfig.DifficultyTiers, final.difficultyTier);
    }

    [TestCaseSource(typeof(DungeonGenConfig), nameof(DungeonGenConfig.TestSeeds))]
    public void FartherRooms_AreNeverALowerTier(int seed)
    {
        var data = PcgTestGen.Generate(seed);
        var start = StartRoom(data);
        foreach (var a in data.rooms)
            foreach (var b in data.rooms)
            {
                if (Vector2Int.Distance(start.center, a.center)
                    <= Vector2Int.Distance(start.center, b.center)) continue;

                Assert.GreaterOrEqual(a.difficultyTier, b.difficultyTier,
                    $"room {a.id} is farther than {b.id} but lower tier");
            }
    }
}
