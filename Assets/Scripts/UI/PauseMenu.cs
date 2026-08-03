using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject runOverPanel;
    [SerializeField] private TMP_Text runOverTitle;
    [SerializeField] private PlayerInput playerInput;

    private bool isPaused;
    private bool runOver;

    private void Start()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        runOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (runOver) return;
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            SetPaused(!isPaused);
    }

    private void SetPaused(bool paused)
    {
        isPaused = paused;
        pausePanel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;

        // player paused too cause timescale didnt affect it
        if (playerInput == null) return;
        if (paused) playerInput.DeactivateInput();
        else playerInput.ActivateInput();
    }

    public void Resume()
    {
        SetPaused(false);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("quit");
    }

    public void ShowRunOver(string message)
    {
        runOver = true;
        isPaused = false;

        pausePanel.SetActive(false);
        runOverPanel.SetActive(true);
        if (runOverTitle != null) runOverTitle.text = message;

        Time.timeScale = 0f;
        if (playerInput != null) playerInput.DeactivateInput();
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}