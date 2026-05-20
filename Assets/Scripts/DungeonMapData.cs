using UnityEngine;
using System.Collections.Generic;

// expose BSPGen data to other scripts without exposing the class
// feel this is better for placement logic and separate it from generation
public class DungeonMapData
{
    public List<DungeonRoom> rooms;

    private HashSet<Vector2Int> floorTiles;
    private HashSet<Vector2Int> blockedTiles;
    private HashSet<Vector2Int> occupiedTiles;

    public DungeonMapData(List<DungeonRoom> rooms, HashSet<Vector2Int> floorTiles, HashSet<Vector2Int> blockedTiles)
    {
        this.rooms = rooms;
        this.floorTiles = new HashSet<Vector2Int>(floorTiles);
        this.blockedTiles = new HashSet<Vector2Int>(blockedTiles);
        occupiedTiles = new HashSet<Vector2Int>();
    }

    public bool IsWalkable(Vector2Int tile)
    {
        return floorTiles.Contains(tile) && !blockedTiles.Contains(tile);
    }

    public bool IsFree(Vector2Int tile)
    {
        return IsWalkable(tile) && !occupiedTiles.Contains(tile);
    }

    public void Occupy(Vector2Int tile)
    {
        occupiedTiles.Add(tile);
    }

    public Vector3 GetWorldPosFromTile(Vector2Int tile)
    {
        return new Vector3(tile.x + 0.5f, tile.y + 0.5f, 0f);
    }

}