using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilsFramework
{
    public static class Config
    {
        public static T GetConfig<T>() where T : ConfigScriptObject
        {
            return ConfigManager.Instance.GetConfig<T>();
        }

#if UNITY_EDITOR
        public static T GetConfigInEditor<T>() where T : ConfigScriptObject
        {
            return  AssetDatabase.LoadAssetAtPath("Assets/Resources/ilsFramework/Configs/" + typeof(T).GetCustomAttribute<AutoBuildOrLoadConfig>().ConfigTargetPath+".asset", typeof(T)) as T;
        }
#endif

        
        public static FrameworkConfig GetFrameworkConfig()
        {
            return Resources.Load<FrameworkConfig>(ConfigManager.RunTimeFrameworkConfigPath);
        }
    }
}