using UnityEngine;

public class BowAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    private static int Attack = Animator.StringToHash("Attack");

    public void SetAim(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        bool flip = Mathf.Abs(angle) > 90f;
        transform.localScale = new Vector3(1f, flip ? -1f : 1f, 1f);
    }

    public void Shoot()
    {
        animator.SetTrigger(Attack);
    }
}