using System;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    /// <summary>
    ///     资产引用，用于解决Config文件之类存储在Resource文件夹的资产与AssetBundle包资产的引用问题
    ///     在使用资产时尽量使用这个
    /// </summary>
    [Serializable]
    [InlineProperty]
    public class AssetReference<T> where T : Object
    {
        [ShowInInspector]
        [HideLabel]
#if UNITY_EDITOR
        [OnValueChanged("AssetChangeListener")]
#endif
        public T Asset;

        [ShowInInspector] [SerializeField] private EAssetLoadMode loadMode;

        [ShowInInspector] [SerializeField] private string loadStr;

        [ShowInInspector] [SerializeField] private bool isVaild;

        private string GUID;

        //获取到加载信息
        public bool TryGetAssetLoadInfo(out EAssetLoadMode loadMode, out string loadStr)
        {
            loadMode = this.loadMode;
            loadStr = this.loadStr;
            if (!isVaild) return false;
            return true;
        }
#if UNITY_EDITOR
        private void AssetChangeListener()
        {
            var path = AssetDatabase.GetAssetPath(Asset);
            if (string.IsNullOrEmpty(path))
            {
                "Resource is not in the project Assets folder.".LogSelf();
                return;
            }

            // 转换路径为 GUID
            var guid = AssetDatabase.AssetPathToGUID(path);
            $"GUID of '{Asset.name}': {guid}".LogSelf();

            var assetDataBaseConnection = DataBase.GetStreamingConnection(AssetManager.AssetDataBasePath);
            if (assetDataBaseConnection.TryGetTable<AssetInfo>(out var table))
            {
                //查询是否有对应的Key
                var resultList = table.Where(ai => ai.GUID == guid).ToList();

                //查询到结果
                if (resultList.Any())
                {
                    GUID = guid;
                    loadMode = EAssetLoadMode.AssetKey;
                    loadStr = resultList.First().AssetKey;
                }
                //没查到结果
                else
                {
                    //看看是AssetBundle还是Resources文件夹
                    //resource
                    if (Core.Asset.CheckAssetInResourcesFolder(path))
                    {
                        loadMode = EAssetLoadMode.Resources;
                        loadStr = path.Replace("Assets/Resources/", string.Empty).Replace(Path.GetExtension(path), string.Empty);
                    }
                    //assetBundle
                    else
                    {
                        var importer = AssetImporter.GetAtPath(path);
                        if (importer.assetBundleName == string.Empty) throw new ArgumentException($"{path}该资源不在Resourece文件夹内且不属于任意的AssetBundle，请分配资源存储方式");

                        loadMode = EAssetLoadMode.AssetBundle;
                        loadStr = importer.assetBundleName + $"/{Path.GetFileNameWithoutExtension(path)}";
                    }
                }

                isVaild = true;
            }
        }

        [Button]
        public void ReFreshReference()
        {
            AssetChangeListener();
        }
#endif
    }
}