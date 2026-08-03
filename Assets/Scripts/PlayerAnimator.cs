using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private float movementDeadZone = 0.01f;
    [SerializeField] private SwordAnim sword;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 lastDir = Vector2.down;
    private bool isDead;

    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Hurt = Animator.StringToHash("Hurt");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        AddFacingToAnim(lastDir);
        animator.SetFloat(Speed, 0f);
    }

    public void SetMovement(Vector2 input)
    {
        if (isDead)
            return;

        float speed = input.magnitude;
        animator.SetFloat(Speed, speed);

        if (speed > movementDeadZone)
        {
            lastDir = MixedDirection(input);
            AddFacingToAnim(lastDir);
        }
    }

    public Vector2 FacingDirection => lastDir;

    public void PlayAttack()
    {
        if (isDead)
            return;

        AddFacingToAnim(lastDir);
        animator.SetTrigger("Attack");

        sword.Swing();
    }

    public void PlayHurt()
    {
        if (isDead)
            return;

        AddFacingToAnim(lastDir);
        animator.SetTrigger(Hurt);
    }

    public void PlayDeath()
    {
        if (isDead)
            return;

        isDead = true;

        animator.SetFloat(Speed, 0f);
        AddFacingToAnim(lastDir);
        animator.SetBool(IsDead, true);
    }

    private void AddFacingToAnim(Vector2 direction)
    {
        Vector2 animDirection = direction;

        // left = flipped.
        if (direction.x < 0f)
        {
            animDirection = Vector2.right;
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        animator.SetFloat(MoveX, animDirection.x);
        animator.SetFloat(MoveY, animDirection.y);

        sword.SetDir(lastDir);
    }

    private Vector2 MixedDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > movementDeadZone)
        {
            return input.x > 0f ? Vector2.right : Vector2.left;
        }

        if (Mathf.Abs(input.y) > movementDeadZone)
        {
            return input.y > 0f ? Vector2.up : Vector2.down;
        }

        return lastDir;
    }
}