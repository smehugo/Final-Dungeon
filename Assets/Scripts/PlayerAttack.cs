using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackRange = 0.6f;
    [SerializeField] private float attackOffset = 0.4f;
    [SerializeField] private float attackCD = 0.4f;
    [SerializeField] private LayerMask enemyMask;

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

        foreach (Collider2D hit in hits)
        {
            EnemyHealth health = hit.GetComponent<EnemyHealth>();
            health.TakeDamage(damage);
        }
    }
}
