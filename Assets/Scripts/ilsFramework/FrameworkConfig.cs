using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif


namespace ilsFramework
{
    /// <summary>
    /// 框架配置，通过这个SO 加载一些框架需要配置的东西
    /// </summary>
    public class FrameworkConfig : ConfigScriptObject
    {
        public override string ConfigName => "FrameworkConfig";

        [LabelText("Config顺序")]
        [HideLabel] 
        [SerializeField]
        [ShowInInspector]
        [ListDrawerSettings(HideAddButton = true,HideRemoveButton = true,DraggableItems = true,ShowFoldout = true,ShowIndexLabels = false)]
#if UNITY_EDITOR
        [OnCollectionChanged("OnSortListChanged")]
#endif
        [PropertyOrder(int.MaxValue)]
        private List<ReadOnlyString> ConfigsViewSort;
        private Dictionary<string,int> ConfigViewSort;
        
        
        [LabelText("Manager轮询/更新顺序")]
        [HideLabel] 
        [SerializeField]
        [ShowInInspector]
        [ListDrawerSettings(HideAddButton = true,HideRemoveButton = true,DraggableItems = true,ShowFoldout = true,ShowIndexLabels = false)]
        [PropertyOrder(int.MaxValue-1)]
        private List<ReadOnlyString> ManagersUpdateSort;
        private Dictionary<string,int> Dict_UpdateSort;

        public FrameworkConfig()
        {
            ConfigsViewSort = new List<ReadOnlyString>();
            ConfigViewSort = new Dictionary<string, int>();
            for (int i = 0; i < ConfigsViewSort.Count; i++)
            {
                ConfigViewSort[ConfigsViewSort[i].Value] =i;
            }
            
            ManagersUpdateSort = new List<ReadOnlyString>();
            //遍历程序集，查找所有Manager
#if UNITY_EDITOR
            ManagersUpdateSort = TypeCache.GetTypesDerivedFrom(typeof(IManager)).Select((type)=>new ReadOnlyString(type.FullName)).ToList();
            
            List<string> NeedPreSortManager = new List<string>()
            {
                "ilsFramework.ConfigManager", "ilsFramework.AssetManager", "ilsFramework.MonoManager"
            };
            
            ManagersUpdateSort = ManagersUpdateSort.OrderBy(s=>!NeedPreSortManager.Contains(s.Value))
                .ToList();
#endif
            
            Dict_UpdateSort = new Dictionary<string, int>();
            for (int i = 0; i < ManagersUpdateSort.Count; i++)
            {
                Dict_UpdateSort[ManagersUpdateSort[i].Value] = i;
            }
        }

        public void OnEnable()
        {
            ConfigViewSort = new Dictionary<string, int>();
            for (int i = 0; i < ConfigsViewSort.Count; i++)
            {
                ConfigViewSort[ConfigsViewSort[i].Value] =i;
            }
            
            Dict_UpdateSort = new Dictionary<string, int>();
            for (int i = 0; i < ManagersUpdateSort.Count; i++)
            {
                Dict_UpdateSort[ManagersUpdateSort[i].Value] = i;
            }
        }

        public int GetConfigSortOrder(string configName)
        {
            if (ConfigViewSort.TryGetValue(configName,out var value))
            {
                return value;
            }

            AddConfigSort(configName);
            return ConfigsViewSort.Count;
        }

        public void AddConfigSort(string configName)
        {
            ConfigsViewSort.Add(configName);
            ConfigViewSort[configName] = ConfigsViewSort.Count;
        }

        public void ReSortConfigShow()
        {
            ConfigsViewSort = ConfigsViewSort.OrderBy(s=>s.Value != "FrameworkConfig").ToList();
            
            ConfigViewSort = new Dictionary<string, int>();
            for (int i = 0; i < ConfigsViewSort.Count; i++)
            {
                ConfigViewSort[ConfigsViewSort[i].Value] =i;
            }
        }
        
        public void OnValidate()
        {

        }
#if UNITY_EDITOR
        private void OnSortListChanged(CollectionChangeInfo info)
        {
            ConfigViewSort = new Dictionary<string, int>();
            for (int i = 0; i < ConfigsViewSort.Count; i++)
            {
                ConfigViewSort[ConfigsViewSort[i].Value] =i;
            }
        }
#endif

        public int GetManagerUpdateIndex(Type mangerType)
        {
            if (Dict_UpdateSort.TryGetValue(mangerType.FullName, out var value))
            {
                return value;
            }
            else
            {
                ManagersUpdateSort.Add(new ReadOnlyString(mangerType.FullName));
                return ManagersUpdateSort.Count;
            }

        }
        
        [Serializable]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private class ReadOnlyString
        {
            [HideLabel]
            [Sirenix.OdinInspector.ReadOnly]
            public string Value;

            public ReadOnlyString(string value)
            {
                Value = value;
            }


            
            
            public static implicit operator ReadOnlyString(string value)
            {
                return new ReadOnlyString(value);
            }
            public static implicit operator string(ReadOnlyString value)
            {
                return value.Value;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj is ReadOnlyString)
                {
                    return Value.Equals(((ReadOnlyString)obj).Value);
                }
                return base.Equals(obj);
            }
        }
    }
}