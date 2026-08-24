using UnityEngine;

public enum PickupType { Artifact, Health, Speed, Damage, Exit, FireRate }

public class Pickup : MonoBehaviour
{
    [SerializeField] private PickupType type;
    [SerializeField] private float amount = 1f;
    [SerializeField] private GameObject floatText;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField, Range(0f, 1f)] private float vol = 1f;
    [SerializeField] private int maxHealthBonus = 0;

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
                int healAmount = Mathf.RoundToInt(amount);

                if (maxHealthBonus > 0)
                {
                    pickupHealth.AddMoreMaxHealth(maxHealthBonus);
                    Popup($"+{healAmount} HP, Max Health +{maxHealthBonus}");
                }
                else
                {
                    pickupHealth.Heal(healAmount);
                    Popup($"+{healAmount} Health");
                }

                break;
            case PickupType.Speed:
                var pickupMovement = other.GetComponent<PlayerMovement>();
                pickupMovement.AddSpeed(amount);
                Popup($"+{amount:0.##} Speed");
                break;
            case PickupType.Damage:
                var pickupBow = other.GetComponent<Bow>();
                int addedDamage = Mathf.RoundToInt(amount);
                pickupBow.AddDamage(addedDamage);
                Popup($"+{addedDamage} Damage");
                break;
            case PickupType.FireRate:
                var rateBow = other.GetComponent<Bow>();
                rateBow.AddFireRate(amount);
                Popup($"+{amount:0.##} Fire Rate");
                break;
            case PickupType.Exit:
                var menu = FindFirstObjectByType<PauseMenu>();
                menu.ShowRunOver("DUNGEON ESCAPED");
                break;
        }
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, vol);
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
