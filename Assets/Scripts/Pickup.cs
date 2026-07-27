using UnityEngine;

public enum PickupType { Artifact, Health, Speed, Damage }

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private int amount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case PickupType.Artifact:
                ArtifactManager.Collect();
                break;
            case PickupType.Health:
                var pickupHealth = other.GetComponent<PlayerHealth>();
                if (pickupHealth != null) pickupHealth.Heal(amount);
                break;
            case PickupType.Speed:
                var pickupMovement = other.GetComponent<PlayerMovement>();
                if (pickupMovement != null) pickupMovement.AddSpeed(amount);
                break;
            case PickupType.Damage:
                var pickupAttack = other.GetComponent<PlayerAttack>();
                if (pickupAttack != null) pickupAttack.AddDamage(amount);
                break;
        }
        Destroy(gameObject);
    }
}
