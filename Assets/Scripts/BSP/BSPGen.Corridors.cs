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
            bool inCorr = false;

            while (p.x != b.x)
            {
                bool was = roomTiles.Contains(p);
                if (!was) corridors.Add(new RectInt(p.x, p.y, 1, 1));
                Vector2Int prev = p;
                if (p.x < b.x)
                { p.x += 1; }
                else
                { p.x += -1; }
                CheckDoorTransition(was, roomTiles.Contains(p), ref inCorr, prev, p, edge);
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
                CheckDoorTransition(was, roomTiles.Contains(p), ref inCorr, prev, p, edge);
            }

            if (!roomTiles.Contains(p))
                corridors.Add(new RectInt(p.x, p.y, 1, 1));
        }
    }

    private void CheckDoorTransition(bool wasInRoom, bool nowInRoom, ref bool inCorr, Vector2Int prev, Vector2Int p, RoomEdge edge)
    {
        Vector2Int stepDir = p - prev;

        if (wasInRoom && !nowInRoom && !inCorr)
        {
            dungeonRooms[edge.a].doors.Add(new DoorData { position = prev, inwardDir = -stepDir });
            inCorr = true;
        }
        else if (!wasInRoom && nowInRoom && inCorr)
        {
            dungeonRooms[edge.b].doors.Add(new DoorData { position = p, inwardDir = stepDir });
            inCorr = false;
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
