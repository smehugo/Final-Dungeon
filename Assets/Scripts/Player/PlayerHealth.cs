using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxHealthCap = 25;
    [SerializeField] private float regenInter = 4f;
    private float nextRegen;
    private int currentHealth;
    public int Current => currentHealth;
    public int Max => maxHealth;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    [SerializeField] private PauseMenu pauseMenu;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth <= 0)
            return;

        if (currentHealth >= maxHealth)
        {
            nextRegen = Time.time + regenInter;
            return;
        }

        if (Time.time < nextRegen)
            return;

        nextRegen = Time.time + regenInter;
        currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
    }

    public void TakeDamage(int dmg)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= dmg;
        nextRegen = Time.time + regenInter;
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
        Debug.Log("died");

        if (pauseMenu != null)
            pauseMenu.ShowRunOver("YOU DIED");
    }

    public void AddMaxHealth(int extra)
    {
        if (maxHealth >= maxHealthCap)
            return;

        maxHealth = Mathf.Min(maxHealth + extra, maxHealthCap);
        currentHealth = Mathf.Min(currentHealth + extra, maxHealth);
    }

    public void AddMoreMaxHealth(int extra)
    {
        maxHealthCap += extra;
        maxHealth += extra;
        currentHealth += extra;
    }

}