using UnityEngine;

[CreateAssetMenu(menuName = "FinalDungeon/SpawnDefSO")]
public class SpawnDefinition : ScriptableObject
{
    // prio
    [Header("Priority // lower number = higher prio")]
    public int priority = 50;

    [Header("Rules")]
    // make the SO, assign to placer, assign prefab to SO
    // DungeonMapData, DungeoonContentPlacer, SpawnDefSO
    public GameObject prefab;
    public ZoneSpawnTag requiredTag;

    public bool avoidStartRoom = true;
    public bool avoidFinalRoom = false;
    public bool finalRoomOnly = false;
    public int minDistanceFromDoors = 2;
    public int minPerRoom = 0;
    public int maxPerRoom = 2;
    [Range(0f, 1f)] public float roomChance = 1f;
    public bool blocksMovement = true;
    public int minSpacingFromSelf = 0;

    [Header("Enemy difficulty")]
    public int extraOnTier = 0;
    public int maxExtraCount = 3;
    public int healthOnTier = 0;


}