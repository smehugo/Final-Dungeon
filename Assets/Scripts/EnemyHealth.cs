using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private Animator animator;

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
            Destroy(gameObject, 1f);
        }
        else
        {
            animator.SetTrigger("hurt");
        }
    }
}