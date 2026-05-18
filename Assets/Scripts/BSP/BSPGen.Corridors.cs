using UnityEngine;

public partial class BSPGen
{
    private void BuildCorridors()
    {
        corridors.Clear();
        foreach (var edge in mstEdges)
        {
            Vector2Int p = roomCenterPoints[edge.a];
            Vector2Int b = roomCenterPoints[edge.b];

            while (p.x != b.x)
            {
                bool was = roomTiles.Contains(p);
                if (!was) corridors.Add(new RectInt(p.x, p.y, 1, 1));
                Vector2Int prev = p;
                if (p.x < b.x)
                { p.x += 1; }
                else
                { p.x += -1; }
                CheckDoorTransition(was, roomTiles.Contains(p), prev, p, edge);
            }

            while (p.y != b.y)
            {
                bool was = roomTiles.Contains(p);
                if (!was) corridors.Add(new RectInt(p.x, p.y, 1, 1));
                Vector2Int prev = p;
                if (p.y < b.y)
                { p.y += 1; }
                else
                { p.y += -1; }
                CheckDoorTransition(was, roomTiles.Contains(p), prev, p, edge);
            }

            if (!roomTiles.Contains(p))
                corridors.Add(new RectInt(p.x, p.y, 1, 1));
        }
    }

    private void CheckDoorTransition(bool wasInRoom, bool nowInRoom, Vector2Int prev, Vector2Int p, RoomEdge edge)
    {
        Vector2Int stepDir = p - prev;

        if (wasInRoom && !nowInRoom)
        {
            if (tileToRoom.TryGetValue(prev, out int roomId))
            {
                var room = dungeonRooms[roomId];
                if (!room.doors.Exists(d => d.position == prev))
                {
                    room.doors.Add(new DoorData { position = prev, inwardDir = -stepDir });
                }
            }
        }
        else if (!wasInRoom && nowInRoom)
        {
            if (tileToRoom.TryGetValue(p, out int roomId))
            {
                var room = dungeonRooms[roomId];
                if (!room.doors.Exists(d => d.position == p))
                {
                    room.doors.Add(new DoorData { position = p, inwardDir = stepDir });
                }
            }
        }
    }

    private void BuildReservedTiles()
    {
        foreach (var room in dungeonRooms)
        {
            foreach (var door in room.doors)
            {
                Vector2Int tile = door.position;
                for (int i = 0; i < 3; i++)
                {
                    if (!room.bounds.Contains(tile))
                        break;
                    room.reservedTiles.Add(tile);
                    tile += door.inwardDir;
                }
            }
        }
    }
}
