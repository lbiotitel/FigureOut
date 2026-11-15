using System;
using UnityEngine;

public static class Logger
{


    public static void Log(string message)
    {
        Debug.Log($"{DateTime.Now} {message}");
    }

    public static void LogWarning(string message)
    {
        Debug.LogWarning($"{DateTime.Now} {message}");
    }

    public static void LogError(string message)
    {
        Debug.LogError($"{DateTime.Now} {message}");
    }
}
