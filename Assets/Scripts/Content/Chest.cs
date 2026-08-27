using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject[] loot;
    [SerializeField] private GameObject artifact;
    [SerializeField, Range(0f, 1f)] private float artifactChance = 0.05f;
    [SerializeField] private LootTableSO lootTable;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float openDuration = 1f;

    private GameObject roll;
    private bool isOpen;

    private DungeonMapData mapData;
    private Vector2Int tile;

    public void Init(DungeonMapData mapData, Vector2Int tile)
    {
        this.mapData = mapData;
        this.tile = tile;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen || !other.CompareTag("Player")) return;

        isOpen = true;
        animator.SetBool("IsOpen", true);
        audioSource.PlayOneShot(openSound);
        roll = RollLoot();
        StartCoroutine(SpawnLoot());
    }

    private GameObject RollLoot()
    {
        if (lootTable != null)
        {
            LootTableSO.Entry entry = lootTable.Roll();
            return entry.pickupPrefab;
        }

        if (Random.value < artifactChance)
        {
            return artifact;
        }
        if (loot != null && loot.Length > 0)
        {
            return loot[Random.Range(0, loot.Length)];
        }
        return null;
    }

    private IEnumerator SpawnLoot()
    {
        yield return new WaitForSeconds(openDuration);

        Instantiate(roll, transform.position, Quaternion.identity, transform.parent);

        if (mapData != null)
        {
            mapData.Unoccupy(tile);
        }

        Destroy(gameObject);
    }
}