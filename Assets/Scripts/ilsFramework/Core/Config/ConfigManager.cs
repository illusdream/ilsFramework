using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     用于加载/生成静态配置
    ///     静态配置：在Unity中编辑/修改，并在打包后不能修改
    ///     因此使用SO作为主要配置手段
    /// </summary>
    public class ConfigManager : ManagerSingleton<ConfigManager>
    {
        public const string EditorFrameworkConfigPath = "Assets/Resources/ilsFramework/Configs/FrameworkConfig.asset";
        public const string RunTimeFrameworkConfigPath = "ilsFramework/Configs/FrameworkConfig";

        [ShowInInspector] private Dictionary<Type, object> _configs;
        
        public override void OnInit()
        {
            _configs = new Dictionary<Type, object>();

            //加载所有Config

            //获取目标文件夹
            var path = "ilsFramework/Configs";
            var loadAll = Resources.LoadAll(path);
            //添加到字典
            foreach (var _object in loadAll)
                if (_object.GetType().GetCustomAttributes(typeof(AutoBuildOrLoadConfig), true).Length > 0)
                    _configs.Add(_object.GetType(), _object);
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

        public T GetConfig<T>() where T : ScriptableObject
        {
            if (_configs.TryGetValue(typeof(T), out var result)) return (T)result;
            return null;
        }


#if UNITY_EDITOR
        public static void CheckFrameworkConfig()
        {
            var frameworkConfig = AssetDatabase.LoadAssetAtPath<FrameworkConfig>(EditorFrameworkConfigPath);
            if (frameworkConfig == null)
            {
                frameworkConfig = ScriptableObject.CreateInstance<FrameworkConfig>();
                AssetDatabase.CreateAsset(frameworkConfig, EditorFrameworkConfigPath);
            }

            AssetDatabase.Refresh();
        }
#endif
    }
}