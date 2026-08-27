using UnityEngine;

[CreateAssetMenu(menuName = "FinalDungeon/LootTable")]
public class LootTableSO : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public GameObject pickupPrefab;
        public float weight = 1f;

        public bool isArtifact;
    }

    public Entry[] entries;

    public Entry Roll()
    {
        if (entries == null || entries.Length == 0) return null;

        float totalWeight = 0f;
        foreach (var entry in entries) totalWeight += entry.weight;
        if (totalWeight <= 0f) return null;

        float pick = Random.Range(0f, totalWeight);
        float running = 0f;

        foreach (var entry in entries)
        {
            running += entry.weight;
            if (pick <= running) return entry;
        }

        return entries[entries.Length - 1];
    }
}