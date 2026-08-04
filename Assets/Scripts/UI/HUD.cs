using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Slider healthBar;

    [SerializeField] private Transform artifactRow;
    [SerializeField] private Image artifactImg;
    [SerializeField] private Color lockedTint = new Color(0.08f, 0.08f, 0.08f, 0.85f);
    [SerializeField] private Color collectedCol = Color.white;

    private List<Image> artifactIcons = new List<Image>();
    private int totalArti = -1;

    private void Update()
    {
        healthText.text = $"HP {playerHealth.Current} / {playerHealth.Max}";

        healthBar.maxValue = playerHealth.Max;
        healthBar.value = playerHealth.Current;

        if (totalArti != ArtifactManager.total)
            AddArtifactImg();

        for (int i = 0; i < artifactIcons.Count; i++)
        {
            if (i < ArtifactManager.collected)
            {
                artifactIcons[i].color = collectedCol;
            }
            else
            {
                artifactIcons[i].color = lockedTint;
            }
        }
    }

    private void AddArtifactImg()
    {
        totalArti = ArtifactManager.total;
        artifactIcons.Clear();
        artifactImg.gameObject.SetActive(false);
        for (int i = artifactRow.childCount - 1; i >= 0; i--)
        {
            Transform child = artifactRow.GetChild(i);
            if (child != artifactImg.transform)
                Destroy(child.gameObject);
        }

        for (int i = 0; i < totalArti; i++)
        {
            Image icon = Instantiate(artifactImg, artifactRow);
            icon.gameObject.SetActive(true);
            artifactIcons.Add(icon);
        }
    }
}