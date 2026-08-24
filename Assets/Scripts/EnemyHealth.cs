using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private Animator animator;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    [SerializeField] private GameObject[] loot;
    [SerializeField] private LootTableSO lootTable;
    [SerializeField] private float dropChance = 0.3f;
    [SerializeField] private float deathDuration = 1f;

    private int currentHealth;

    public int Current => currentHealth;
    public int Max => maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void AddHealth(int extra)
    {
        maxHealth += extra;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        if (currentHealth <= 0) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.enabled = false;
            CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();
            circleCollider.enabled = false;

            animator.SetTrigger("die");
            audioSource.PlayOneShot(deathSound);
            ChaserEnemy chaser = GetComponent<ChaserEnemy>();
            if (chaser != null)
            {
                chaser.SetDead();
                PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
                if (player != null && Random.value < 0.5f)
                {
                    player.AddMaxHealth(1);
                }
                StartCoroutine(DropLoot());
            }
            else
            {
                animator.SetTrigger("hurt");
                audioSource.PlayOneShot(hurtSound);
            }
        }
    }

    private IEnumerator DropLoot()
    {
        yield return new WaitForSeconds(deathDuration);

        if (Random.value < dropChance)
        {
            GameObject drop = RollLoot();
            Instantiate(drop, transform.position, Quaternion.identity, transform.parent);
        }

        Destroy(gameObject);
    }

    private GameObject RollLoot()
    {
        if (lootTable != null)
        {
            LootTableSO.Entry entry = lootTable.Roll();
            return entry.pickupPrefab;
        }

        if (loot != null && loot.Length > 0)
        {
            return loot[Random.Range(0, loot.Length)];
        }
        return null;
    }
}