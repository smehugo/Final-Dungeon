using System.Collections.Generic;
using UnityEngine;

public static class Reachability
{
    public static HashSet<Vector2Int> FloodFill(DungeonMapData mapData, Vector2Int start)
    {
        var visited = new HashSet<Vector2Int>();

        if (!mapData.IsWalkable(start))
            return visited;

        var q = new Queue<Vector2Int>();
        q.Enqueue(start);
        visited.Add(start);

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            foreach (var d in dirs)
            {
                var next = cur + d;
                if (visited.Contains(next)) continue;
                if (!mapData.IsWalkable(next)) continue;
                visited.Add(next);
                q.Enqueue(next);
            }
        }

        return visited;
    }
}
