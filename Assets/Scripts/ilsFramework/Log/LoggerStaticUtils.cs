using JetBrains.Annotations;
using UnityEngine;

namespace ilsFramework
{
    public static class Logger
    {
        /// <summary>
        /// 发送Console消息(Log)
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
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

        /// <summary>
        /// 发送警告消息
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
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
        /// <summary>
        /// 发送错误消息
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
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
        /// <summary>
        /// 发送Console消息(Log)
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
        public static void LogSelf(this object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            Log(message, context, showTime, showThreadID, showStackTrace, colorConvert);
        }
        /// <summary>
        /// 发送警告消息
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
        public static void WarningSelf(this object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            Warning(message, context, showTime, showThreadID, showStackTrace, colorConvert);
        }
        /// <summary>
        /// 发送错误消息
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
        public static void ErrorSelf(this object message, Object context = null, bool showTime = true, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            Error(message, context, showTime, showThreadID, showStackTrace, colorConvert);
        }
    }
}