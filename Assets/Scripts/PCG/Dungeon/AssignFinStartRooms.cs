using UnityEngine;
using System.Collections.Generic;

public class AssignFinStartRooms
{
    private List<DungeonRoom> dungeonRooms;
    private int artifactZones;

    public void AssignSpecialRooms(List<DungeonRoom> dungeonRooms, int artifactZones)
    {
        this.dungeonRooms = dungeonRooms;
        this.artifactZones = artifactZones;

        foreach (var room in dungeonRooms)
        {
            room.isStartRoom = false;
            room.isFinalRoom = false;
            room.hasArtifact = false;
        }

        if (dungeonRooms.Count == 0)
            return;

        DungeonRoom startRoom = dungeonRooms[0];

        foreach (var room in dungeonRooms)
        {
            if (room.center.x + room.center.y < startRoom.center.x + startRoom.center.y)
                startRoom = room;
        }

        startRoom.isStartRoom = true;

        DungeonRoom finalRoom = startRoom;
        float farthestDistance = 0f;

        foreach (var room in dungeonRooms)
        {
            if (room == startRoom)
                continue;

            float distance = Vector2Int.Distance(startRoom.center, room.center);

            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                finalRoom = room;
            }
        }

        finalRoom.isFinalRoom = true;

        List<DungeonRoom> artifactCandidates = new List<DungeonRoom>();

        foreach (var room in dungeonRooms)
        {
            if (!room.isStartRoom && !room.isFinalRoom)
                artifactCandidates.Add(room);
        }

        for (int i = artifactCandidates.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (artifactCandidates[i], artifactCandidates[j]) = (artifactCandidates[j], artifactCandidates[i]);
        }

        int count = Mathf.Min(artifactZones, artifactCandidates.Count);

        for (int i = 0; i < count; i++)
        {
            artifactCandidates[i].hasArtifact = true;
        }
    }
}