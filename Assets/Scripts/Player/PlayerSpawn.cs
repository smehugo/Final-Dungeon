using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private BSPGen generator;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask blockLayers;
    [SerializeField] private float clearance = 0.3f;

    private void Start()
    {
        StartCoroutine(SpawnReady());
    }

    private IEnumerator SpawnReady()
    {
        yield return new WaitUntil(() => generator != null && generator.MapData != null);

        yield return new WaitForFixedUpdate();

        var mapData = generator.MapData;
        Vector2Int spawnTile = GetSpawnTile(mapData);
        player.position = mapData.GetWorldPosFromTile(spawnTile);
    }

    private Vector2Int GetSpawnTile(DungeonMapData mapData)
    {
        //start room
        var startRoom = mapData.rooms.Find(r => r.isStartRoom);
        if (startRoom == null) return default;

        var reachable = Reachability.FloodFill(mapData, FloodOrigin(mapData, startRoom));

        var good = new List<Vector2Int>();
        var usable = new List<Vector2Int>();

        for (int x = startRoom.bounds.xMin; x < startRoom.bounds.xMax; x++)
        {
            for (int y = startRoom.bounds.yMin; y < startRoom.bounds.yMax; y++)
            {
                var tile = new Vector2Int(x, y);

                if (!mapData.IsFree(tile)) continue;
                if (!reachable.Contains(tile)) continue;
                if (!IsClear(mapData, tile)) continue;

                usable.Add(tile);
                if (Clearance(mapData, tile)) good.Add(tile);
            }
        }

        if (good.Count > 0) return good[Random.Range(0, good.Count)];
        if (usable.Count > 0) return usable[Random.Range(0, usable.Count)];
        return startRoom.center;
    }

    private Vector2Int FloodOrigin(DungeonMapData mapData, DungeonRoom startRoom)
    {
        foreach (var door in startRoom.doors)
            if (mapData.IsWalkable(door.position)) return door.position;

        for (int x = startRoom.bounds.xMin; x < startRoom.bounds.xMax; x++)
            for (int y = startRoom.bounds.yMin; y < startRoom.bounds.yMax; y++)
            {
                var tile = new Vector2Int(x, y);
                if (mapData.IsWalkable(tile)) return tile;
            }

        return startRoom.center;
    }

    // all neighbours free
    private bool Clearance(DungeonMapData mapData, Vector2Int tile)
    {
        return mapData.IsFree(tile + Vector2Int.up)
            && mapData.IsFree(tile + Vector2Int.down)
            && mapData.IsFree(tile + Vector2Int.left)
            && mapData.IsFree(tile + Vector2Int.right);
    }

    // tests on colliders, not map data
    private bool IsClear(DungeonMapData mapData, Vector2Int tile)
    {
        if (blockLayers == 0) return true;

        Vector3 world = mapData.GetWorldPosFromTile(tile);
        Vector2 centre = new Vector2(world.x, world.y);
        return Physics2D.OverlapCircle(centre, clearance, blockLayers) == null;
    }
}