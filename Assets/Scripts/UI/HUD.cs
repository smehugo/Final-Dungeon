using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthBar;

    [SerializeField] private Image[] artifactIcons;
    [SerializeField] private Color lockedTint = new Color(0.08f, 0.08f, 0.08f, 0.85f);
    [SerializeField] private Color collectedCol = Color.white;

    private void Update()
    {
        healthText.text = $"HP {playerHealth.Current} / {playerHealth.Max}";

        healthBar.maxValue = playerHealth.Max;
        healthBar.value = playerHealth.Current;

        for (int i = 0; i < artifactIcons.Length; i++)
        {
            artifactIcons[i].color = i < ArtifactManager.collected ? collectedCol : lockedTint;
        }
    }
}