using UnityEngine;
using UnityEngine.InputSystem;

public class Bow : MonoBehaviour
{
    [SerializeField] private int dmg = 10;
    [SerializeField] private float fireCD = 0.5f;
    [SerializeField] private float minFireCD = 0.1f;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float spawnOfset = 0.4f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private BowAnim bowAnim;

    private Camera cam;
    private bool firing;
    private Vector2 aimDir = Vector2.down;

    private PlayerAnimator playerAnimator;
    private float nextFireTime;

    public int Dmg => dmg;
    public float FireCD => fireCD;
    public Vector2 AimDirection => aimDir;

    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
        cam = Camera.main;
    }

    public void OnAttack(InputValue val)
    {
        firing = val.isPressed;
    }

    private void Update()
    {
        UpdateAim();

        if (!firing || Time.time < nextFireTime)
            return;

        Fire();
    }

    private void Fire()
    {
        nextFireTime = Time.time + fireCD;
        playerAnimator.PlayAttack();

        Vector3 spawnPos = transform.position + (Vector3)(aimDir * spawnOfset);
        GameObject obj = Instantiate(arrowPrefab, spawnPos, Quaternion.identity);

        Arrow arrow = obj.GetComponent<Arrow>();
        arrow.Launch(aimDir, dmg);

        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }

    private void UpdateAim()
    {
        if (cam == null || Mouse.current == null)
            return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        float depth = Mathf.Abs(cam.transform.position.z - transform.position.z);
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, depth));

        Vector2 delta = (Vector2)world - (Vector2)transform.position;

        if (delta.sqrMagnitude < 0.0001f)
            return;

        aimDir = delta.normalized;

        if (bowAnim != null)
            bowAnim.SetAim(aimDir);
    }

    public void AddDamage(int diff)
    {
        dmg += diff;
    }

    public void AddFireRate(float diff)
    {
        fireCD = Mathf.Max(fireCD - diff, minFireCD);
        Debug.Log("fireCD: " + fireCD);
    }
}