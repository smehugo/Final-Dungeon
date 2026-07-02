using UnityEngine;
using System.Collections.Generic;

// https://www.redblobgames.com/pathfinding/a-star/introduction.html
// all tiles cost 1 = no heuristic

public static class AStar
{
    private static readonly Vector2Int[] dirs =
    {
        // maybe add diagonals later
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    public static List<Vector2Int> FindPath(
        DungeonMapData mapData,
        Vector2Int start,
        Vector2Int goal,
        RectInt searchBounds)
    {
        // at goal, not in room, blocked tile... early out
        if (start == goal) return new List<Vector2Int> { start };
        if (!searchBounds.Contains(goal)) return null;
        if (!mapData.IsFree(goal)) return null;

        var queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // early exit when removing goal from Q
            if (current == goal)
                return RebuildPath(cameFrom, start, goal);

            foreach (Vector2Int next in GetNeighbors(current, mapData, searchBounds))
            {
                if (cameFrom.ContainsKey(next))
                    continue;

                cameFrom[next] = current;
                queue.Enqueue(next);
            }
        }

        return null;
    }

    private static List<Vector2Int> GetNeighbors(
        Vector2Int tile,
        DungeonMapData mapData,
        RectInt searchBounds)
    {
        var neighbors = new List<Vector2Int>();

        foreach (Vector2Int dir in dirs)
        {
            Vector2Int next = tile + dir;
            if (!searchBounds.Contains(next))
                continue;

            if (!mapData.IsFree(next))
                continue;

            neighbors.Add(next);
        }

        return neighbors;
    }

    private static List<Vector2Int> RebuildPath(
        Dictionary<Vector2Int, Vector2Int> cameFrom,
        Vector2Int start,
        Vector2Int goal)
    {
        var path = new List<Vector2Int>();
        Vector2Int current = goal;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
}
