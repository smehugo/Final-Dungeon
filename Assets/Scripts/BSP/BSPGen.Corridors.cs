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

            while (p.x != b.x)
            {
                if (!roomTiles.Contains(p))
                    corridors.Add(new RectInt(p.x, p.y, 1, 1));
                p.x += p.x < b.x ? 1 : -1;
            }

            while (p.y != b.y)
            {
                if (!roomTiles.Contains(p))
                    corridors.Add(new RectInt(p.x, p.y, 1, 1));
                p.y += p.y < b.y ? 1 : -1;
            }

            if (!roomTiles.Contains(p))
                corridors.Add(new RectInt(p.x, p.y, 1, 1));
        }
    }
}
