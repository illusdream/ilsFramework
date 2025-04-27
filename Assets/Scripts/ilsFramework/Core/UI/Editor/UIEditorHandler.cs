using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ilsFramework
{
    
    public static class UIEditorHandler
    {
        [MenuItem("GameObject/ilsFramework/BasePanel", false)]
        static void CreateCustomObject(MenuCommand menuCommand) {
            // 创建新的GameObject
            GameObject customGO = new GameObject("BasePanel");
            
            customGO.layer = LayerMask.NameToLayer("UI");
            
            // 添加自定义组件
            customGO.AddComponent<RectTransform>();
            customGO.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            customGO.AddComponent<CanvasScaler>();
            customGO.AddComponent<GraphicRaycaster>();
            customGO.AddComponent<CanvasGroup>();
            
            GameObjectUtility.SetParentAndAlign(customGO, menuCommand.context as GameObject);
            
            Undo.RegisterCreatedObjectUndo(customGO, "Create " + customGO.name);
            Selection.activeObject = customGO;
        }
    }
}