using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField seedInput;
    [SerializeField] private TMP_Text seedModeTxt;
    [SerializeField] private TMP_Dropdown difficultyDrop;
    [SerializeField] private string sceneName = "ProceduralGen";

    [SerializeField] private Slider mapSizeSlider;
    [SerializeField] private Slider roomsSlider;
    [SerializeField] private Slider artifactsSlider;
    [SerializeField] private Slider roomFillSlider;

    [SerializeField] private TMP_Text mapSizeTxt;
    [SerializeField] private TMP_Text roomsTxt;
    [SerializeField] private TMP_Text artifactsTxt;
    [SerializeField] private TMP_Text roomFillTxt;

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
            // setSeed true, input empty
            if (int.TryParse(seedInput.text, out var seed))
            {
                RunConfig.Seed = seed;
            }
            else
            {
                RunConfig.Seed = Random.Range(0, int.MaxValue);
            }
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
                RunConfig.EnemyCountBonus = -2;
                RunConfig.DiffName = "Easy";
                break;
            case 2:
                RunConfig.EnemyCountBonus = 3;
                RunConfig.DiffName = "Hard";
                break;
            default:
                RunConfig.EnemyCountBonus = 0;
                RunConfig.DiffName = "Normal";
                break;
        }

        ApplyGenOptions();
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    // slider onvalue hook
    public void OnGenOptionChanged()
    {
        mapSizeTxt.text = "Map size: " + (int)mapSizeSlider.value;
        roomsTxt.text = "Room Count: " + (int)roomsSlider.value;
        artifactsTxt.text = "Artifacts Required: " + (int)artifactsSlider.value;
        roomFillTxt.text = "Room Coverage: " + roomFillSlider.value.ToString("0.00");
    }

    private void ApplyGenOptions()
    {
        RunConfig.UseCustomGen = true;
        RunConfig.MapSize = (int)mapSizeSlider.value;
        RunConfig.RoomCount = (int)roomsSlider.value;
        RunConfig.Artifacts = (int)artifactsSlider.value;
        RunConfig.RoomFill = roomFillSlider.value;
    }

    public void ResetGenOptions()
    {
        mapSizeSlider.value = 128;
        roomsSlider.value = 16;
        artifactsSlider.value = 5;
        roomFillSlider.value = 0.8f;
        OnGenOptionChanged();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
