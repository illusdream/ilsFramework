using System;
using System.Collections;
using UnityEngine;

namespace ilsFramework
{
    public class ResourceManager : ManagerSingleton<ResourceManager>, IManager
    {
        public void Init()
        {

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

        public T Load<T>(string objName) where T : UnityEngine.Object
        {
            T res = Resources.Load<T>(objName);

            if (res is GameObject)
            {

            }
            //�����õ���ʱ������
            switch (res)
            {
                case GameObject:
                {

                    return res;
                }
                    break;
                default:
                    break;
            }

            return res;
        }

        public void LoadAsync<T>(string name, Action<T> callBack) where T : UnityEngine.Object
        {
            FrameworkCore.Get_Manager<MonoManager>().StartCoroutine(CurrentLoadAsync<T>(name, callBack));
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



    }
}
