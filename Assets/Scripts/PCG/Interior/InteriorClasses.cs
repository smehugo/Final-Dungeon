using UnityEngine;
using System.Collections.Generic;
using System;

public enum ZoneType
{
    Empty,
    Enemy,
    Treasure,
    Open,
    Decoration,
    Artifact,
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

[Flags]
public enum ZoneSpawnTag
{
    // use bits to flag tags
    None = 0,
    Enemy = 1 << 0,
    Loot = 1 << 1,
    Light = 1 << 2,
    Decoration = 1 << 3,
    Obstacle = 1 << 4,
    Artifact = 1 << 5,
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

    public int difficultyTier;

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

    public ZoneSpawnTag allowedTags;
}

public class InteriorWall
{
    public RectInt bounds;
    public bool isVertical;
    public List<Vector2Int> tiles = new();
}