using UnityEngine;

public static class EditorLog
{
    [System.Diagnostics.Conditional("TEST_BUILD")]
    public static void Log(object message)
    {
        Debug.Log(message);
    }

    [System.Diagnostics.Conditional("TEST_BUILD")]
    public static void LogWarning(object message)
    {
        Debug.LogWarning(message);
    }

    [System.Diagnostics.Conditional("TEST_BUILD")]
    public static void LogError(object message)
    {
        Debug.LogError(message);
    }
}