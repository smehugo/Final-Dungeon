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
        }
    }
}