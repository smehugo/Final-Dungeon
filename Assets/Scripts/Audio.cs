using UnityEngine;

public class Audio : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private float footstepInterval = 0.35f;

    private Rigidbody2D rb;
    private Vector2 lastPos;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        lastPos = rb.position;
    }

    private void FixedUpdate()
    {
        bool moving = Vector2.Distance(rb.position, lastPos) > 0.001f;
        lastPos = rb.position;

        if (!moving)
        {
            timer = 0f;
            return;
        }

        timer -= Time.fixedDeltaTime;
        if (timer > 0f)
            return;

        timer = footstepInterval + Random.Range(-0.02f, 0.02f);
        source.pitch = Random.Range(0.7f, 1.3f);
        source.Play();
    }
}