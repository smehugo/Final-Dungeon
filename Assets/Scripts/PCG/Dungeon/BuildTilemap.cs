using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BuildTilemap
{
    private void AddFloorTM(HashSet<Vector2Int> finalFloorTiles, HashSet<Vector2Int> roomTiles)
    {
        finalFloorTiles.UnionWith(roomTiles);
    }

    private void AddCorridorTM(List<RectInt> corridors, HashSet<Vector2Int> finalCorridorTiles, HashSet<Vector2Int> finalFloorTiles)
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

    private void AddInteriorWallTM(List<DungeonRoom> dungeonRooms, HashSet<Vector2Int> finalWallTiles, HashSet<Vector2Int> blockedTiles)
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

    private void AddWallTM(HashSet<Vector2Int> finalFloorTiles, HashSet<Vector2Int> finalWallTiles, HashSet<Vector2Int> blockedTiles)
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

    public void BuildAllTM(Tilemap floorTilemap, Tilemap wallTilemap, TileBase floorTile, TileBase wallTile, TileBase corridorTile, TileBase stoneTile, TileBase woodTile, TileBase metalTile, TileBase dirtTile, TileBase carpetTile, TileBase demonicTile, HashSet<Vector2Int> roomTiles, List<RectInt> corridors, List<DungeonRoom> dungeonRooms, HashSet<Vector2Int> finalFloorTiles, HashSet<Vector2Int> finalCorridorTiles, HashSet<Vector2Int> finalWallTiles, HashSet<Vector2Int> blockedTiles, Dictionary<Vector2Int, FloorTheme> floorThemeByTile)
    {
        ClearTilemaps(floorTilemap, wallTilemap, blockedTiles, finalFloorTiles, finalCorridorTiles, finalWallTiles, floorThemeByTile);

        AddFloorTM(finalFloorTiles, roomTiles);
        AddCorridorTM(corridors, finalCorridorTiles, finalFloorTiles);
        AddInteriorWallTM(dungeonRooms, finalWallTiles, blockedTiles);
        AddZoneThemes(dungeonRooms, roomTiles, finalWallTiles, floorThemeByTile);
        AddWallTM(finalFloorTiles, finalWallTiles, blockedTiles);

        foreach (var tile in finalFloorTiles)
        {
            TileBase selectedFloorTile = Tile2ThemeSetter(floorThemeByTile.ContainsKey(tile) ? floorThemeByTile[tile] : FloorTheme.Default, floorTile, stoneTile, woodTile, metalTile, dirtTile, carpetTile, demonicTile);
            floorTilemap.SetTile(new Vector3Int(tile.x, tile.y, 0), selectedFloorTile);
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

    private void ClearTilemaps(Tilemap floorTilemap, Tilemap wallTilemap, HashSet<Vector2Int> blockedTiles, HashSet<Vector2Int> finalFloorTiles, HashSet<Vector2Int> finalCorridorTiles, HashSet<Vector2Int> finalWallTiles, Dictionary<Vector2Int, FloorTheme> floorThemeByTile)
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        blockedTiles.Clear();
        finalFloorTiles.Clear();
        finalCorridorTiles.Clear();
        finalWallTiles.Clear();
        floorThemeByTile.Clear();
    }

    private void AddZoneThemes(List<DungeonRoom> dungeonRooms, HashSet<Vector2Int> roomTiles, HashSet<Vector2Int> finalWallTiles, Dictionary<Vector2Int, FloorTheme> floorThemeByTile)
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

    private TileBase Tile2ThemeSetter(FloorTheme theme, TileBase floorTile, TileBase stoneTile, TileBase woodTile, TileBase metalTile, TileBase dirtTile, TileBase carpetTile, TileBase demonicTile)
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