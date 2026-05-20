using UnityEngine;
using System.Collections.Generic;

public partial class BSPGen
{
    private void BuildRoomInteriors()
    {
        foreach (var room in dungeonRooms)
        {
            BuildRoomInterior(room);
        }
        AssignZoneTypes();
    }

    private void BuildRoomInterior(DungeonRoom room)
    {
        RectInt interior = new RectInt(
            room.bounds.xMin + 1,
            room.bounds.yMin + 1,
            room.bounds.width - 2,
            room.bounds.height - 2
            );

        for (int attempt = 0; attempt < 10; attempt++)
        {
            room.zones.Clear();
            room.interiorWalls.Clear();

            int depth = GetInteriorBspDepth(interior);
            List<RectInt> leaves = new List<RectInt>();
            List<InteriorWall> walls = new List<InteriorWall>();
            SplitInterior(interior, 0, depth, leaves);
            GetWallsByZone(leaves, walls);
            room.interiorWalls.AddRange(walls);

            foreach (var leaf in leaves)
            {
                room.zones.Add(new RoomZone { bounds = leaf });
            }

            foreach (var wall in room.interiorWalls)
            {
                PopulateWallTiles(wall, room.reservedTiles);
            }
            foreach (var wall in room.interiorWalls)
            {
                WallOpener(wall, room.reservedTiles);
            }

            // validate room
            if (IsRoomValid(room))
            {
                return;
            }
            //fallback to empty
            room.zones.Clear();
            room.interiorWalls.Clear();
            room.zones.Add(new RoomZone { bounds = interior, type = ZoneType.Empty, theme = SetFloorTheme(room, ZoneType.Empty) });
        }
    }

    private int GetInteriorBspDepth(RectInt bounds)
    {
        int minDim = Mathf.Min(bounds.width, bounds.height);
        return Mathf.Clamp(minDim / interiorDepthStep, 1, interiorMaxDepth);
    }

    private void SplitInterior(RectInt rect, int depth, int maxDepth, List<RectInt> leaves)
    {
        int minZone = minZoneSize;
        int maxZone = maxZoneSize;

        bool canSplitH = rect.height >= minZone * 2;
        bool canSplitV = rect.width >= minZone * 2;
        bool gottaStopThoseSteroidsMate = rect.width > maxZone || rect.height > maxZone;

        if ((!gottaStopThoseSteroidsMate && depth >= maxDepth) || (!canSplitH && !canSplitV))
        {
            leaves.Add(rect);
            return;
        }

        bool splitHor = canSplitH && (!canSplitV || SplitDirection(rect, depth));

        if (splitHor)
        {
            int minCut = rect.yMin + minZone;
            int maxCut = rect.yMax - minZone;
            int splitY = Random.Range(minCut, maxCut + 1);
            SplitInterior(new RectInt(rect.xMin, rect.yMin, rect.width, splitY - rect.yMin), depth + 1, maxDepth, leaves);
            SplitInterior(new RectInt(rect.xMin, splitY, rect.width, rect.yMax - splitY), depth + 1, maxDepth, leaves);
        }
        else
        {
            int minCut = rect.xMin + minZone;
            int maxCut = rect.xMax - minZone;
            int splitX = Random.Range(minCut, maxCut + 1);
            SplitInterior(new RectInt(rect.xMin, rect.yMin, splitX - rect.xMin, rect.height), depth + 1, maxDepth, leaves);
            SplitInterior(new RectInt(splitX, rect.yMin, rect.xMax - splitX, rect.height), depth + 1, maxDepth, leaves);
        }
    }

    private void GetWallsByZone(List<RectInt> zones, List<InteriorWall> walls)
    {
        for (int i = 0; i < zones.Count; i++)
        {
            for (int j = i + 1; j < zones.Count; j++)
            {
                var a = zones[i];
                var b = zones[j];

                // a below b
                if (a.yMax == b.yMin)
                {
                    int xMin = Mathf.Max(a.xMin, b.xMin);
                    int xMax = Mathf.Min(a.xMax, b.xMax);
                    if (xMax > xMin)
                        walls.Add(new InteriorWall { bounds = new RectInt(xMin, a.yMax, xMax - xMin, 1), isVertical = false });
                }
                // b below a
                else if (b.yMax == a.yMin)
                {
                    int xMin = Mathf.Max(a.xMin, b.xMin);
                    int xMax = Mathf.Min(a.xMax, b.xMax);
                    if (xMax > xMin)
                        walls.Add(new InteriorWall { bounds = new RectInt(xMin, b.yMax, xMax - xMin, 1), isVertical = false });
                }
                // a left to b
                else if (a.xMax == b.xMin)
                {
                    int yMin = Mathf.Max(a.yMin, b.yMin);
                    int yMax = Mathf.Min(a.yMax, b.yMax);
                    if (yMax > yMin)
                        walls.Add(new InteriorWall { bounds = new RectInt(a.xMax, yMin, 1, yMax - yMin), isVertical = true });
                }
                // a right to b
                else if (b.xMax == a.xMin)
                {
                    int yMin = Mathf.Max(a.yMin, b.yMin);
                    int yMax = Mathf.Min(a.yMax, b.yMax);
                    if (yMax > yMin)
                        walls.Add(new InteriorWall { bounds = new RectInt(b.xMax, yMin, 1, yMax - yMin), isVertical = true });
                }

            }
        }
    }

