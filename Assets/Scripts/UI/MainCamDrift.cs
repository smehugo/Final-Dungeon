using UnityEngine;

public class MainCamDrift : MonoBehaviour
{
    [SerializeField] private float speed = 0.15f;
    [SerializeField] private float wobb = 0.5f;

    private Vector3 start;
    private float xMin = 165f;
    private float xMax = 219f;
    private float yCenter = 271f;
    private float zPos = -10f;

    private void Awake()
    {
        start = transform.position;
    }

    private void Update()
    {
        float t = Time.unscaledTime * speed;
        float xCenter = (xMin + xMax) / 2f;
        float xAmp = (xMax - xMin) / 2f;
        float x = xCenter + Mathf.Sin(t) * xAmp;
        float y = yCenter + Mathf.Cos(t * 4f) * wobb;
        float z = zPos;

        transform.position = new Vector3(x, y, z);
    }
}