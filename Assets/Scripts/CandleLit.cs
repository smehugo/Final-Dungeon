using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CandleLightTrigger : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Light2D light2D;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip litSound;

    private bool isLit;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        light2D = GetComponent<Light2D>();
        light2D.enabled = false;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            LightCandle();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLit)
            return;

        if (!other.CompareTag(playerTag))
            return;

        LightCandle();
    }

    private void LightCandle()
    {
        isLit = true;
        animator.SetBool("IsLit", true);
        light2D.enabled = true;
        if (SceneManager.GetActiveScene().name != "MainMenu")
            audioSource.PlayOneShot(litSound);
    }
}