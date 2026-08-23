using UnityEngine;
using System.Collections.Generic;

public class RoomValidation
{
    public bool IsRoomValid(DungeonRoom room, DungeonMapData mapData)
    {
        // need all blocked tiles: walls
        HashSet<Vector2Int> blocked = new();
        foreach (var wall in room.interiorWalls)
            foreach (var tile in wall.tiles)
            {
                blocked.Add(tile);
            }

        if (mapData != null)
        {
            for (int x = room.bounds.min.x; x < room.bounds.max.x; x++)
            {
                for (int y = room.bounds.min.y; y < room.bounds.max.y; y++)
                {
                    Vector2Int tile = new Vector2Int(x, y);
                    if (mapData.IsWalkable(tile) && !mapData.IsFree(tile))
                        blocked.Add(tile);
                }
            }
        }

        Vector2Int start = room.doors[0].position;
        HashSet<Vector2Int> visited = FloodFill(start, room.bounds, blocked);
        // debugLastFlood = visited;

        // Debug.Log($"Room {room.id} bounds={room.bounds} center={room.center} inBounds={room.bounds.Contains(room.center)} doors={room.doors.Count} visited={visited.Count} blocked={blocked.Count}");

        // verify doors
        foreach (var door in room.doors)
        {
            if (!visited.Contains(door.position))
            {
                // Debug.Log($"  door pos={door.position} inBounds={room.bounds.Contains(door.position)} inBlocked={blocked.Contains(door.position)} inVisited={visited.Contains(door.position)}");
                return false;
            }
        }

        return true;
    }

    private HashSet<Vector2Int> FloodFill(Vector2Int start, RectInt bounds, HashSet<Vector2Int> blocked)
    {
        HashSet<Vector2Int> visited = new();
        Queue<Vector2Int> queue = new();
        queue.Enqueue(start);
        visited.Add(start);

        // flood movement
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;
                if (!visited.Contains(next) && bounds.Contains(next) && !blocked.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue(next);
                }
            }
        }

        return visited;
    }
}