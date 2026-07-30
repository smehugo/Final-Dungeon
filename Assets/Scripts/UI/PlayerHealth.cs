using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public int Current => currentHealth;
    public int Max => maxHealth;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth > 0)
        {
            audioSource.PlayOneShot(hurtSound);
            Debug.Log("hp: " + currentHealth);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        Debug.Log("hp: " + currentHealth);
    }

    private void Die()
    {
        audioSource.PlayOneShot(deathSound);
        // TODO: respawn
        Debug.Log("died");
    }
}