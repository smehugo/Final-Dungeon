using UnityEngine;

public class SealHint : MonoBehaviour
{
    [SerializeField] private GameObject floatText;
    [SerializeField] private float cooldown = 2f;

    private float nextTime;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (Time.time < nextTime) return;

        nextTime = Time.time + cooldown;

        var obj = Instantiate(floatText, collision.transform.position + Vector3.up * 0.5f, Quaternion.identity);
        obj.GetComponent<FloatPickupText>().Popup($"Collect all {ArtifactManager.total} artifacts first");
    }
}