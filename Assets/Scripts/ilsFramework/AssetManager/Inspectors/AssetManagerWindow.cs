using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
#endif
using Sirenix.Utilities;

using UnityEngine;

namespace ilsFramework
{
#if UNITY_EDITOR
    public class AssetManagerWindow : OdinEditorWindow
    {
        [HideInInspector]
        public AssetConfig assetConfig;
        
        [ValueDropdown("GetAllAssetCollectionNames")]
        [OnValueChanged("UpdateSearch")]
        public string SelectedAssetCollection = "All";
        [OnValueChanged("UpdateSearch")]
        public string SearchBar = string.Empty;
        
        public List<AssetEditorInfo> AssetEditorInfos;

        [HideInInspector]
        public AssetBundle mainAB;
        [HideInInspector]
        public AssetBundleManifest mainManifest;
        
        public static void OpenWindow(AssetConfig assetConfig)
        {
            AssetChangeLog.GetConnect();
            var  _mainAB= AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + AssetBundleLoader.mainABName);
            var  _mainManifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            
            var window = GetWindow<AssetManagerWindow>();
            window.assetConfig = assetConfig;   
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(720, 720);
            window.titleContent = new GUIContent("Detail/Asset设置");
            window.mainAB = _mainAB;
            window.mainManifest = _mainManifest;
            window.UpdateSearch();
        }

        public List<string> GetAllAssetCollectionNames()
        {
            var list = assetConfig.AssetFilters.Select((a) => a.AssetKeyCollectionName).ToHashSet().ToList();
            list.Add("All");
            return list;
        }

        private void UpdateSearch()
        {
            var  assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            
            if (!assetDataBaseConnection.TryGetTable<AssetInfo>(out var table))
            {
                assetDataBaseConnection.CreateTable<AssetInfo>();
            }

            if (AssetEditorInfos != null)
            {
                foreach (var assetEditorInfo in AssetEditorInfos)
                {
                    assetEditorInfo.RefreshNeedUpdate(assetDataBaseConnection);
                }
            }
            
            
            
            List<AssetInfo> list = null;
            if (SelectedAssetCollection == "All")
            {
                list = (assetDataBaseConnection.Table<AssetInfo>().Where(predExpr: (a) => true)).ToList();
                list = list.Where((a)=>a.AssetKey.Contains(SearchBar)).ToList();
            }
            else
            {
                list = (assetDataBaseConnection.Table<AssetInfo>().Where(predExpr: (a) => a.AssetKey.Contains(SelectedAssetCollection))).ToList();
                list = list.Where((a)=>a.AssetKey.Contains(SearchBar)).ToList();
            }
            AssetEditorInfos = new List<AssetEditorInfo>();
            foreach (var assetInfo in list)
            {
                string[] s = assetInfo.AssetKey.Split('.');
                var instance = new AssetEditorInfo()
                {
                    ID = assetInfo.ID,
                    AssetCollectionName = s[0],
                    AssetKey = s[1],
                    AssetDescription = assetInfo.AssetDescription,
                    GUID = assetInfo.GUID,
                    AssetBundlesName = assetInfo.AssetBundleName
                };
                instance.allAssetbundleNames = mainManifest.GetAllAssetBundles().ToList();;
                AssetEditorInfos.Add(instance);
            }
            
            assetDataBaseConnection.Close();
        }

        private void OnInspectorUpdate()
        {
            foreach (var info in AssetEditorInfos)
            {
                info.Update();
            }
        }

        public void OnBecameInvisible()
        {
            mainAB?.Unload(true);
            if (AssetEditorInfos != null)
            {
                var  assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
                foreach (var assetEditorInfo in AssetEditorInfos)
                {
                    assetEditorInfo.RefreshNeedUpdate(assetDataBaseConnection);
                }
                assetDataBaseConnection.Close();
            }
        }
    
    }
#endif
}