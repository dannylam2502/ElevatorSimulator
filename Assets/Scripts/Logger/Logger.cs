using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logger : MonoBehaviour
{
    public const string kTagReq = "req-";
    public const string kTagRes = "res-";
    public const string kTagError = "err-";
    
    // only log when using Unity Editor
#if UNITY_EDITOR
    public static bool isDebugOn = true;
#else
    public static bool isDebugOn = false;
#endif

    /// <summary>
    /// Wrapper Method of Debug.Log
    /// Logs a message to Unity Console
    /// </summary>
    /// <param name="message"> The message you would like to log</param>
    public static void Log(string tag, object message)
    {
        if (isDebugOn)
        {
            Debug.unityLogger.Log(tag, message);
        }
    }

    /// <summary>
    /// Wrapper method of Debug.LogError
    /// A variant of Debug.Log that logs an error message to the console
    /// </summary>
    /// <param name="message"> The message you would like to log</param>
    public static void LogError(string tag, object message)
    {
        if (isDebugOn)
        {
            Debug.unityLogger.LogError(tag, message);
        }
    }

    /// <summary>
    /// Wrapper method of Debug.LogWarning
    /// A variant of Debug.Log that logs a warning message to the console
    /// </summary>
    /// <param name="message"> The message you would like to log</param>
    public static void LogWarning(string tag, object message)
    {
        if (isDebugOn)
        {
            Debug.unityLogger.LogWarning(tag, message);
        }
    }

    /// <summary>
    /// Logs a formatted message
    /// </summary>
    /// <param name="logType">The type of log message</param>
    /// <param name="format">A composite format string</param>
    /// <param name="args"></param>
    public static void LogFormat(LogType logType, string format, params object[] args)
    {
        if (isDebugOn)
        {
            Debug.unityLogger.LogFormat(logType, format, args);
        }
    }
}
