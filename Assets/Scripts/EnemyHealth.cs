using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private Animator animator;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip hurtSound;

    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            animator.SetTrigger("die");
            audioSource.PlayOneShot(deathSound);
            ChaserEnemy chaser = GetComponent<ChaserEnemy>();
            if (chaser != null)
            {
                chaser.SetDead();
            }
            Destroy(gameObject, 2f);
        }
        else
        {
            animator.SetTrigger("hurt");
            audioSource.PlayOneShot(hurtSound);
        }
    }
}