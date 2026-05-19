using UnityEngine;
using UnityEngine.Tilemaps;

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
                    finalCorridorTiles.Add(new Vector2Int(x, y));
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
        AddZoneThemes();
        AddWallTM();

        foreach (var tile in finalFloorTiles)
        {
            TileBase floorTile = Tile2ThemeSetter(floorThemeByTile.ContainsKey(tile) ? floorThemeByTile[tile] : FloorTheme.Default);
            floorTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), floorTile);
        }
        foreach (var tile in finalCorridorTiles)
        {
            floorTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), corridorTile);
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
        finalCorridorTiles.Clear();
        finalWallTiles.Clear();
        floorThemeByTile.Clear();
    }

    private void AddZoneThemes()
    {
        // holy loop
        foreach (var room in dungeonRooms)
        {
            foreach (var zone in room.zones)
            {
                for (int x = zone.bounds.xMin; x < zone.bounds.xMax; x++)
                {
                    for (int y = zone.bounds.yMin; y < zone.bounds.yMax; y++)
                    {
                        if (roomTiles.Contains(new Vector2Int(x, y)) && !finalWallTiles.Contains(new Vector2Int(x, y)))
                        {
                            var tile = new Vector2Int(x, y);
                            floorThemeByTile[tile] = zone.theme;
                        }
                    }
                }
            }
        }
    }

    private TileBase Tile2ThemeSetter(FloorTheme theme)
    {
        switch (theme)
        {
            case FloorTheme.Stone:
                return stoneTile;
            case FloorTheme.Wood:
                return woodTile;
            case FloorTheme.Metal:
                return metalTile;
            case FloorTheme.Dirt:
                return dirtTile;
            case FloorTheme.Carpet:
                return carpetTile;
            case FloorTheme.Demonic:
                return demonicTile;
            default: return floorTile;
        }
    }
}