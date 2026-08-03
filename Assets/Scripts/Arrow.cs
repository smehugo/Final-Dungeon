using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float maxTime = 5f;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private float vol = 0.5f;

    private Rigidbody2D rb;
    private int damage;
    private bool done;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, int dmg)
    {
        damage = dmg;

        // sprite dir
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, maxTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (done)
        {
            return;
        }

        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            AudioSource.PlayClipAtPoint(hitSound, transform.position, vol);
        }
        Kill();
    }

    private void Kill()
    {
        done = true;
        Destroy(gameObject);
    }
}