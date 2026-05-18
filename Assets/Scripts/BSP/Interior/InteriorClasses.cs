using UnityEngine;
using System.Collections.Generic;

public class DungeonRoom
{
    public int id;
    public RectInt bounds;
    public Vector2Int center;
    public List<DoorData> doors = new();

    public bool isStartRoom;
    public bool isFinalRoom;
    public bool hasArtifact;
}

public class DoorData
{
    public Vector2Int position;
}