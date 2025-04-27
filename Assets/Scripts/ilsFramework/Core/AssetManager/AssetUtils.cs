using System;
using System.Text.RegularExpressions;
using UnityEditor;
using Object = UnityEngine.Object;

namespace ilsFramework.Core
{
    public static class Asset
    {
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
        public static T Load<T>(EAssetLoadMode assetLoadMode, string assetLoadStr) where T : Object
        {
            return AssetManager.Instance.Load<T>(assetLoadMode, assetLoadStr);
        }

        /// <summary>
        ///     通用同步加载，使用<see cref="AssetReference{T}" />
        /// </summary>
        /// <param name="assetReference"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Load<T>(AssetReference<T> assetReference) where T : Object
        {
            return AssetManager.Instance.Load(assetReference);
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
        public static void LoadAsync<T>(EAssetLoadMode assetLoadMode, string assetLoadStr, Action<T> callback, Action<Object> assetKeyModeCallBack)
            where T : Object
        {
            AssetManager.Instance.LoadAsync(assetLoadMode, assetLoadStr, callback, assetKeyModeCallBack);
        }

        /// <summary>
        ///     通用异步加载,使用<see cref="AssetReference{T}" />
        /// </summary>
        /// <param name="assetReference"></param>
        /// <param name="callback"></param>
        /// <param name="assetKeyModeCallBack"></param>
        /// <typeparam name="T"></typeparam>
        public static void LoadAsync<T>(AssetReference<T> assetReference, Action<T> callback, Action<Object> assetKeyModeCallBack) where T : Object
        {
            AssetManager.Instance.LoadAsync(assetReference, callback, assetKeyModeCallBack);
        }

        /// <summary>
        ///     使用同步加载位于Resources文件夹的资源
        /// </summary>
        /// <param name="path">相对于Resources文件夹的相对路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public static T LoadByResources<T>(string loadAssetStr) where T : Object
        {
            return AssetManager.Instance.LoadByResources<T>(loadAssetStr);
        }

        /// <summary>
        ///     使用异步加载位于Resources文件夹的资源
        /// </summary>
        /// <param name="path">相对于Resources文件夹的相对路径</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源类型</typeparam>
        public static void AsyncLoadByResources<T>(string loadAssetStr, Action<T> callback) where T : Object
        {
            AssetManager.Instance.AsyncLoadByResources(loadAssetStr, callback);
        }

        /// <summary>
        ///     使用同步加载位于AssetBundle内的资源
        /// </summary>
        /// <param name="assetBundleName">AssetBundle包名</param>
        /// <param name="assetName">资源名</param>
        /// <typeparam name="T">资源的类型</typeparam>
        /// <returns></returns>
        public static T LoadByAssetBundle<T>(string assetBundleName, string assetName) where T : Object
        {
            return AssetManager.Instance.LoadByAssetBundle<T>(assetBundleName, assetName);
        }

        /// <summary>
        ///     使用异步加载位于AssetBundle内的资源
        /// </summary>
        /// <param name="assetBundleName">AssetBundle包名</param>
        /// <param name="assetName">资源名</param>
        /// <param name="callback">回调</param>
        /// <typeparam name="T">资源的类型</typeparam>
        public static void AsyncLoadByAssetBundle<T>(string assetBundleName, string assetName, Action<T> callback) where T : Object
        {
            AssetManager.Instance.AsyncLoadByAssetBundle(assetBundleName, assetName, callback);
        }

        /// <summary>
        ///     AssetKey同步加载
        /// </summary>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static Object LoadByAssetKey(string assetKey)
        {
            return AssetManager.Instance.LoadByAssetKey(assetKey);
        }

        /// <summary>
        ///     AssetKey异步加载
        /// </summary>
        /// <param name="assetKey"></param>
        /// <param name="callback"></param>
        public static void AsyncLoadByAssetKey(string assetKey, Action<Object> callback)
        {
            AssetManager.Instance.AsyncLoadByAssetKey(assetKey, callback);
        }
#if UNITY_EDITOR
        public static bool CheckAssetInResourcesFolder(string assetPath)
        {
            var resourceParten = "^Assets/Resources/";
            var importer = AssetImporter.GetAtPath(assetPath);
            var inReourcesFolder = Regex.IsMatch(importer.assetPath, resourceParten);
            return inReourcesFolder;
        }
#endif
    }
}