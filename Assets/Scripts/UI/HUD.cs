using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text artifactText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthBar;

    private void Update()
    {
        artifactText.text = $"Artifacts {ArtifactManager.collected} / {ArtifactManager.total}";

        healthText.text = $"HP {playerHealth.Current} / {playerHealth.Max}";

        healthBar.maxValue = playerHealth.Max;
        healthBar.value = playerHealth.Current;
    }
}