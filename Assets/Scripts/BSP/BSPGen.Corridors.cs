using UnityEngine;

public partial class BSPGen
{
    private void BuildCorridors()
    {
        corridors.Clear();
        foreach (var edge in mstEdges)
        {
            Vector2Int a = roomCenterPoints[edge.a];
            Vector2Int b = roomCenterPoints[edge.b];
            Vector2Int p = a;
            bool inCorr = false;

            while (p.x != b.x)
            {
                bool wasInRoom = roomTiles.Contains(p);
                if (!wasInRoom)
                    corridors.Add(new RectInt(p.x, p.y, 1, 1));

                Vector2Int prev = p;
                p.x += p.x < b.x ? 1 : -1;
                bool nowInRoom = roomTiles.Contains(p);

                if (wasInRoom && !nowInRoom && !inCorr)
                {
                    dungeonRooms[edge.a].doors.Add(new DoorData { position = prev });
                    inCorr = true;
                }
                else if (!wasInRoom && nowInRoom && inCorr)
                {
                    dungeonRooms[edge.b].doors.Add(new DoorData { position = p });
                    inCorr = false;
                }
            }

            while (p.y != b.y)
            {
                bool wasInRoom = roomTiles.Contains(p);
                if (!wasInRoom)
                    corridors.Add(new RectInt(p.x, p.y, 1, 1));

                Vector2Int prev = p;
                p.y += p.y < b.y ? 1 : -1;
                bool nowInRoom = roomTiles.Contains(p);

                if (wasInRoom && !nowInRoom && !inCorr)
                {
                    dungeonRooms[edge.a].doors.Add(new DoorData { position = prev });
                    inCorr = true;
                }
                else if (!wasInRoom && nowInRoom && inCorr)
                {
                    dungeonRooms[edge.b].doors.Add(new DoorData { position = p });
                    inCorr = false;
                }
            }

            if (!roomTiles.Contains(p))
                corridors.Add(new RectInt(p.x, p.y, 1, 1));

                Debug.Log($"corridor from {edge.a} to {edge.b} length: {Vector2Int.Distance(a, b)}");
                Debug.Log($"doors {edge.a}: {dungeonRooms[edge.a].doors.Count}, doors in room {edge.b}: {dungeonRooms[edge.b].doors.Count}");
        }
    }
}
