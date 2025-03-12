using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace ilsFramework
{
    /// <summary>
    /// 所有管理类应该从这个地方访问
    /// </summary>
    public class FrameworkCore : ScriptSingleton<FrameworkCore>
    {
        private Dictionary<Type, IManager> innerManagers;
        private Dictionary<Type, GameObject> managerContainerObjects;
         [ShowInInspector]
        private LinkedList<IManager> managerList;

        private List<(string, Action<GameObject>)> emptyGameObjectCallBacks;
        private GameObject otherGameObject;
        
        private FrameworkConfig frameworkConfig;
        
        public FrameworkCore()
        {
            innerManagers = new Dictionary<Type, IManager>();
            managerContainerObjects = new Dictionary<Type, GameObject>();
            managerList = new LinkedList<IManager>();
            emptyGameObjectCallBacks = new List<(string, Action<GameObject>)>();
        }


        [RuntimeInitializeOnLoadMethod]
        private static void InitFramework()
        {
            _ = FrameworkCore.Instance;
        }
        
        private void Awake()
        {
            frameworkConfig = Config.GetFrameworkConfig();
            AssemblyForeach();
            GameObject other = new GameObject("Other");
            other.transform.parent = transform;
            otherGameObject = other;

            foreach (var emptyGameObjectCallBack in emptyGameObjectCallBacks)
            {
                GameObject result = new GameObject(emptyGameObjectCallBack.Item1);
                result.transform.parent = otherGameObject.transform;

                emptyGameObjectCallBack.Item2?.Invoke(result);
            }
        }

        private void Start()
        {


        }

        private void Update()
        {

            foreach (var manager in managerList)
            {
                manager.Update();
            }
        }

        private void LateUpdate()
        {
            foreach (var manager in managerList)
            {
                manager.LateUpdate();
            }
        }

        private void FixedUpdate()
        {
            foreach (var manager in managerList)
            {
                manager.FixedUpdate();
            }
        }

        private void OnDestroy()
        {
            //删除
            foreach (var manager in managerList)
            {
                manager.OnDestroy();
            }
            innerManagers.Clear();
            managerList.Clear();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var manager in managerList)
            {
                manager.OnDrawGizmos();
                if (Selection.activeGameObject == managerContainerObjects[manager.GetType()])
                {
                    manager.OnDrawGizmosSelected();
                }
            }
        }
#endif

        

        //遍历程序集
        void AssemblyForeach()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            List<IAssemblyForeach> list = new List<IAssemblyForeach>();
            List<(IManager,int)> allManagers = new List<(IManager,int)>();
            //找出对应需要遍历程序集
            foreach (var type in types)
            {
                if (typeof(IManager).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    object manager = Activator.CreateInstance(type);

                    if (manager == null)
                    {
                        Debug.LogError("管理类创建失败：" + type.FullName);
                        continue;
                    }

                    IManager Im = manager as IManager;
                    IManagerSingleton IMS = manager as IManagerSingleton;
                    IAssemblyForeach Ia = manager as IAssemblyForeach;
                    if (Im != null)
                    {
                        allManagers.Add((Im,frameworkConfig.GetManagerUpdateIndex(type)));

                        //将Manager作为一个子物体存在FrameworkCore下？这样利于调试
                        GameObject managerContianer = new GameObject(manager.GetType().Name);
                        managerContianer.AddComponent<ManagerContainer>().Manager = Im;
                        managerContianer.transform.parent = transform;

                        //加入字典
                        innerManagers.TryAdd(type, Im);
                        managerContainerObjects.TryAdd(type, managerContianer);
                        
                    }

                    if (Ia != null)
                    {
                        list.Add(Ia);
                    }
                }
            }
            //排序Manger，并初始化
            allManagers.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            foreach (var manager in allManagers)
            {
                managerList.AddLast(manager.Item1);
                manager.Item1.Init();
            }
            
            //找出需要遍历的类型
            foreach (var iAssemblyForeach in list)
            {
                iAssemblyForeach.ForeachCurrentAssembly(types);
            }

        }

        //获取对应的Manager实例
        public T GetManager<T>() where T : class, IManager
        {
            if (innerManagers.TryGetValue(typeof(T), out var manager))
            {
                return manager as T;
            }
            else
            {
                return CreateManager<T>();
            }
        }
        //创建Manager实例
        private T CreateManager<T>() where T : class, IManager
        {
            IManager manager = (IManager)Activator.CreateInstance(typeof(T));

            if (manager == null)
            {
                Debug.LogError("管理类创建失败：" + typeof(T).FullName);
                return null;
            }

            //加入链表
            LinkedListNode<IManager> current = managerList.First;

            managerList.AddLast(manager);

            //将Manager作为一个子物体存在FrameworkCore下？这样利于调试
            GameObject managerContianer = new GameObject(manager.GetType().Name);
            managerContianer.AddComponent<ManagerContainer>().Manager = manager;
            managerContianer.transform.parent = transform;

            //加入字典
            innerManagers.TryAdd(typeof(T), manager);
            managerContainerObjects.Add(typeof(T), managerContianer);



            manager.Init();


            return (T)manager;
        }

        /// <summary>
        /// 创建一个空gameObject,该GameObject挂载在FrameworkCore下的Other
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public void CreateEmptyGameObject(string name, Action<GameObject> createCallBack)
        {
            if (otherGameObject is not null)
            {
                GameObject result = new GameObject(name);
                result.transform.parent = otherGameObject.transform;

                createCallBack?.Invoke(result);
            }
            else
            {
                emptyGameObjectCallBacks.Add((name, createCallBack));
            }
        }

        public GameObject GetManagerContainerGameObject<T>() where T : class, IManager
        {
            if (!innerManagers.ContainsKey(typeof(T)))
            {
                CreateManager<T>();
            }
            return managerContainerObjects[typeof(T)];
        }

        #region 快速静态方法
        public static T Get_Manager<T>() where T : class, IManager => Instance.GetManager<T>();
        public static void Create_EmptyGameObject(string name, Action<GameObject> createCallBack) => Instance.CreateEmptyGameObject(name, createCallBack);
        #endregion
    }
}
