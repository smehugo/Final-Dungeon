using UnityEngine;

public partial class BSPGen
{
    private void AddFloorTM()
    {
        finalFloorTiles.UnionWith(roomTiles);
    }

    private void AddCorridorTM()
    {
        foreach (var corridor in corridors)
        {
            for (int x = corridor.xMin; x < corridor.xMax; x++)
            {
                for (int y = corridor.yMin; y < corridor.yMax; y++)
                {
                    finalFloorTiles.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    private void AddInteriorWallTM()
    {
        foreach (var room in dungeonRooms)
        {
            foreach (var wall in room.interiorWalls)
            {
                foreach (var tile in wall.tiles)
                {
                    finalWallTiles.Add(tile);
                    blockedTiles.Add(tile);
                }
            }
        }
    }

    private void AddWallTM()
    {
        // O(n^2)?
        foreach (var tile in finalFloorTiles)
        {
            for (int x = tile.x - 1; x <= tile.x + 1; x++)
            {
                for (int y = tile.y - 1; y <= tile.y + 1; y++)
                {
                    Vector2Int wallTile = new Vector2Int(x, y);
                    if (!finalFloorTiles.Contains(wallTile))
                    {
                        finalWallTiles.Add(wallTile);
                        blockedTiles.Add(wallTile);
                    }
                }
            }
        }
    }

    private void BuildAllTM()
    {
        ClearTilemaps();

        AddFloorTM();
        AddCorridorTM();
        AddInteriorWallTM();
        AddWallTM();

        foreach (var tile in finalFloorTiles)
        {
            floorTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), floorTile);
        }
        foreach (var tile in finalWallTiles)
        {
            wallTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), wallTile);
        }

        // this fs doesnt make up for the loops above xD
        floorTilemap.CompressBounds();
        wallTilemap.CompressBounds();
    }

    private void ClearTilemaps()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        blockedTiles.Clear();
        finalFloorTiles.Clear();
        finalWallTiles.Clear();
    }
}