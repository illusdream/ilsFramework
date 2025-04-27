using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace ilsFramework.Core
{
#if UNITY_EDITOR
    public class AssetManagerWindow : OdinEditorWindow
    {
        [HideInInspector] public Core.AssetConfig assetConfig;

        [ValueDropdown("GetAllAssetCollectionNames")] [OnValueChanged("UpdateSearch")]
        public string SelectedAssetCollection = "All";

        [OnValueChanged("UpdateSearch")] public string SearchBar = string.Empty;

        public List<AssetEditorInfo> AssetEditorInfos;

        [HideInInspector] public AssetBundle mainAB;

        [HideInInspector] public AssetBundleManifest mainManifest;

        public void OnBecameInvisible()
        {
            mainAB?.Unload(true);
            if (AssetEditorInfos != null)
            {
                var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
                foreach (var assetEditorInfo in AssetEditorInfos) assetEditorInfo.RefreshNeedUpdate(assetDataBaseConnection);
                assetDataBaseConnection.Close();
            }
        }

        private void OnInspectorUpdate()
        {
            foreach (var info in AssetEditorInfos) info.Update();
        }

        public static void OpenWindow(Core.AssetConfig assetConfig)
        {
            AssetChangeLog.GetConnect();
            var _mainAB = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + AssetBundleLoader.mainABName);
            var _mainManifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

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
            var list = assetConfig.AssetFilters.Select(a => a.AssetKeyCollectionName).ToHashSet().ToList();
            list.Add("All");
            return list;
        }

        private void UpdateSearch()
        {
            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);

            if (!assetDataBaseConnection.TryGetTable<AssetInfo>(out var table)) assetDataBaseConnection.CreateTable<AssetInfo>();

            if (AssetEditorInfos != null)
                foreach (var assetEditorInfo in AssetEditorInfos)
                    assetEditorInfo.RefreshNeedUpdate(assetDataBaseConnection);


            List<AssetInfo> list = null;
            if (SelectedAssetCollection == "All")
            {
                list = assetDataBaseConnection.Table<AssetInfo>().Where(a => true).ToList();
                list = list.Where(a => a.AssetKey.Contains(SearchBar)).ToList();
            }
            else
            {
                list = assetDataBaseConnection.Table<AssetInfo>().Where(a => a.AssetKey.Contains(SelectedAssetCollection)).ToList();
                list = list.Where(a => a.AssetKey.Contains(SearchBar)).ToList();
            }

            AssetEditorInfos = new List<AssetEditorInfo>();
            foreach (var assetInfo in list)
            {
                var s = assetInfo.AssetKey.Split('.');
                var instance = new AssetEditorInfo
                {
                    ID = assetInfo.ID,
                    AssetCollectionName = s[0],
                    AssetKey = s[1],
                    AssetDescription = assetInfo.AssetDescription,
                    GUID = assetInfo.GUID,
                    AssetBundlesName = assetInfo.AssetBundleName
                };
                instance.allAssetbundleNames = mainManifest.GetAllAssetBundles().ToList();
                ;
                AssetEditorInfos.Add(instance);
            }

            assetDataBaseConnection.Close();
        }
    }
#endif
}