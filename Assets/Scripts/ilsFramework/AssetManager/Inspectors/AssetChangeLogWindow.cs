#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using SQLite4Unity3d;
using UnityEngine;

namespace ilsFramework
{
    public class AssetChangeLogWindow : OdinEditorWindow
    {
        [ValueDropdown("GetAllMatchOptions",IsUniqueList = true)]
        [ListDrawerSettings(DraggableItems = false, HideRemoveButton = true,ShowFoldout = false)]
        [HideLabel]
        [OnValueChanged("UpdateSearch")]
        public List<AssetChangeType> MatchOption;
        [OnValueChanged("UpdateSearch")]
        public string SearchBar = string.Empty;
        
        [HideLabel]
        [TypeSelectorSettings(ShowNoneItem = false, ShowCategories = false, PreferNamespaces = false)]
        [ListDrawerSettings(
            ShowFoldout = false,   // 不显示折叠箭头
            ShowIndexLabels = false, // 隐藏元素索引
            HideAddButton = true,  // 隐藏添加按钮（可选）
            HideRemoveButton = true, // 隐藏删除按钮（可选）
            DraggableItems = false // 禁止拖动元素（可选）
        )]
        [InlineProperty]
        [ShowInInspector]
        private List<AssetChangeLogeDetailWindow> AssetChangeLogInfos;
        
        
        public static void OpenWindow()
        {
            var window = GetWindow<AssetChangeLogWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(720, 720);
            window.titleContent = new GUIContent("Detail/Asset修改记录");
            window.MatchOption =  new List<AssetChangeType>();
        }


        private List<AssetChangeType> GetAllMatchOptions()
        {
            var list = new List<AssetChangeType>();
            foreach (var _enum in Enum.GetValues(typeof(AssetChangeType)))
            {
                list.Add((AssetChangeType)_enum);
            }
            return list;
        }
        private void UpdateSearch()
        {
            MatchOption ??= new List<AssetChangeType>();
            
            HashSet<AssetChangeType> set = MatchOption.ToHashSet();
            var  assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            
            if (!assetDataBaseConnection.TryGetTable<AssetChangeLog>(out var table))
            {
                assetDataBaseConnection.CreateTable<AssetChangeLog>();
            }
            
            var list = (assetDataBaseConnection.Table<AssetChangeLog>().Where(predExpr: (a) => set.Contains(a.ChangeType))).ToList();
            AssetChangeLogInfos = new List<AssetChangeLogeDetailWindow>();
            SQLiteConnection connection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            foreach (var assetChangeLog in list)
            {
                AssetChangeLogeDetailWindow instance = assetChangeLog.ChangeType switch
                {
                    AssetChangeType.ChangeKeyName => new ChangeKeyNameWindow(assetChangeLog,connection),
                    AssetChangeType.ChangeCollection => new ChangeCollectionWindow(assetChangeLog,connection),
                    AssetChangeType.ChangeDescription => new ChangeDescriptionWindow(assetChangeLog,connection),
                    AssetChangeType.AssetRemove => new AssetRemoveWindow(assetChangeLog,connection),
                    AssetChangeType.ChangeUseAssetBundle => new ChangeUseAssetBundleWindow(assetChangeLog,connection),
                    AssetChangeType.ChangeAssetBundle => new ChangeAssetBundleWindow(assetChangeLog,connection),
                    AssetChangeType.ChangeAssetName => new ChangeAssetNameWindow(assetChangeLog,connection),
                    AssetChangeType.ChangeAssetPath => new ChangeAssetPathWindow(assetChangeLog,connection),
                    _ => null
                };
                if (instance != null)
                {
                    instance.NeedFresh += Refresh;
                    AssetChangeLogInfos.Add(instance);
                }
            }
            AssetChangeLogInfos= AssetChangeLogInfos.Where((log) => log.Title().Contains(SearchBar)).ToList();
            
            AssetChangeLogInfos.Sort((a,b)=>a.Date.CompareTo(b.Date) * -1);
            connection.Close();
        }

        private void Refresh()
        {
            UpdateSearch();
        }
    }
}
#endif