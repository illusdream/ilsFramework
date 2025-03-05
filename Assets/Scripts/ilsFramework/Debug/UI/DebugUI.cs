using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ilsFramework
{
    [UIPanelSetting(EUILayer.Debug,0,true,EAssetLoadMode.Resources,"ilsFramework/Prefab/UI/DebugUI")]
    public class DebugUI : UIPanel
    {
        [AutoUIElement("Panel/Scroll View/Viewport/Content")]
        private RectTransform contentTransform;

        [AutoUIElement("Panel/Scroll View")]
        private ScrollRect scrollRect;
        
        [AutoUIElement("Panel/InputField")]
        private TMP_InputField inputField;
        
        private GameObject LogMessageShow;
        
        private Queue<GameObject> LogMessageList;
        
        private DebugConfig debugConfig;
        
        public override void InitUIPanel()
        {
            Application.logMessageReceived += HandleUnityLog;
            LogMessageList = new Queue<GameObject>();
            
            LogMessageShow = AssetManager.Instance.Load<GameObject>(EAssetLoadMode.Resources, "ilsFramework/Prefab/UI/DebugUI_LogMessage");

            debugConfig = ConfigManager.Instance.GetConfig<DebugConfig>();
            
            base.InitUIPanel();
        }


        public override void OnDestroy()
        {
            RemoveLogHandler();
            base.OnDestroy();
        }

        public void RemoveLogHandler()
        {
            Application.logMessageReceived -= HandleUnityLog;
        }
        
        private void HandleUnityLog(string log, string stackTrace, LogType type)
        {
            string logMessage = GetTextBeforeFirstNewline(log);
            var instance = GameObject.Instantiate(LogMessageShow, contentTransform);
            if (instance.GetComponentInChildren<TMP_Text>() is { } text)
            {
                switch (type)
                {
                    case LogType.Error:
                        logMessage =  ConvertTextColor(Color.red, logMessage);
                        break;
                    case LogType.Assert:
                        break;
                    case LogType.Warning:
                        logMessage =  ConvertTextColor(Color.yellow, logMessage);
                        break;
                    case LogType.Log:
                        break;
                    case LogType.Exception:
                        logMessage =  ConvertTextColor(Color.red, logMessage);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                text.text = logMessage;
            }

            LogMessageList.Enqueue(instance);
            while (LogMessageList.Count > debugConfig.MaxDebugUIStoreLogCount)
            {
                if (LogMessageList.TryDequeue(out var result))
                {
                    GameObject.Destroy(result);
                }
            }
            
            MonoManager.Instance.StartCoroutine(MoveToUnder());
            
            
            string GetTextBeforeFirstNewline(string input)
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input));

                int newlineIndex = input.IndexOfAny(new[] { '\r', '\n' });
                return newlineIndex >= 0 ? input.Substring(0, newlineIndex) : input;
            }
        }

        public void EnterMessage()
        {
            if (!DebugManager.Instance.TryExecuteCommand(inputField.text))
            {
               inputField.text.LogSelf();
            }
            inputField.text = "";
        }

        private IEnumerator MoveToUnder()
        {
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = -0.1f;
        }

        private string ConvertTextColor(Color color, string text)
        {
            var _color ="#"+ ColorUtility.ToHtmlStringRGB(color);
            return $"<color={_color}>{text}</color>"; 
        }
    }
}