using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    public static int total;
    public static int collected;

    public static void Collect()
    {
        collected++;
    }

    public static void ResetArtis()
    {
        total = 0;
        collected = 0;
    }

    public static bool AllArtifactsCollected()
    {
        return collected >= total;
    }
}
