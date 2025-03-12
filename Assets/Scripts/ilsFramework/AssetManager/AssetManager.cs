using System;
using System.Linq;
using Sirenix.OdinInspector;
using SQLite4Unity3d;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework
{
    public partial class AssetManager : ManagerSingleton<AssetManager>,IManager
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
        /// <summary>
        /// 使用同步加载位于Resources文件夹的资源
        /// </summary>
        /// <param name="path">相对于Resources文件夹的相对路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public T LoadByResources<T>(string path) where T : UnityEngine.Object
        {
            return resourceLoader.Load<T>(path);
        }

        /// <summary>
        /// 使用异步加载位于Resources文件夹的资源
        /// </summary>
        /// <param name="path">相对于Resources文件夹的相对路径</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void AsyncLoadByResources<T>(string path,Action<T> callback) where T : UnityEngine.Object
        {
            resourceLoader.LoadAsync(path,callback);
        }

        /// <summary>
        /// 使用同步加载位于AssetBundle内的资源
        /// </summary>
        /// <param name="assetBundleName">AssetBundle包名</param>
        /// <param name="assetName">资源名</param>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <returns></returns>
        public T LoadByAssetBundle<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            return assetBundleLoader.LoadAsset<T>(assetBundleName, assetName);
        }

        /// <summary>
        /// 使用异步加载位于AssetBundle内的资源
        /// </summary>
        /// <param name="assetBundleName">AssetBundle包名</param>
        /// <param name="assetName">资源名</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源的类型</typeparam>
        public void AsyncLoadByAssetBundle<T>(string assetBundleName, string assetName, Action<T> callback) where T : UnityEngine.Object
        {
            assetBundleLoader.LoadAssetAsync<T>(assetBundleName, assetName, callback);
        }

        /// <summary>
        /// AssetKey同步加载
        /// </summary>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public Object LoadByAssetKey(string assetKey)
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
        /// <summary>
        /// AssetKey异步加载
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="callback"></param>
        public void AsyncLoadByAssetKey(string assetKey, Action<Object> callback)
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

        /// <summary>
        ///  通用加载方式
        /// </summary>
        /// <param name="assetLoadMode">加载模式</param>
        /// <param name="assetLoadStr">加载所使用的字符串
        /// <para>Resources模式:Resource下的相对文件路径</para>
        /// <para>AssetBundle模式:{AssetBundle名}/{对应资源名}</para>
        /// <para>AssetKey模式:对应的AssetKey</para>
        /// </param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T Load<T>(EAssetLoadMode assetLoadMode, string assetLoadStr) where T : UnityEngine.Object
        {
            switch (assetLoadMode)
            {
                case EAssetLoadMode.Resources:
                    return LoadByResources<T>(assetLoadStr);
                case EAssetLoadMode.AssetBundle:
                    var key = StringUtils.SplitAtLastSlash(assetLoadStr);
                    return LoadByAssetBundle<T>(key.Item1, key.Item2);
                case EAssetLoadMode.AssetKey:
                    return LoadByAssetKey(assetLoadStr) as T;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetLoadMode), assetLoadMode, null);
            }
        }

        /// <summary>
        /// 异步通用加载
        /// </summary>
        /// <param name="assetLoadMode">加载模式</param>
        /// <param name="assetLoadStr">加载所使用的字符串
        /// <para>Resources模式:Resource下的相对文件路径</para>
        /// <para>AssetBundle模式:{AssetBundle名}/{对应资源名}</para>
        /// <para>AssetKey模式:对应的AssetKey</para>
        /// </param>
        /// <param name="callback">Resources/AssetBundle使用的回调</param>
        /// <param name="assetKeyModeCallBack">AssetKey模式使用的回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void LoadAsync<T>(EAssetLoadMode assetLoadMode, string assetLoadStr, Action<T> callback = null,Action<Object> assetKeyModeCallBack = null) where T : UnityEngine.Object
        {
            switch (assetLoadMode)
            {
                case EAssetLoadMode.Resources:
                    AsyncLoadByResources(assetLoadStr, callback);
                    break;
                case EAssetLoadMode.AssetBundle:
                    var key = StringUtils.SplitAtLastSlash(assetLoadStr);
                    AsyncLoadByAssetBundle(key.Item1,key.Item2,callback);
                    break;
                case EAssetLoadMode.AssetKey:
                    AsyncLoadByAssetKey(assetLoadStr, assetKeyModeCallBack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetLoadMode), assetLoadMode, null);
            }
        }
    }
}