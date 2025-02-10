using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
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
        [ListDrawerSettings(HideAddButton = true,HideRemoveButton = false,DraggableItems = true,ShowFoldout = false,ShowIndexLabels = false)]
#if true
        [OnCollectionChanged("OnSortListChanged")]
#endif
        [PropertyOrder(int.MaxValue)]
        private List<ReadOnlyString> ConfigsViewSort;
        private Dictionary<string,int> ConfigViewSort;
        
        
        
        

        public FrameworkConfig()
        {
            ConfigsViewSort = new List<ReadOnlyString>();
            ConfigViewSort = new Dictionary<string, int>();
            for (int i = 0; i < ConfigsViewSort.Count; i++)
            {
                ConfigViewSort[ConfigsViewSort[i].Value] =i;
            }
        }

        public void OnEnable()
        {
            ConfigViewSort = new Dictionary<string, int>();
            for (int i = 0; i < ConfigsViewSort.Count; i++)
            {
                ConfigViewSort[ConfigsViewSort[i].Value] =i;
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