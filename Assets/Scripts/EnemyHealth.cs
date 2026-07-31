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
            Instantiate(loot[Random.Range(0, loot.Length)], transform.position, Quaternion.identity, transform.parent);

        Destroy(gameObject);
    }
}