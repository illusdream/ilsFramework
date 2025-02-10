using System;
using System.Linq;
using Sirenix.OdinInspector;
using SQLite4Unity3d;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework
{
    public class AssetManager : ManagerSingleton<AssetManager>,IManager
    {
        [ShowInInspector]
        private ResourceLoader resourceLoader;
        [ShowInInspector]
        private AssetBundleLoader assetBundleLoader;

        public const string AssetDataBasePath = "Assets";
        
        private SQLiteConnection assetDataBaseConnection;
        private TableQuery<AssetInfo> assetInfos;
        
        
        public void Init()
        {
            resourceLoader = new ResourceLoader();
            resourceLoader.Init();
            assetBundleLoader = new AssetBundleLoader();
            assetBundleLoader.Init();
            assetDataBaseConnection = DataBase.GetStreamingConnection(AssetDataBasePath);
            if (assetDataBaseConnection == null)
            {
                throw new NullReferenceException("AssetDataBaseConnection is null");
            }
            if (assetDataBaseConnection.TryGetTable<AssetInfo>(out var table))
            {
                assetInfos = table;
            }
            else
            {
                assetDataBaseConnection.CreateTable<AssetInfo>();
                assetInfos = assetDataBaseConnection.Table<AssetInfo>();
            }
        }

        public void Update()
        {
            
        }

        public void LateUpdate()
        {
           
        }

        public void FixedUpdate()
        {
          
        }

        public void OnDestroy()
        {
            
        }

        public void OnDrawGizmos()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            
        }

        public T LoadByResources<T>(string path) where T : UnityEngine.Object
        {
            return resourceLoader.Load<T>(path);
        }

        public void AsyncLoadByResources<T>(string path,Action<T> callback) where T : UnityEngine.Object
        {
            resourceLoader.LoadAsync(path,callback);
        }

        public T LoadByAssetBundle<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            return assetBundleLoader.LoadAsset<T>(assetBundleName, assetName);
        }

        public void AsyncLoadByAssetBundle<T>(string assetBundleName, string assetName, Action<T> callback) where T : UnityEngine.Object
        {
            assetBundleLoader.LoadAssetAsync<T>(assetBundleName, assetName, callback);
        }

        //通用同步加载
        public Object Load(string assetKey)
        {
            Object result = null;

            var qResult = assetInfos.Where((info) => info.AssetKey == assetKey);
            if (qResult ==null)
            {
                return null;
            }
            if (qResult.Any())
            {
                AssetInfo target = qResult.First();
                if (target.UseAssetBundle)
                {
                    result = assetBundleLoader.LoadAsset(target.AssetBundleName, target.AssetName);
                }
                else
                {
                    result = resourceLoader.Load(target.ResourcesTargetPath);
                }
            }
            else
            {
                $"不存在该Key:{assetKey},请检查代码".ErrorSelf();
            }
            return result;
        }
        //通用异步加载
        public void LoadAsync(string assetKey, Action<Object> callback)
        {
            var qResult = assetInfos.Where((info) => info.AssetKey == assetKey);
            if (qResult ==null)
            {
                return;
            }
            if (qResult.Any())
            {
                AssetInfo target = qResult.First();
                if (target.UseAssetBundle)
                {
                    assetBundleLoader.LoadAssetAsync(target.AssetBundleName, target.AssetName, callback);
                }
                else
                {
                    resourceLoader.LoadAsync(target.ResourcesTargetPath,callback);
                }
            }
            else
            {
                $"不存在该Key:{assetKey},请检查代码".ErrorSelf();
            }
        }
    }
}