using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject[] loot;
    [SerializeField] private GameObject artifact;
    [SerializeField, Range(0f, 1f)] private float artifactChance = 0.05f;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private float openDuration = 1f;

    private GameObject roll;
    private bool isOpen;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpen || !other.CompareTag("Player")) return;

        isOpen = true;
        animator.SetBool("IsOpen", true);
        audioSource.PlayOneShot(openSound);
        roll = Random.value < artifactChance ? artifact : loot[Random.Range(0, loot.Length)];
        StartCoroutine(SpawnLoot());
    }

    private IEnumerator SpawnLoot()
    {
        yield return new WaitForSeconds(openDuration);

        Instantiate(roll, transform.position, Quaternion.identity, transform.parent);
        Destroy(gameObject);
    }
}