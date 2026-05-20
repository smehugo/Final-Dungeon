using UnityEngine;
using System.Collections.Generic;

public class DungeonContentPlacer : MonoBehaviour
{
    [SerializeField] private List<SpawnDefinition> spawnDefs = new();

    private List<GameObject> spawnedObj = new();

    public void PlaceContent(DungeonMapData mapData)
    {
        ClearPrefabs();

        foreach (var def in spawnDefs)
        {
            foreach (var room in mapData.rooms)
            {
                if (def.avoidStartRoom && room.isStartRoom)
                { continue; }
                if (def.avoidFinalRoom && room.isFinalRoom)
                { continue; }
                if (Random.value > def.roomChance)
                { continue; }

                int range = Random.Range(def.minPerRoom, def.maxPerRoom + 1);
                for (int i = 0; i < range; i++)
                {
                    if (GetFreeTile(room, def, mapData, out Vector2Int tile))
                        SpawnObj(def, tile, mapData);
                }
            }
        }
    }

    private void ClearPrefabs()
    {
        foreach (var obj in spawnedObj)
        {
            DestroyImmediate(obj);
            Destroy(obj);
        }
        spawnedObj.Clear();
    }

    private bool GetFreeTile(DungeonRoom room, SpawnDefinition def, DungeonMapData mapData, out Vector2Int result)
    {
        var avaliables = new List<Vector2Int>();

        foreach (var zone in room.zones)
        {
            if (!zone.allowedTags.HasFlag(def.requiredTag)) continue;

            for (int x = zone.bounds.xMin; x < zone.bounds.xMax; x++)
            {
                for (int y = zone.bounds.yMin; y < zone.bounds.yMax; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (!IsValidSpawnCell(cell, room, def, mapData)) continue;
                    avaliables.Add(cell);
                }
            }
        }

        if (avaliables.Count == 0) { result = default; return false; }

        result = avaliables[Random.Range(0, avaliables.Count)];
        return true;
    }

    private void SpawnObj(SpawnDefinition def, Vector2Int tile, DungeonMapData mapData)
    {
        Vector3 worldPos = mapData.GetWorldPosFromTile(tile);
        GameObject obj = Instantiate(def.prefab, worldPos, Quaternion.identity, transform);
        spawnedObj.Add(obj);

        if (def.blocksMovement)
            mapData.Occupy(tile);
    }

    private bool IsValidSpawnCell(Vector2Int cell, DungeonRoom room, SpawnDefinition def, DungeonMapData mapData)
    {
        // better do now then later for validation
        if (!mapData.IsFree(cell)) return false;
        if (room.reservedTiles.Contains(cell)) return false;
        if (DistToDoor(room, cell) < def.minDistanceFromDoors) return false;
        return true;
    }

    private int DistToDoor(DungeonRoom room, Vector2Int cell)
    {
        int goated = int.MaxValue;
        foreach (var door in room.doors)
        {
            int dist = Mathf.Abs(cell.x - door.position.x) + Mathf.Abs(cell.y - door.position.y);
            if (dist < goated) goated = dist;
        }
        return goated;
    }
}