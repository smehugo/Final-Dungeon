using UnityEngine;
using TMPro;

public class FloatPickupText : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private float speed = 1.5f;

    private float passed;
    private Color color;

    private void Awake()
    {
        text = GetComponentInChildren<TMP_Text>();
        color = text.color;
    }

    public void Popup(string message)
    {
        text = GetComponentInChildren<TMP_Text>();
        text.text = message;
    }

    private void Update()
    {
        passed += Time.deltaTime;
        transform.position += Vector3.up * speed * Time.deltaTime;

        var c = color;
        c.a = Mathf.Lerp(color.a, 0f, passed / duration);
        text.color = c;

        if (passed >= duration) Destroy(gameObject);
    }
}