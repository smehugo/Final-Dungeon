using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text artifactText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private PlayerHealth playerHealth;

    private void Update()
    {
        artifactText.text = $"Artifacts {ArtifactManager.collected} / {ArtifactManager.total}";

        healthText.text = $"HP {playerHealth.Current} / {playerHealth.Max}";
    }
}