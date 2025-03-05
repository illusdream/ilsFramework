using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace ilsFramework
{
    [Serializable]
    public class AssetBundleLoader
    {
        
        //主包
        public static string mainABName
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                return "StandaloneWindows";
#elif UNITY_IPHONE
                return "IOS";
#elif UNITY_ANDROID
                return "Android";
#endif
            }
        }
        
        private AssetBundle _mainAB = null;
        [ShowInInspector]
        private AssetBundleManifest _mainManifest;
        private bool _mainManifestLoaded;
        
        //资源缓存与引用
        [ShowInInspector]
        private Dictionary<string, AssetBundle> _loadedAssetBundles = new Dictionary<string, AssetBundle>();
        [ShowInInspector]
        private Dictionary<string,int> _referenceCounts = new Dictionary<string, int>();

        
        
        public void Init()
        {
            LoadMainfest();
        }

        private void LoadMainfest()
        {
            _mainAB= AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + mainABName);
            _mainManifest = _mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            _mainManifestLoaded = true;
            
            _loadedAssetBundles.Add("MainAssetBundle", _mainAB);
        }

        //分成两部分

        #region AssetBundle 包相关

        public AssetBundle LoadAssetBundle(string bundleName)
        {
            AssetBundle result = null;
            if (!_loadedAssetBundles.TryGetValue(bundleName, out result))
            {
                 result = AssetBundle.LoadFromFile(Application.streamingAssetsPath+"/"+bundleName);
                 _loadedAssetBundles.Add(bundleName, result);
                 _referenceCounts.Add(bundleName, 0);
            }

            foreach (var dependency in _mainManifest.GetAllDependencies(bundleName))
            {
                if (!_loadedAssetBundles.ContainsKey(dependency))
                {
                    LoadAssetBundle(dependency);
                }
                _referenceCounts[dependency]++;
            }
            return result;
        }

        public void UnloadAssetBundle(string bundleName, bool unloadAllLoadedObjects)
        {
            if (_loadedAssetBundles.TryGetValue(bundleName, out var assetBundle))
            {
                assetBundle.Unload(unloadAllLoadedObjects);
                
                foreach (var dependency in _mainManifest.GetAllDependencies(bundleName))
                {
                    _referenceCounts[dependency]--;
                    if (_referenceCounts[dependency] == 0)
                    {
                        UnloadAssetBundle(bundleName, unloadAllLoadedObjects);
                        _referenceCounts.Remove(dependency);
                    }
                }
                
            }
        }

        public void UnLoadUnUsedAssetBundle(bool unloadAllLoadedObjects)
        {
            var needRemove = _referenceCounts.Where((kvp)=>kvp.Value==0).Select((kvp)=>kvp.Key);

            foreach (var key in needRemove)
            {
                UnloadAssetBundle(key,unloadAllLoadedObjects);
            }
            
        }

        public void LoadAssetBundleAsync(string bundleName, Action callback = null)
        {
            MonoManager.Instance.StartCoroutine(IE_LoadAssetBundleAsync(bundleName, callback));
        }

        private IEnumerator IE_LoadAssetBundleAsync(string bundleName, Action callback)
        {
            if (!_loadedAssetBundles.ContainsKey(bundleName))
            {
                foreach (var dependency in _mainManifest.GetAllDependencies(bundleName))
                {

                    if (!_loadedAssetBundles.ContainsKey(bundleName))
                    {
                        yield return IE_LoadAssetBundleAsync(dependency, null);
                    }
                    _referenceCounts[dependency]++;
                    
                }
                
                var target_abcr = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath+"/"+bundleName);
                yield return target_abcr;
                _loadedAssetBundles.Add(bundleName, target_abcr.assetBundle);
                _referenceCounts.Add(bundleName, 0);
            }
            callback?.Invoke();
        }

        public void UnloadAssetBundleAsync(string bundleName, bool unloadAllLoadedObjects, Action callback = null)
        {
            MonoManager.Instance.StartCoroutine(IE_UnLoadAssetBundleAsync(bundleName, unloadAllLoadedObjects, callback));
        }

        private IEnumerator IE_UnLoadAssetBundleAsync(string bundleName, bool unloadAllLoadedObjects, Action callback)
        {
            if (_loadedAssetBundles.TryGetValue(bundleName, out var assetBundle))
            {
               yield return assetBundle.UnloadAsync(unloadAllLoadedObjects);
               callback?.Invoke();
            }
        }
        
        #endregion

        #region Asset 资源相关

        public Object LoadAsset(string assetBundleName, string assetName)
        {
            AssetBundle ab = LoadAssetBundle(assetBundleName);
            if (ab == null)
            {
                $"对应的AssetBundle:{assetBundleName}不存在".ErrorSelf();
                return null;
            }
            return ab.LoadAsset(assetName);
        }

        public T LoadAsset<T>(string assetBundleName, string assetName) where T : Object
        {
            AssetBundle ab = LoadAssetBundle(assetBundleName);
            if (ab == null)
            {
                $"对应的AssetBundle:{assetBundleName}不存在".ErrorSelf();
                return null;
            }
            return ab.LoadAsset<T>(assetName);
        }

        public Object LoadAsset(string assetBundleName, string assetName, Type type)
        {
            AssetBundle ab = LoadAssetBundle(assetBundleName);
            if (ab == null)
            {
                $"对应的AssetBundle:{assetBundleName}不存在".ErrorSelf();
                return null;
            }
            return ab.LoadAsset(assetName, type);
        }

        public void LoadAssetAsync(string assetBundleName, string assetName, Action<Object> callback)
        {
            LoadAssetBundleAsync(assetBundleName, () =>
            {
                AssetBundle ab = LoadAssetBundle(assetBundleName);
                MonoManager.Instance.StartCoroutine(IE_LoadAssetAsync(ab, assetName, callback));
            });
        }

        private IEnumerator IE_LoadAssetAsync(AssetBundle assetBundle,string assetName, Action<Object> callback)
        {
            var abr = assetBundle.LoadAssetAsync(assetName);
            yield return abr;
            callback?.Invoke(abr.asset);
        }
        
        public void LoadAssetAsync<T>(string assetBundleName, string assetName,Action<T> callback) where T : Object
        {
            LoadAssetBundleAsync(assetBundleName, () =>
            {
                AssetBundle ab = LoadAssetBundle(assetBundleName);
                MonoManager.Instance.StartCoroutine(IE_LoadAssetAsync(ab, assetName, callback));
            });
        }

        private IEnumerator IE_LoadAssetAsync<T>(AssetBundle assetBundle,string assetName, Action<T> callback) where T : Object
        {
            var abr = assetBundle.LoadAssetAsync<T>(assetName);
            yield return abr;
            callback?.Invoke((T)abr.asset);
        }

        public void LoadAssetAsync(string assetBundleName, string assetName, Type type, Action<Object> callback)
        {
            LoadAssetBundleAsync(assetBundleName, () =>
            {
                AssetBundle ab = LoadAssetBundle(assetBundleName);
                MonoManager.Instance.StartCoroutine(IE_LoadAssetAsync(ab, assetName, type,callback));
            });
        }

        private IEnumerator IE_LoadAssetAsync(AssetBundle assetBundle, string assetName, Type type, Action<Object> callback)
        {
            var abr = assetBundle.LoadAssetAsync(assetName, type);
            yield return abr;
            if (abr.asset)
            {
                callback?.Invoke(abr.asset);
            }
        }
        
        #endregion
    }
}