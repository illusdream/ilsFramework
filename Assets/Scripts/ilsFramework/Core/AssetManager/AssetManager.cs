using System;
using System.Linq;
using ilsFramework.Core.SQLite4Unity3d;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    public class AssetManager : ManagerSingleton<AssetManager>
    {
        public const string AssetDataBasePath = "Assets";

        [ShowInInspector] private AssetBundleLoader assetBundleLoader;

        private SQLiteConnection assetDataBaseConnection;
        private TableQuery<AssetInfo> assetInfos;

        [ShowInInspector] private ResourceLoader resourceLoader;
        
        public override void OnInit()
        {
            resourceLoader = new ResourceLoader();
            resourceLoader.Init();
            assetBundleLoader = new AssetBundleLoader();
            assetBundleLoader.Init();
            assetDataBaseConnection = DataBase.GetStreamingConnection(AssetDataBasePath);
            if (assetDataBaseConnection == null) throw new NullReferenceException("AssetDataBaseConnection is null");
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

        /// <summary>
        ///     使用同步加载位于Resources文件夹的资源
        /// </summary>
        /// <param name="path">相对于Resources文件夹的相对路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public T LoadByResources<T>(string path) where T : Object
        {
            return resourceLoader.Load<T>(path);
        }

        /// <summary>
        ///     使用异步加载位于Resources文件夹的资源
        /// </summary>
        /// <param name="path">相对于Resources文件夹的相对路径</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        public void AsyncLoadByResources<T>(string path, Action<T> callback) where T : Object
        {
            resourceLoader.LoadAsync(path, callback);
        }

        /// <summary>
        ///     使用同步加载位于AssetBundle内的资源
        /// </summary>
        /// <param name="assetBundleName">AssetBundle包名</param>
        /// <param name="assetName">资源名</param>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <returns></returns>
        public T LoadByAssetBundle<T>(string assetBundleName, string assetName) where T : Object
        {
            return assetBundleLoader.LoadAsset<T>(assetBundleName, assetName);
        }

        /// <summary>
        ///     使用异步加载位于AssetBundle内的资源
        /// </summary>
        /// <param name="assetBundleName">AssetBundle包名</param>
        /// <param name="assetName">资源名</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源的类型</typeparam>
        public void AsyncLoadByAssetBundle<T>(string assetBundleName, string assetName, Action<T> callback) where T : Object
        {
            assetBundleLoader.LoadAssetAsync(assetBundleName, assetName, callback);
        }

        /// <summary>
        ///     AssetKey同步加载
        /// </summary>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public Object LoadByAssetKey(string assetKey)
        {
            Object result = null;

            var qResult = assetInfos.Where(info => info.AssetKey == assetKey);
            if (qResult == null) return null;
            if (qResult.Any())
            {
                var target = qResult.First();
                if (target.UseAssetBundle)
                    result = assetBundleLoader.LoadAsset(target.AssetBundleName, target.AssetName);
                else
                    result = resourceLoader.Load(target.ResourcesTargetPath);
            }
            else
            {
                $"不存在该Key:{assetKey},请检查代码".ErrorSelf();
            }

            return result;
        }

        /// <summary>
        ///     AssetKey异步加载
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="callback"></param>
        public void AsyncLoadByAssetKey(string assetKey, Action<Object> callback)
        {
            var qResult = assetInfos.Where(info => info.AssetKey == assetKey);
            if (qResult == null) return;
            if (qResult.Any())
            {
                var target = qResult.First();
                if (target.UseAssetBundle)
                    assetBundleLoader.LoadAssetAsync(target.AssetBundleName, target.AssetName, callback);
                else
                    resourceLoader.LoadAsync(target.ResourcesTargetPath, callback);
            }
            else
            {
                $"不存在该Key:{assetKey},请检查代码".ErrorSelf();
            }
        }

        /// <summary>
        ///     通用加载方式
        /// </summary>
        /// <param name="assetLoadMode">加载模式</param>
        /// <param name="assetLoadStr">
        ///     加载所使用的字符串
        ///     <para>Resources模式:Resource下的相对文件路径</para>
        ///     <para>AssetBundle模式:{AssetBundle名}/{对应资源名}</para>
        ///     <para>AssetKey模式:对应的AssetKey</para>
        /// </param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public T Load<T>(EAssetLoadMode assetLoadMode, string assetLoadStr) where T : Object
        {
            switch (assetLoadMode)
            {
                case EAssetLoadMode.Resources:
                    return LoadByResources<T>(assetLoadStr);
                case EAssetLoadMode.AssetBundle:
                    var key = ilsStringUtils.SplitAtLastSlash(assetLoadStr);
                    return LoadByAssetBundle<T>(key.Item1, key.Item2);
                case EAssetLoadMode.AssetKey:
                    return LoadByAssetKey(assetLoadStr) as T;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetLoadMode), assetLoadMode, null);
            }
        }

        /// <summary>
        ///     通用同步加载，使用<see cref="AssetReference{T}" />
        /// </summary>
        /// <param name="reference"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(AssetReference<T> reference) where T : Object
        {
            if (reference.TryGetAssetLoadInfo(out var assetLoadMode, out var loadStr)) return Load<T>(assetLoadMode, loadStr);
            return null;
        }

        /// <summary>
        ///     异步通用加载
        /// </summary>
        /// <param name="assetLoadMode">加载模式</param>
        /// <param name="assetLoadStr">
        ///     加载所使用的字符串
        ///     <para>Resources模式:Resource下的相对文件路径</para>
        ///     <para>AssetBundle模式:{AssetBundle名}/{对应资源名}</para>
        ///     <para>AssetKey模式:对应的AssetKey</para>
        /// </param>
        /// <param name="callback">Resources/AssetBundle使用的回调</param>
        /// <param name="assetKeyModeCallBack">AssetKey模式使用的回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void LoadAsync<T>(EAssetLoadMode assetLoadMode, string assetLoadStr, Action<T> callback = null, Action<Object> assetKeyModeCallBack = null)
            where T : Object
        {
            switch (assetLoadMode)
            {
                case EAssetLoadMode.Resources:
                    AsyncLoadByResources(assetLoadStr, callback);
                    break;
                case EAssetLoadMode.AssetBundle:
                    var key = ilsStringUtils.SplitAtLastSlash(assetLoadStr);
                    AsyncLoadByAssetBundle(key.Item1, key.Item2, callback);
                    break;
                case EAssetLoadMode.AssetKey:
                    AsyncLoadByAssetKey(assetLoadStr, assetKeyModeCallBack);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetLoadMode), assetLoadMode, null);
            }
        }

        /// <summary>
        ///     通用异步加载,使用<see cref="AssetReference{T}" />
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="callback"></param>
        /// <param name="assetKeyModeCallBack"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsync<T>(AssetReference<T> reference, Action<T> callback = null, Action<Object> assetKeyModeCallBack = null) where T : Object
        {
            if (reference.TryGetAssetLoadInfo(out var assetLoadMode, out var loadStr)) LoadAsync(assetLoadMode, loadStr, callback, assetKeyModeCallBack);
        }
    }
}