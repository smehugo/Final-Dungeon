using UnityEngine;

public enum PickupType { Artifact, Health, Speed, Damage, Exit }

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private int amount = 1;
    [SerializeField] private GameObject floatText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case PickupType.Artifact:
                ArtifactManager.Collect();
                if (ArtifactManager.total > 0 && ArtifactManager.AllArtifactsCollected())
                    Popup("Final room unlocked - find the exit scroll!");
                else
                    Popup($"Artifact {ArtifactManager.collected} / {ArtifactManager.total}");
                break;
            case PickupType.Health:
                var pickupHealth = other.GetComponent<PlayerHealth>();
                pickupHealth.Heal(amount);
                Popup($"+{amount} Health");
                break;
            case PickupType.Speed:
                var pickupMovement = other.GetComponent<PlayerMovement>();
                pickupMovement.AddSpeed(amount);
                Popup($"+{amount} Speed");
                break;
            case PickupType.Damage:
                var pickupAttack = other.GetComponent<PlayerAttack>();
                pickupAttack.AddDamage(amount);
                Popup($"+{amount} Damage");
                break;
            case PickupType.Exit:
                var menu = FindFirstObjectByType<PauseMenu>();
                menu.ShowRunOver("DUNGEON ESCAPED");
                break;
        }
        Destroy(gameObject);
    }

    private void Popup(string message)
    {
        var obj = Instantiate(floatText, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        var text = obj.GetComponent<FloatPickupText>();
        text.Popup(message);
    }
}
