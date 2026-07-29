using System.Globalization;
using NUnit.Framework;
using UnityEngine;
using System;

// data collection for metrics, writes a csv file one row per seed
public class PcgMetricsTests : PcgTestBase
{
    private const string FileName = "generation.csv";

    private const string Header =
    "run,seed,rooms,leaves,roomsCreated,floorTiles,corridorTiles,blockedTiles," +
    "mstBaseEdges,mstEdges,mstComponents,corridorFailures,doors," +
    "zones,interiorWalls,solidWalls,blockedRoomCentres,artifactRooms," +
    "walkableTiles,reachableTiles,reachableRatio,startToFinalPathLength,detourFactor";

    [Test]
    public void RecordMetrics4Seeds()
    {
        string path = PcgMetricsRecorder.EnsureFile(FileName, Header);
        string run = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        foreach (int seed in DungeonGenConfig.TestSeeds)
            PcgMetricsRecorder.AppRow(path, run + "," + BuildRow(seed));

        Debug.Log($"done {path}");
    }

    private static string BuildRow(int seed)
    {
        var data = PcgTestGen.Generate(seed);

        int doors = 0;
        int zones = 0;
        int walls = 0;
        int solidWalls = 0;
        int blockedCentres = 0;
        int artifactRooms = 0;

        foreach (var room in data.rooms)
        {
            doors += room.doors.Count;
            zones += room.zones.Count;
            walls += room.interiorWalls.Count;
            if (room.hasArtifact) artifactRooms++;
            if (!data.mapData.IsWalkable(room.center)) blockedCentres++;

            foreach (var wall in room.interiorWalls)
                if (wall.tiles.Count >= wall.bounds.width * wall.bounds.height) solidWalls++;
        }

        int walkable = 0;
        foreach (var tile in data.floorTiles)
            if (data.mapData.IsWalkable(tile)) walkable++;

        int reachable = 0;
        int pathLength = -1;
        float detour = -1f;

        var start = StartRoom(data);
        var final = FinalRoom(data);

        if (start != null && TryGetWalkaleTile(data, start, out var from))
        {
            reachable = Reachability.FloodFill(data.mapData, from).Count;

            if (final != null && TryGetWalkaleTile(data, final, out var to))
            {
                var path = AStar.FindPath(data.mapData, from, to, MapBounds);
                if (path != null)
                {
                    pathLength = path.Count;

                    float straight = Vector2Int.Distance(start.center, final.center);
                    if (straight > 0f) detour = pathLength / straight;
                }
            }
        }

        float ratio = walkable > 0 ? (float)reachable / walkable : 0f;
        int components = CountComponents(data.rooms.Count, data.mstBaseEdges);

        return string.Join(",",
            seed.ToString(CultureInfo.InvariantCulture),
            data.rooms.Count.ToString(CultureInfo.InvariantCulture),
            data.leaves.Count.ToString(CultureInfo.InvariantCulture),
            data.roomsCreated.ToString(CultureInfo.InvariantCulture),
            data.floorTiles.Count.ToString(CultureInfo.InvariantCulture),
            data.corridorTiles.Count.ToString(CultureInfo.InvariantCulture),
            data.blockedTiles.Count.ToString(CultureInfo.InvariantCulture),
            data.mstBaseEdges.Count.ToString(CultureInfo.InvariantCulture),
            data.mstEdges.Count.ToString(CultureInfo.InvariantCulture),
            components.ToString(CultureInfo.InvariantCulture),
            data.corridorFailures.ToString(CultureInfo.InvariantCulture),
            doors.ToString(CultureInfo.InvariantCulture),
            zones.ToString(CultureInfo.InvariantCulture),
            walls.ToString(CultureInfo.InvariantCulture),
            solidWalls.ToString(CultureInfo.InvariantCulture),
            blockedCentres.ToString(CultureInfo.InvariantCulture),
            artifactRooms.ToString(CultureInfo.InvariantCulture),
            walkable.ToString(CultureInfo.InvariantCulture),
            reachable.ToString(CultureInfo.InvariantCulture),
            ratio.ToString("F4", CultureInfo.InvariantCulture),
            pathLength.ToString(CultureInfo.InvariantCulture),
            detour.ToString("F4", CultureInfo.InvariantCulture));
    }
}
