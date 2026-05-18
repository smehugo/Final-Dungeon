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

        int depth = GetInteriorBspDepth(interior);
        List<RectInt> leaves = new List<RectInt>();
        List<InteriorWall> walls = new List<InteriorWall>();
        SplitInterior(interior, 0, depth, leaves, walls);
        room.interiorWalls.AddRange(walls);

        foreach (var leaf in leaves)
        {
            room.zones.Add(new RoomZone { bounds = leaf });
        }
    }

    private int GetInteriorBspDepth(RectInt bounds)
    {
        int minDim = Mathf.Min(bounds.width, bounds.height);
        if (minDim < 16) return 1;
        if (minDim < 28) return 2;
        return 3;
    }

    private void SplitInterior(RectInt rect, int depth, int maxDepth, List<RectInt> leaves, List<InteriorWall> walls)
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
            walls.Add(new InteriorWall
            {
                bounds = new RectInt(rect.xMin, splitY, rect.width, 1),
                isVertical = false
            });
            SplitInterior(new RectInt(rect.xMin, rect.yMin, rect.width, splitY - rect.yMin), depth + 1, maxDepth, leaves, walls);
            SplitInterior(new RectInt(rect.xMin, splitY, rect.width, rect.yMax - splitY), depth + 1, maxDepth, leaves, walls);
        }
        else
        {
            int minCut = rect.xMin + minZone;
            int maxCut = rect.xMax - minZone;
            int splitX = Random.Range(minCut, maxCut + 1);
            walls.Add(new InteriorWall
            {
                bounds = new RectInt(splitX, rect.yMin, 1, rect.height),
                isVertical = true
            });
            SplitInterior(new RectInt(rect.xMin, rect.yMin, splitX - rect.xMin, rect.height), depth + 1, maxDepth, leaves, walls);
            SplitInterior(new RectInt(splitX, rect.yMin, rect.xMax - splitX, rect.height), depth + 1, maxDepth, leaves, walls);
        }
    }
}