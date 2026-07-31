using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private GameObject barRoot;
    [SerializeField] private Slider bar;

    private void Update()
    {
        bar.maxValue = enemyHealth.Max;
        bar.value = enemyHealth.Current;
        barRoot.SetActive(enemyHealth.Current < enemyHealth.Max && enemyHealth.Current > 0);
    }
}