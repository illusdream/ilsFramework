using System;
using System.Collections;
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
        
        public override void InitUIPanel()
        {
            Application.logMessageReceived += HandleUnityLog;
            
            
            LogMessageShow = AssetManager.Instance.Load<GameObject>(EAssetLoadMode.Resources, "ilsFramework/Prefab/UI/DebugUI_LogMessage");
            
            (contentTransform == null).LogSelf();
            inputField.LogSelf();
            LogMessageShow.LogSelf();
            
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
                text.text = logMessage;
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
            //inputField.text.LogSelf();
            DebugManager.Instance.TryExecuteCommand(inputField.text);
            inputField.text = "";
        }

        public IEnumerator MoveToUnder()
        {
            yield return new WaitForEndOfFrame();
            scrollRect.verticalNormalizedPosition = -0.1f;
        }
    }
}