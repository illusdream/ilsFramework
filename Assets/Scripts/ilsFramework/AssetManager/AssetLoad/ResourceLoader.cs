using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ilsFramework
{
    public class ResourceLoader
    {
        private Dictionary<string, Object> _resources;
        
        
        public void Init()
        {
            _resources = new Dictionary<string, Object>();
        }

        public void OnDestroy()
        {
            
        }
        
        public T Load<T>(string assetKey) where T : Object
        {
            if (_resources.TryGetValue(assetKey,out var resource))
            {
                return (T)resource;
            }
            else
                return Resources.Load<T>(assetKey);
        }

        public Object Load(string assetKey)
        {
            return Resources.Load(assetKey);
        }
        
        public void LoadAsync<T>(string assetKey, Action<T> callBack) where T : UnityEngine.Object
        {
            if (_resources.TryGetValue(assetKey, out var resource))
            {
                callBack?.Invoke(resource as T);
            }
            else
                FrameworkCore.Get_Manager<MonoManager>().StartCoroutine(CurrentLoadAsync<T>(assetKey, callBack));
        }
        IEnumerator CurrentLoadAsync<T>(string name, Action<T> callback) where T : UnityEngine.Object
        {
            ResourceRequest request = Resources.LoadAsync<T>(name);
            yield return request;

            if (request.asset is GameObject)
            {
                T t = GameObject.Instantiate(request.asset) as T;
                if (t != null)
                {               
                    t.name = name;
                    callback(t);
                }

            }
            else
            {
                request.asset.name = name;
                callback(request.asset as T);
            }

        }

        public void Unload(string assetKey)
        {
            if (_resources.TryGetValue(assetKey, out var resource))
            {
                Resources.UnloadAsset(resource);
                _resources.Remove(assetKey);
            }
        }
        
    }
}