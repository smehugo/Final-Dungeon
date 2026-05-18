using UnityEngine;
using System.Collections.Generic;

public partial class BSPGen
{
    private void PopulateWallTiles(InteriorWall wall, HashSet<Vector2Int> reserved)
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

    private void WallOpener(InteriorWall wall, HashSet<Vector2Int> reserved)
    {
        int openingW = Random.Range(3, 5);

        // not reserved
        List<Vector2Int> candidates = new List<Vector2Int>(wall.tiles);

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