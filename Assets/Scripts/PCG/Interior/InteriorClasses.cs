using UnityEngine;
using System.Collections.Generic;

public enum ZoneType
{
    Empty,
    Enemy,
    Treasure,
}

public enum FloorTheme
{
    Default,
    Stone,
    Wood,
    Metal,
    Dirt,
    Carpet,
    Demonic,
}

public class DungeonRoom
{
    public int id;
    public RectInt bounds;
    public Vector2Int center;
    public List<DoorData> doors = new();
    public HashSet<Vector2Int> reservedTiles = new();
    public List<RoomZone> zones = new();
    public List<InteriorWall> interiorWalls = new();

    public bool isStartRoom;
    public bool isFinalRoom;
    public bool hasArtifact;
}

public class DoorData
{
    public Vector2Int position;
    public Vector2Int inwardDir;
}

public class RoomZone
{
    public RectInt bounds;
    public ZoneType type;
    public FloorTheme theme;
}

public class InteriorWall
{
    public RectInt bounds;
    public bool isVertical;
    public List<Vector2Int> tiles = new();
}