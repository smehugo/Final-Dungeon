using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxSpeed = 10f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private PlayerAnimator playerAnimator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    public void OnMove(InputValue val)
    {
        moveInput = val.Get<Vector2>();
        playerAnimator.SetMovement(moveInput);
    }

    private void FixedUpdate()
    {
        Vector2 movement = moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    public void AddSpeed(float delta)
    {
        moveSpeed = Mathf.Min(moveSpeed + delta, maxSpeed);
        Debug.Log("speed: " + moveSpeed);
    }
}
