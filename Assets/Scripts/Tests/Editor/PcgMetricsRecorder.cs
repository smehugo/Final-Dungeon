using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// metrics logger
// csv goes outside Assets
public static class PcgMetricsRecorder
{
    // assets/../Pcgmetrics
    public static string OutputFolder => Path.Combine(Application.dataPath, "..", "PcgMetrics");

    public static void Record(string testName, bool passed, Dictionary<string, object> metrics = null)
    {
        var msg = $"{testName} passed={passed}";
        foreach (var kv in metrics)
        {
            msg += $", {kv.Key}={kv.Value}";
        }
        Debug.Log(msg);
    }

    public static string EnsureFile(string fileName, string header)
    {
        Directory.CreateDirectory(OutputFolder);
        string path = Path.Combine(OutputFolder, fileName);

        if (!File.Exists(path))
        {
            File.WriteAllText(path, header + "\n");
            return path;
        }

        return path;
    }

    public static void AppRow(string fileName, string row)
    {
        string path = Path.Combine(OutputFolder, fileName);
        File.AppendAllText(path, row + "\n");
    }

}