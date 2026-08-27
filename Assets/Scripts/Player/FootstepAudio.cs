using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private float stepDistance = 1.75f;

    private Rigidbody2D rb;
    private Vector2 lastPos;
    private float distance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
        lastPos = rb.position;
    }

    private void FixedUpdate()
    {
        float moved = Vector2.Distance(rb.position, lastPos);
        lastPos = rb.position;

        if (moved < 0.001f)
        {
            distance = 0f;
            return;
        }

        distance -= moved;
        if (distance > 0f) return;

        distance = stepDistance;
        source.pitch = Random.Range(0.7f, 1.3f);
        source.Play();
    }
}