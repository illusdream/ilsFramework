using UnityEngine;

namespace ilsFramework
{
    /// <summary>
    /// 基于MonoBehaviour的单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ScriptSingleton<T> : MonoBehaviour where T : ScriptSingleton<T>
    {
        private static T _instance;
        private static readonly object locker = new object();
        public static T Instance
        {
            get
            {
                //创建单例
                if (_instance == null)
                {
                    lock (locker)
                    {
                        //先在场景里查找单例
                        _instance = FindObjectOfType<T>();
                        //有脚本但是数目不对
                        if (FindObjectsOfType<T>().Length > 1)
                        {
                            Debug.LogError("场景中的单例脚本数量 > 1:" + _instance.GetType());
                            return _instance;
                        }
    
                        //找不到
                        if (_instance == null)
                        {
                            string instanceName = typeof(T).Name;
                            //查到对应名称的空物体
                            GameObject instance = GameObject.Find(instanceName);
    
                            if (instance is null)
                            {
                                //挂载
                                instance = new GameObject(instanceName);
                                DontDestroyOnLoad(instance);
                                _instance = instance.AddComponent<T>();
                                DontDestroyOnLoad(_instance);
                            }
                            else
                            {
                                //场景中已存在同名游戏物体时就打印提示
                                Debug.LogError("场景中已存在单例脚本所挂载的游戏物体:" + instance.name);
                            }
                        }
    
                    }
                }
                return _instance;
            }
        }
    
        private void OnDestroy()
        {
            _instance = null;
        }
    
    }
}


