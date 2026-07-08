using System;
using System.Collections.Generic;
using UnityEngine;

// metrics logger
public static class PcgMetricsRecorder
{
    public static void Record(string testName, bool passed, Dictionary<string, object> metrics = null)
    {
        var msg = $"{testName} passed={passed}";
        if (metrics != null)
        {
            foreach (var kv in metrics)
            {
                msg += $", {kv.Key}={kv.Value}";
            }
        }
        Debug.Log(msg);
    }
}