    private FloorTheme SetFloorTheme(DungeonRoom room, ZoneType type)
    {
        if (room.isStartRoom)
            return FloorTheme.Stone;
        if (room.isFinalRoom)
            return FloorTheme.Demonic;

        switch (type)
        {
            case ZoneType.Empty:
                return FloorTheme.Default;
            case ZoneType.Enemy:
                return FloorTheme.Dirt;
            case ZoneType.Treasure:
                return FloorTheme.Carpet;
            case ZoneType.Open:
                return FloorTheme.Default;
            case ZoneType.Decoration:
                return FloorTheme.Default;
            case ZoneType.Artifact:
                return FloorTheme.Carpet;
            default:
                return FloorTheme.Default;
        }
    }

    private ZoneSpawnTag GetZoneTag(DungeonRoom room, ZoneType type)
    {
        if (room.isStartRoom)
            return ZoneSpawnTag.Light | ZoneSpawnTag.Decoration;

        return type switch
        {
            ZoneType.Enemy => ZoneSpawnTag.Enemy | ZoneSpawnTag.Light | ZoneSpawnTag.Decoration,
            ZoneType.Treasure => ZoneSpawnTag.Loot | ZoneSpawnTag.Light | ZoneSpawnTag.Decoration,
            ZoneType.Decoration => ZoneSpawnTag.Decoration | ZoneSpawnTag.Obstacle | ZoneSpawnTag.Light,
            ZoneType.Artifact => ZoneSpawnTag.Artifact | ZoneSpawnTag.Light | ZoneSpawnTag.Decoration,
            ZoneType.Open => ZoneSpawnTag.Light,
            _ => ZoneSpawnTag.Light,
        };
    }

    private void AssignZoneTypes()
    {
        // https://www.geeksforgeeks.org/c-sharp/c-sharp-tuple-class/
        var privilegedZones = new List<(DungeonRoom room, RoomZone zone)>();
        var normalZones = new List<(DungeonRoom room, RoomZone zone)>();

        // privileged zones remove
        foreach (var room in dungeonRooms)
        {
            foreach (var zone in room.zones)
            {
                if (room.isStartRoom || room.isFinalRoom)
                {
                    privilegedZones.Add((room, zone));
                }
                else
                {
                    normalZones.Add((room, zone));
                }
            }
        }

        foreach (var (room, zone) in privilegedZones)
        {
            zone.type = ZoneType.Open;
            zone.theme = SetFloorTheme(room, zone.type);
            zone.allowedTags = GetZoneTag(room, zone.type);
        }

        // shuffle normals https://www.geeksforgeeks.org/dsa/shuffle-a-given-array-using-fisher-yates-shuffle-algorithm/
        for (int i = normalZones.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (normalZones[i], normalZones[j]) = (normalZones[j], normalZones[i]);
        }

        var budget = new List<ZoneType>();

        for (int i = 0; i < artifactZones; i++)
            budget.Add(ZoneType.Artifact);

        // fill the rest
        int remaining = normalZones.Count - artifactZones;
        for (int i = 0; i < remaining; i++)
        {
            int gamba = Random.Range(0, 10);
            if (gamba < 4) budget.Add(ZoneType.Enemy);
            else if (gamba < 6) budget.Add(ZoneType.Treasure);
            else if (gamba < 8) budget.Add(ZoneType.Decoration);
            else budget.Add(ZoneType.Open);
        }

        // normal zones
        for (int i = 0; i < normalZones.Count; i++)
        {
            var (room, zone) = normalZones[i];
            zone.type = budget[i];
            zone.theme = SetFloorTheme(room, zone.type);
            zone.allowedTags = GetZoneTag(room, zone.type);
        }
    }
}