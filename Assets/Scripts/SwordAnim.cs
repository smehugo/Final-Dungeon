using UnityEngine;

public class SwordAnim : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sr;

    private static int Attack = Animator.StringToHash("Attack");

    public void SetDir(Vector2 direction)
    {
        float angle;

        if (direction.x > 0f)
            angle = 0f;
        else if (direction.x < 0f)
            angle = 180f;
        else if (direction.y > 0f)
            angle = 90f;
        else
            angle = -90f;

        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void Swing()
    {
        animator.SetTrigger(Attack);
    }
}