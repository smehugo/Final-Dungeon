using UnityEngine;

public class Artifact : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // let pickup handle collection for dupped artifact score
        if (GetComponent<Pickup>() != null)
        {
            Destroy(gameObject);
            return;
        }

        ArtifactManager.Collect();
        Destroy(gameObject);
        Debug.Log("artifact:" + ArtifactManager.collected + " / " + ArtifactManager.total);
    }
}
