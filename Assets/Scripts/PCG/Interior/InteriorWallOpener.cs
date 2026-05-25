using UnityEngine;
using System.Collections.Generic;

public class InteriorWallOpener
{
    public void PopulateWallTiles(InteriorWall wall, HashSet<Vector2Int> reserved)
    {
        for (int x = wall.bounds.xMin; x < wall.bounds.xMax; x++)
        {
            for (int y = wall.bounds.yMin; y < wall.bounds.yMax; y++)
            {
                var tile = new Vector2Int(x, y);
                if (!reserved.Contains(tile))
                    wall.tiles.Add(tile);
            }
        }
    }

    public void WallOpener(InteriorWall wall, HashSet<Vector2Int> reserved, int wallOpeningMin, float wallOpeningMax, float wallExtraHoleGamba)
    {
        // not reserved
        List<Vector2Int> candidates = new List<Vector2Int>(wall.tiles);

        int openingW = Random.Range(wallOpeningMin, Mathf.Max(wallOpeningMin + 1, (int)(candidates.Count * wallOpeningMax)));
        foreach (var tile in candidates)
        {
            if (Random.value < wallExtraHoleGamba)
                wall.tiles.Remove(tile);
        }

        // find start
        for (int attempt = 0; attempt < 20; attempt++)
        {
            if (candidates.Count < openingW)
                break;

            int startIdx = Random.Range(0, candidates.Count - openingW + 1);

            bool valid = true;
            for (int i = 0; i < openingW; i++)
            {
                if (reserved.Contains(candidates[startIdx + i]))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                for (int i = 0; i < openingW; i++)
                    wall.tiles.Remove(candidates[startIdx + i]);
                return;
            }
        }

    }
}