using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private float attackOffset = 0.4f;
    [SerializeField] private float attackCD = 0.4f;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    private PlayerAnimator playerAnimator;
    private float nextAttackTime;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    public void OnAttack(InputValue val)
    {
        if (!val.isPressed || Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCD;
        playerAnimator.PlayAttack();

        Vector2 hitCenter = (Vector2)transform.position + playerAnimator.FacingDirection * attackOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitCenter, attackRange, enemyMask);
        if (hits.Length > 0)
        {
            audioSource.PlayOneShot(hitSound);
        }

        foreach (Collider2D hit in hits)
        {
            EnemyHealth health = hit.GetComponent<EnemyHealth>();
            health.TakeDamage(damage);
        }
    }

    public void AddDamage(int delta)
    {
        damage += delta;
        Debug.Log("damage: " + damage);
    }
}
