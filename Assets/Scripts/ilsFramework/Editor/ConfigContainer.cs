using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ilsFramework.Editor
{
    [Serializable]
    public class ConfigContainer
    {
        [HideLabel] 
        [ListDrawerSettings(
            ShowFoldout = false,   // 不显示折叠箭头
            ShowIndexLabels = false, // 隐藏元素索引
            HideAddButton = true,  // 隐藏添加按钮（可选）
            HideRemoveButton = true, // 隐藏删除按钮（可选）
            DraggableItems = false // 禁止拖动元素（可选）
        )]
        [ShowInInspector]
        List<ConfigScriptObject> configScriptObjects;

        public ConfigContainer(string path)
        {
            configScriptObjects = new List<ConfigScriptObject>();
            //是文件夹
            if (AssetDatabase.IsValidFolder(path))
            {
                string[] guids = AssetDatabase.FindAssets(null, new[] { path });
                
                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                    if (asset !=null && asset is ConfigScriptObject configScriptObject)
                    {
                        configScriptObjects.Add(configScriptObject);
                    }
                }
            }
            else
            {
                ConfigScriptObject asset = AssetDatabase.LoadAssetAtPath<ConfigScriptObject>(path);
                if (asset != null)
                {
                    configScriptObjects.Add(asset);
                }
            }
            
        }

        public string GetConfigName()
        {
            return String.Empty;
            
        }
    }
}