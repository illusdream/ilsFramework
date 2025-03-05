using System;
using System.Reflection;
using UnityEngine;

namespace ilsFramework
{
    public static class Config
    {
        public static T GetConfig<T>() where T : ConfigScriptObject
        {
            return ConfigManager.Instance.GetConfig<T>();
        }

        public static FrameworkConfig GetFrameworkConfig()
        {
            return Resources.Load<FrameworkConfig>(ConfigManager.RunTimeFrameworkConfigPath);
        }
    }
}