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
            room.zones.Add(new RoomZone { bounds = interior });
        }
    }

    private int GetInteriorBspDepth(RectInt bounds)
    {
        int minDim = Mathf.Min(bounds.width, bounds.height);
        if (minDim < 16) return 1;
        if (minDim < 28) return 2;
        if (minDim < 36) return 3;
        return 4;
    }

    private void SplitInterior(RectInt rect, int depth, int maxDepth, List<RectInt> leaves)
    {
        const int minZone = 8;

        bool canSplitH = rect.height >= minZone * 2;
        bool canSplitV = rect.width >= minZone * 2;

        if (depth >= maxDepth || (!canSplitH && !canSplitV))
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
}