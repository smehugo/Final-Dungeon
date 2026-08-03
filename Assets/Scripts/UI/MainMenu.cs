using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private TMP_Text seedModeTxt;
    [SerializeField] private TMP_Dropdown difficultyDrop;
    [SerializeField] private string sceneName = "ProceduralGen";

    private bool useFixedSeed;

    private void Start()
    {
        ApplySeedMode();
    }

    public void ToggleSeedMode()
    {
        useFixedSeed = !useFixedSeed;
        ApplySeedMode();
    }

    private void ApplySeedMode()
    {
        if (seedModeTxt != null)
            if (useFixedSeed)
            {
                seedModeTxt.text = "Set Seed";
            }
            else
            {
                seedModeTxt.text = "Random Seed";
            }
        if (seedInput != null)
            seedInput.interactable = useFixedSeed;
    }

    public void Play()
    {
        if (useFixedSeed && seedInput != null)
        {
            RunConfig.UseFixedSeed = true;
            RunConfig.Seed = int.Parse(seedInput.text);
        }
        else
        {
            RunConfig.UseFixedSeed = false;
        }

        int choice;
        if (difficultyDrop != null)
        {
            choice = difficultyDrop.value;
        }
        else
        {
            choice = 1;
        }

        switch (choice)
        {
            case 0:
                RunConfig.EnemyCountBonus = -1;
                RunConfig.DiffName = "Easy";
                break;
            case 2:
                RunConfig.EnemyCountBonus = 2;
                RunConfig.DiffName = "Hard";
                break;
            default:
                RunConfig.EnemyCountBonus = 0;
                RunConfig.DiffName = "Normal";
                break;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
