using System;
using System.Diagnostics;
using Object = UnityEngine.Object;

public static class Logs {
    public const string EnableLogsString = "ENABLE_LOGS";


    public static bool IsEnable() {
#if ENABLE_LOGS
			return true;
#else
        return false;
#endif
    }

    [Conditional(EnableLogsString)]
    public static void Log(string logMsg) {
        UnityEngine.Debug.Log(logMsg);
    }


    [Conditional(EnableLogsString)]
    public static void Log(object message, Object context) {
        UnityEngine.Debug.Log(message, context);
    }

    [Conditional(EnableLogsString)]
    public static void LogFormat(string format, params object[] args) {
        UnityEngine.Debug.LogFormat(format, args);
    }

    [Conditional(EnableLogsString)]
    public static void LogFormat(Object context, string format, params object[] args) {
        UnityEngine.Debug.LogFormat(context, format, args);
    }

    public static void LogError(object message) {
        UnityEngine.Debug.LogError(message);
#if PRODUCTION_BUILD && FIREBASE_ENABLE
		//Events.LogError(message.ToString());
#endif
    }


    public static void LogError(object message, Object context) {
        UnityEngine.Debug.LogError(message, context);
#if PRODUCTION_BUILD && FIREBASE_ENABLE
		//Events.LogError(message.ToString());
#endif
    }

    public static void LogErrorFormat(string format, params object[] args) {
        UnityEngine.Debug.LogErrorFormat(format, args);
#if PRODUCTION_BUILD && FIREBASE_ENABLE
		//Events.LogError(message.ToString());
#endif
    }

    public static void LogErrorFormat(Object context, string format, params object[] args) {
        UnityEngine.Debug.LogErrorFormat(context, format, args);
#if PRODUCTION_BUILD && FIREBASE_ENABLE
		//Events.LogError(message.ToString());
#endif
    }

    public static void LogException(Exception exception) {
        UnityEngine.Debug.LogException(exception);
#if PRODUCTION_BUILD && FIREBASE_ENABLE
		//Events.LogError(message.ToString());
#endif
    }

    public static void LogException(Exception exception, Object context) {
        UnityEngine.Debug.LogException(exception, context);
#if PRODUCTION_BUILD && FIREBASE_ENABLE
		//Events.LogError(message.ToString());
#endif
    }

    [Conditional(EnableLogsString)]
    public static void LogWarning(object message) {
        UnityEngine.Debug.LogWarning(message);
    }

    [Conditional(EnableLogsString)]
    public static void LogWarning(object message, Object context) {
        UnityEngine.Debug.LogWarning(message, context);
    }

    [Conditional(EnableLogsString)]
    public static void LogWarningFormat(string format, params object[] args) {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    [Conditional(EnableLogsString)]
    public static void LogWarningFormat(Object context, string format, params object[] args) {
        UnityEngine.Debug.LogWarningFormat(context, format, args);
    }
}