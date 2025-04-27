using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core
{
    public class FrameworkCore : Singleton<FrameworkCore>
    {
        private readonly List<(string, Action<GameObject>)> emptyGameObjectCallBacks;

        private readonly List<Exception> exceptions;
        private readonly Dictionary<Type, IManager> innerManagers;
        private readonly Dictionary<Type, GameObject> managerContainerObjects;

        private readonly LinkedList<IManager> managerList;

        private FrameworkConfig frameworkConfig;

        public Transform frameworkGOBaseTransform;

        private double LogicUpdateCounter;
        private GameObject otherGameObject;

        public FrameworkCore()
        {
            innerManagers = new Dictionary<Type, IManager>();
            managerContainerObjects = new Dictionary<Type, GameObject>();
            managerList = new LinkedList<IManager>();
            emptyGameObjectCallBacks = new List<(string, Action<GameObject>)>();
            LogicUpdateCounter = 0;
            LogicUpdateCountSinceInit = 0;
            exceptions = new List<Exception>();
        }

        public int LogicUpdateCountSinceInit { get; private set; }

        public void Initialize()
        {
            exceptions.Clear();
            frameworkConfig = Config.GetFrameworkConfig();

            AssemblyForeach();

            var other = new GameObject("Other");
            other.transform.parent = frameworkGOBaseTransform;
            otherGameObject = other;

            foreach (var emptyGameObjectCallBack in emptyGameObjectCallBacks)
            {
                var result = new GameObject(emptyGameObjectCallBack.Item1);
                result.transform.parent = otherGameObject.transform;

                emptyGameObjectCallBack.Item2?.Invoke(result);
            }

            ThrowAllExceptions();
        }

        /// <summary>
        ///     程序集遍历
        /// </summary>
        private void AssemblyForeach()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var list = new List<IAssemblyForeach>();
            var allManagers = new List<(IManager, int)>();

            //找出对应需要遍历程序集
            foreach (var type in types)
                if (typeof(IManager).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
                {
                    var manager = Activator.CreateInstance(type);

                    if (manager == null)
                    {
                        Debug.LogError("管理类创建失败：" + type.FullName);
                        continue;
                    }

                    var Im = manager as IManager;
                    var Ia = manager as IAssemblyForeach;
                    if (Im != null)
                    {
                        allManagers.Add((Im, frameworkConfig.GetManagerUpdateIndex(type)));

                        //将Manager作为一个子物体存在FrameworkCore下？这样利于调试
                        var managerContianer = new GameObject(manager.GetType().Name);
                        managerContianer.AddComponent<ManagerContainer>().Manager = Im;
                        managerContianer.transform.parent = frameworkGOBaseTransform;

                        //加入字典
                        innerManagers.TryAdd(type, Im);
                        managerContainerObjects.TryAdd(type, managerContianer);
                    }

                    if (Ia != null) list.Add(Ia);
                }

            //排序Manger，并初始化
            allManagers.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            foreach (var manager in allManagers)
            {
                managerList.AddLast(manager.Item1);
                try
                {
                    manager.Item1.Init();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            //找出需要遍历的类型
            foreach (var iAssemblyForeach in list)
                try
                {
                    iAssemblyForeach.ForeachCurrentAssembly(types);
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
        }

        public void Update()
        {
            //优先查看逻辑帧更新
            var logicUpdateInterval = 1 / (double)frameworkConfig.LogicUpdateCountPerScecond;
            LogicUpdateCounter += Time.unscaledDeltaTime;
            exceptions.Clear();
            while (LogicUpdateCounter > logicUpdateInterval)
            {
                LogicUpdateCounter -= logicUpdateInterval;
                foreach (var manager in managerList)
                    try
                    {
                        manager.LogicUpdate();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
            }

            foreach (var manager in managerList)
                try
                {
                    manager.Update();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

            ThrowAllExceptions();
        }

        public void LateUpdate()
        {
            exceptions.Clear();
            foreach (var manager in managerList)
                try
                {
                    manager.LateUpdate();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

            ThrowAllExceptions();
        }

        public void FixedUpdate()
        {
            exceptions.Clear();
            foreach (var manager in managerList)
                try
                {
                    manager.FixedUpdate();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

            ThrowAllExceptions();
        }

        public void OnDestroy()
        {
            exceptions.Clear();
            //删除
            foreach (var manager in managerList)
                try
                {
                    manager.OnDestroy();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

            innerManagers.Clear();
            managerList.Clear();
            ThrowAllExceptions();
        }

        public void OnDrawGizmos()
        {
            exceptions.Clear();
            foreach (var manager in managerList)
                try
                {
                    manager.OnDrawGizmos();
                    if (Selection.activeGameObject == managerContainerObjects[manager.GetType()]) manager.OnDrawGizmosSelected();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

            ThrowAllExceptions();
        }

        public void OnDrawGizmosSelected()
        {
            exceptions.Clear();
            foreach (var manager in managerList)
                try
                {
                    manager.OnDrawGizmos();
                    manager.OnDrawGizmosSelected();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }

            ThrowAllExceptions();
        }

        private void ThrowAllExceptions()
        {
            if (exceptions.Any()) throw new AggregateException(exceptions);
        }

        #region Manager

        //获取对应的Manager实例
        public T GetManager<T>() where T : class, IManager
        {
            if (innerManagers.TryGetValue(typeof(T), out var manager)) return manager as T;
            if (!typeof(T).IsAbstract) Debug.LogError($"类{typeof(T)}是抽象类，无法实例化");
            if (!typeof(T).IsAssignableFrom(typeof(IManager))) Debug.LogError($"类{typeof(T)}未继承管理器接口:{typeof(IManager)}");
            return null;
        }

        public GameObject GetManagerContainerGameObject<T>() where T : class, IManager
        {
            return managerContainerObjects.GetValueOrDefault(typeof(T));
        }

        public void CreateEmptyGameObject(string name, Action<GameObject> createCallBack)
        {
            if (otherGameObject is not null)
            {
                var result = new GameObject(name);
                result.transform.parent = otherGameObject.transform;

                createCallBack?.Invoke(result);
            }
            else
            {
                emptyGameObjectCallBacks.Add((name, createCallBack));
            }
        }

        public static T Get_Manager<T>() where T : class, IManager
        {
            return Instance.GetManager<T>();
        }

        public static void Create_EmptyGameObject(string name, Action<GameObject> createCallBack)
        {
            Instance.CreateEmptyGameObject(name, createCallBack);
        }

        #endregion
    }
}