using UnityEngine;
using System.Collections.Generic;

public class CorridorBuilder
{
    public void BuildCorridors(List<RoomEdge> mstEdges, List<DungeonRoom> dungeonRooms, HashSet<Vector2Int> roomTiles, List<RectInt> corridors)
    {
        corridors.Clear();
        foreach (var edge in mstEdges)
        {
            var roomA = dungeonRooms[edge.a];
            var roomB = dungeonRooms[edge.b];
            TryBuildCorr(roomA, roomB, roomTiles, corridors);
        }
    }

    private bool TryBuildCorr(DungeonRoom ra, DungeonRoom rb, HashSet<Vector2Int> roomTiles, List<RectInt> corridors)
    {
        // horizontal

        // can we do straight?
        int yMin = Mathf.Max(ra.bounds.yMin, rb.bounds.yMin);
        int yMax = Mathf.Min(ra.bounds.yMax, rb.bounds.yMax);
        if (yMax - yMin > 1)
        {
            return TryBuildHorCorr(ra, rb, roomTiles, corridors);
        }

        // vertical
        int xMin = Mathf.Max(ra.bounds.xMin, rb.bounds.xMin);
        int xMax = Mathf.Min(ra.bounds.xMax, rb.bounds.xMax);
        if (xMax - xMin > 1)
        {
            return TryBuildVerCorr(ra, rb, roomTiles, corridors);
        }
        return false;
    }

    private void AddDoor(DungeonRoom room, Vector2Int pos, Vector2Int inDirr)
    {
        if (!room.doors.Exists(d => d.position == pos))
        {
            room.doors.Add(new DoorData { position = pos, inwardDir = inDirr });
        }
    }

    public void BuildReservedTiles(List<DungeonRoom> dungeonRooms)
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

    private bool TryBuildVerCorr(DungeonRoom ra, DungeonRoom rb, HashSet<Vector2Int> roomTiles, List<RectInt> corridors)
    {
        DungeonRoom T;
        if (ra.bounds.yMax < rb.bounds.yMax)
        { T = ra; }
        else { T = rb; }
        DungeonRoom B;
        if (T == ra) { B = rb; }
        else { B = ra; }

        if (T.bounds.yMax > B.bounds.yMin)
            return false;

        int x;
        if (T.bounds.xMin > B.bounds.xMin)
        {
            x = Random.Range(T.bounds.xMin + 1,
                            (T.bounds.xMax < B.bounds.xMax
                            ? T.bounds.xMax : B.bounds.xMax) - 1);
        }
        else
        {
            x = Random.Range(B.bounds.xMin + 1,
                            (T.bounds.xMax < B.bounds.xMax
                            ? T.bounds.xMax : B.bounds.xMax) - 1);
        }

        // check if clear
        bool blocked = false;
        for (int y = T.bounds.yMax; y < B.bounds.yMin; y++)
        {
            if (roomTiles.Contains(new Vector2Int(x, y)))
            {
                blocked = true; break;
            }
        }
        if (blocked) return false;

        for (int y = T.bounds.yMax; y < B.bounds.yMin; y++)
        {
            corridors.Add(new RectInt(x, y, 1, 1));
        }

        AddDoor(T, new Vector2Int(x, T.bounds.yMax - 1), new Vector2Int(0, -1));
        AddDoor(B, new Vector2Int(x, B.bounds.yMin), new Vector2Int(0, 1));
        return true;
    }

    private bool TryBuildHorCorr(DungeonRoom ra, DungeonRoom rb, HashSet<Vector2Int> roomTiles, List<RectInt> corridors)
    {
        DungeonRoom L;
        if (ra.bounds.xMin < rb.bounds.xMin)
        { L = ra; }
        else
        { L = rb; }
        DungeonRoom R;
        if (L == ra)
        { R = rb; }
        else
        { R = ra; }
        if (L.bounds.xMax > R.bounds.xMin)
            return false;

        int y;
        if (L.bounds.yMin > R.bounds.yMin)
        {
            y = Random.Range(L.bounds.yMin + 1,
                            (L.bounds.yMax < R.bounds.yMax
                            ? L.bounds.yMax : R.bounds.yMax) - 1);
        }
        else
        {
            y = Random.Range(R.bounds.yMin + 1,
                            (L.bounds.yMax < R.bounds.yMax
                            ? L.bounds.yMax : R.bounds.yMax) - 1);
        }

        // check if clear
        bool blocked = false;
        for (int x = L.bounds.xMax; x < R.bounds.xMin; x++)
        {
            if (roomTiles.Contains(new Vector2Int(x, y))) { blocked = true; break; }
        }
        if (blocked) return false;

        for (int x = L.bounds.xMax; x < R.bounds.xMin; x++)
        {
            corridors.Add(new RectInt(x, y, 1, 1));
        }

        AddDoor(L, new Vector2Int(L.bounds.xMax - 1, y), new Vector2Int(-1, 0));
        AddDoor(R, new Vector2Int(R.bounds.xMin, y), new Vector2Int(1, 0));
        return true;
    }

}