using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private BSPGen generator;
    [SerializeField] private Transform player;

    private void Start()
    {
        StartCoroutine(SpawnReady());
    }

    private IEnumerator SpawnReady()
    {
        yield return new WaitUntil(() => generator != null && generator.MapData != null);

        Vector2Int spawnTile = GetSpawnTile(generator.MapData);
        player.position = generator.MapData.GetWorldPosFromTile(spawnTile);
        generator.MapData.Occupy(spawnTile);
    }

    private Vector2Int GetSpawnTile(DungeonMapData mapData)
    {
        //start room
        var startRoom = mapData.rooms.Find(r => r.isStartRoom);
        if (startRoom == null) return default;

        // walkable tiles
        var walkable = new List<Vector2Int>();
        for (int x = startRoom.bounds.xMin; x < startRoom.bounds.xMax; x++)
        {
            for (int y = startRoom.bounds.yMin; y < startRoom.bounds.yMax; y++)
            {
                var tile = new Vector2Int(x, y);
                if (mapData.IsWalkable(tile))
                    walkable.Add(tile);
            }
        }

        if (walkable.Count > 0)
            return walkable[UnityEngine.Random.Range(0, walkable.Count)];

        return startRoom.center;
    }
}