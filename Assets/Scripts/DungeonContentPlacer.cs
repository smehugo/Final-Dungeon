using UnityEngine;
using System.Collections.Generic;

public class DungeonContentPlacer : MonoBehaviour
{
    [SerializeField] private List<SpawnDefinition> spawnDefs = new();

    private List<GameObject> spawnedObj = new();

    public void PlaceContent(DungeonMapData mapData)
    {
        ClearPrefabs();
        var placedPositions = new List<Vector2Int>();

        ArtifactManager.ResetArtis();

        var orderedDefs = new List<SpawnDefinition>(spawnDefs);
        orderedDefs.Sort((a, b) => a.priority.CompareTo(b.priority));

        foreach (var def in orderedDefs)
        {
            foreach (var room in mapData.rooms)
            {
                if (def.avoidStartRoom && room.isStartRoom)
                { continue; }
                if (def.avoidFinalRoom && room.isFinalRoom)
                { continue; }
                if (def.finalRoomOnly && !room.isFinalRoom)
                { continue; }
                if (Random.value > def.roomChance)
                { continue; }

                int bonus = Mathf.Min(room.difficultyTier * def.extraOnTier, def.maxExtraCount);

                // menu difficulty - offset to SO so it doesnt stack
                bool isEnemy = def.prefab != null && def.prefab.GetComponent<ChaserEnemy>() != null;
                int diffBonus;
                if (isEnemy)
                {
                    diffBonus = RunConfig.EnemyCountBonus;
                }
                else
                {
                    diffBonus = 0;
                }

                int min = Mathf.Max(0, def.minPerRoom + bonus + diffBonus);
                int max = Mathf.Max(min, def.maxPerRoom + bonus + diffBonus);
                int range = Random.Range(min, max + 1);

                for (int i = 0; i < range; i++)
                {
                    if (GetFreeTile(room, def, mapData, placedPositions, out Vector2Int tile))
                    {
                        SpawnObj(def, tile, mapData, room);
                        placedPositions.Add(tile);
                    }
                }
            }
        }
    }

    private void ClearPrefabs()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(child);
            else
                DestroyImmediate(child);
        }

        spawnedObj.Clear();
    }

    private bool GetFreeTile(DungeonRoom room, SpawnDefinition def, DungeonMapData mapData, List<Vector2Int> placedPositions, out Vector2Int result)
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
                    if (!IsValidSpawnCell(cell, room, def, mapData, placedPositions)) continue;
                    avaliables.Add(cell);
                }
            }
        }

        if (avaliables.Count == 0) { result = default; return false; }

        result = avaliables[Random.Range(0, avaliables.Count)];
        return true;
    }

    private void SpawnObj(SpawnDefinition def, Vector2Int tile, DungeonMapData mapData, DungeonRoom room)
    {
        Vector3 worldPos = mapData.GetWorldPosFromTile(tile);
        GameObject obj = Instantiate(def.prefab, worldPos, Quaternion.identity, transform);
        spawnedObj.Add(obj);

        if (obj.CompareTag("Artifact"))
            ArtifactManager.total++;

        // needs map + room bounds for bfs + activation
        ChaserEnemy enemy = obj.GetComponent<ChaserEnemy>();
        if (enemy != null)
            enemy.Init(mapData, room);

        EnemyHealth enemyHealth = obj.GetComponent<EnemyHealth>();
        if (enemyHealth != null && def.healthOnTier > 0)
        {
            enemyHealth.AddHealth(room.difficultyTier * def.healthOnTier);
        }

        if (enemy != null && def.damageOnTier > 0)
        {
            enemy.AddDamage(room.difficultyTier * def.damageOnTier);
        }

        if (def.blocksMovement)
        {
            mapData.Occupy(tile);

            if (!IsRoomTraversale(room, mapData))
            {
                // Revert
                Destroy(obj);
                mapData.Unoccupy(tile); // needs to be added
            }
        }
    }

    private bool IsValidSpawnCell(Vector2Int cell, DungeonRoom room, SpawnDefinition def, DungeonMapData mapData, List<Vector2Int> placedPositions)
    {
        // better do now then later for validation
        if (!mapData.IsFree(cell)) return false;
        if (room.reservedTiles.Contains(cell)) return false;
        if (DistToDoor(room, cell) < def.minDistanceFromDoors) return false;
        foreach (var placed in placedPositions)
        {
            int dist = Mathf.Abs(cell.x - placed.x) + Mathf.Abs(cell.y - placed.y);
            if (dist < def.minSpacingFromSelf)
                return false;
        }
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

    private bool IsRoomTraversale(DungeonRoom room, DungeonMapData mapData)
    {
        var validator = new RoomValidaton();
        return validator.IsRoomValid(room, mapData);
    }
}