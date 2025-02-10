using JetBrains.Annotations;
using UnityEngine;

namespace ilsFramework
{
    public static class Logger
    {
        public static void Log([CanBeNull] object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            if ((Application.isEditor && Application.isPlaying) || !Application.isEditor)
            {
                LoggerManager.Instance.Log(message,context,showTime,showThreadID,showStackTrace,colorConvert);
                return;
            }
            Debug.Log(LoggerManager.BuildLogMessage(message,showTime,showThreadID,showStackTrace,colorConvert),context);
        }

        public static void Warning([CanBeNull] object message, Object context = null, bool showTime = true, bool showThreadID = false,
            bool showStackTrace = false,
            string colorConvert = null)
        {
            if ((Application.isEditor && Application.isPlaying) || !Application.isEditor)
            {
                LoggerManager.Instance.LogWarning(message,context,showTime,showThreadID,showStackTrace,colorConvert);
                return;
            }
            Debug.LogWarning(LoggerManager.BuildLogMessage(message,showTime,showThreadID,showStackTrace,colorConvert),context);
        }

        public static void Error([CanBeNull] object message, Object context = null, bool showTime = true, bool showThreadID = false,
            bool showStackTrace = false,
            string colorConvert = null)
        {
            if ((Application.isEditor && Application.isPlaying) || !Application.isEditor)
            {
                LoggerManager.Instance.LogError(message,context,showTime,showThreadID,showStackTrace,colorConvert);
                return;
            }
            Debug.LogError(LoggerManager.BuildLogMessage(message,showTime,showThreadID,showStackTrace,colorConvert),context);
        }

        public static void LogSelf(this object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            Log(message, context, showTime, showThreadID, showStackTrace, colorConvert);
        }

        public static void WarningSelf(this object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            Warning(message, context, showTime, showThreadID, showStackTrace, colorConvert);
        }

        public static void ErrorSelf(this object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            Error(message, context, showTime, showThreadID, showStackTrace, colorConvert);
        }
    }
}