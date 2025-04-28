using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    public class LoggerManager : ManagerSingleton<LoggerManager>
    {
        public bool DebugLogEnable = true;
        public bool ErrorLogEnable = true;
        public bool WanringLogEnable = true;
        
        public override void OnInit()
        {
            LoadConfig();
        }

        public override void OnUpdate()
        {
           
        }
        public override void OnLateUpdate()
        {
           
        }
        public override void OnLogicUpdate()
        {
           
        }
        public override void OnFixedUpdate()
        {
           
        }
        public override void OnDestroy()
        {
           
        }

        public override void OnDrawGizmos()
        {
           
        }
        public override void OnDrawGizmosSelected()
        {
            
        }

        private void LoadConfig()
        {
            var logConfig = ConfigManager.Instance.GetConfig<LogConfig>();
            DebugLogEnable = logConfig.DebugLogEnable;
            WanringLogEnable = logConfig.WanringLogEnable;
            ErrorLogEnable = logConfig.ErrorLogEnable;
        }

        /// <summary>
        ///     创建最终输出的文本
        /// </summary>
        /// <param name="message"></param>
        /// <param name="context"></param>
        /// <param name="showTime"></param>
        /// <param name="showStackTrace"></param>
        /// <param name="colorConvert"></param>
        /// <returns></returns>
        public static string BuildLogMessage([CanBeNull] object message, bool showTime = false, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            var sb = new StringBuilder();
            var logMessage = message?.ToString();
            if (colorConvert != null) logMessage = GenerateColorLogMessage(logMessage, colorConvert);
            if (showTime) sb.Append($"[{DateTime.Now:HH:mm:ss:fff}] ");

            if (showThreadID) sb.Append($"[ThreadID: {Thread.CurrentThread.ManagedThreadId}] ");
            sb.Append($"| {logMessage}");
            if (showStackTrace) sb.Append($"\n StackTrace:{GenerateStackTraceInfo()}");
            return sb.ToString();
        }


        private static string GenerateStackTraceInfo()
        {
            var st = new StackTrace(3, true);
            var stackTrace = "";
            for (var i = 0; i < st.FrameCount; i++)
            {
                var frame = st.GetFrame(i);
                stackTrace += $"\n   {frame.GetFileName()} | {frame.GetMethod()} | Line:{frame.GetFileLineNumber()}";
            }

            return stackTrace;
        }

        private static string GenerateColorLogMessage(string message, string colorConvert)
        {
            var regex_MatchColor = @"^#[0-9a-fA-F]{6}$";
            string color;
            if (Regex.IsMatch(colorConvert, regex_MatchColor))
                color = colorConvert;
            else
                color = "#" + ColorUtility.ToHtmlStringRGB(Color.gray);
            return $"<color={color}>{message}</color>";
        }

        /// <summary>
        ///     发送Console消息(Log)
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
        public void Log([CanBeNull] object message, Object context = null, bool showTime = false, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            if (!DebugLogEnable || !Application.isEditor) return;
            var finalMessage = BuildLogMessage(message, showTime, showThreadID, showStackTrace, colorConvert);
            Debug.Log(finalMessage, context);
        }

        /// <summary>
        ///     发送警告消息
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
        public void LogWarning([CanBeNull] object message, Object context = null, bool showTime = false, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            if (!WanringLogEnable || !Application.isEditor) return;
            var finalMessage = BuildLogMessage(message, showTime, showThreadID, showStackTrace, colorConvert);
            Debug.LogWarning(finalMessage, context);
        }

        /// <summary>
        ///     发送错误消息
        /// </summary>
        /// <param name="message">要发送的Message</param>
        /// <param name="context">从哪个Mono发送的</param>
        /// <param name="showTime">是否显示发送时间</param>
        /// <param name="showThreadID">是否显示线程ID</param>
        /// <param name="showStackTrace">是否显示堆栈消息</param>
        /// <param name="colorConvert">颜色设置</param>
        public void LogError([CanBeNull] object message, Object context = null, bool showTime = false, bool showThreadID = false, bool showStackTrace = false,
            string colorConvert = null)
        {
            if (!ErrorLogEnable || !Application.isEditor) return;
            var finalMessage = BuildLogMessage(message, showTime, showThreadID, showStackTrace, colorConvert);
            Debug.LogError(finalMessage, context);
        }
    }
}