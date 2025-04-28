using System;
using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework.Core
{
    public class GameObjectPool : IPool<GameObject>
    {
        /// <summary>
        ///     对象池销毁时该做什么
        /// </summary>
        private readonly Action<GameObject> _actionOnDestroy;

        /// <summary>
        ///     从对象池中取出对象时该做什么
        /// </summary>
        private readonly Action<GameObject> _actionOnGet;

        /// <summary>
        ///     回收对象时该做什么
        /// </summary>
        private readonly Action<GameObject> _actionOnRecycle;

        /// <summary>
        ///     如何创建一个新的对象
        /// </summary>
        private readonly Func<GameObjectPool, GameObject> _createObjectFunc;


        //最大容量
        private readonly int _maxCapacity;

        //活跃的物体
        private readonly List<GameObject> activeObjects = new();

        //不活跃的物体
        private readonly Stack<GameObject> deactiveObjects = new();

        //将脚本接入对象池的生命周期控制
        private readonly Dictionary<GameObject, IPoolable> poolables = new();

        //集合检查使用
        private readonly HashSet<GameObject> poolRepeatCheckCollection = new();

        //初始容量
        private int _initialCapacity;


        public GameObjectPool(int maxCapacity, int initialCapacity, Func<GameObjectPool, GameObject> createObjectFunc, Action<GameObject> actionOnGet,
            Action<GameObject> actionOnRecycle, Action<GameObject> actionOnDestroy, string name, Transform gameObjectParent)
        {
            PoolViewer = new GameObject(name);
            _maxCapacity = maxCapacity;
            _initialCapacity = initialCapacity;
            _createObjectFunc = createObjectFunc;
            _actionOnGet = actionOnGet;
            _actionOnRecycle = actionOnRecycle;
            _actionOnDestroy = actionOnDestroy;
            this.Name = name;
            this.GameObjectParent = gameObjectParent;
            PoolViewer.transform.SetParent(gameObjectParent);
        }

        public Transform GameObjectParent { get; private set; }

        public GameObject PoolViewer { get; }

        public string Name { get; }

        /// <summary>
        ///     所有对象数目
        /// </summary>
        public int ObjectCount => GetObjectCount();

        /// <summary>
        ///     对象池的最大容量
        /// </summary>
        public int MaxCapacity => GetMaxCapacity();

        /// <summary>
        ///     活跃的对象数
        /// </summary>
        public int ActiveObjectCount => GetActiveObjectCount();

        /// <summary>
        ///     不活跃的对象数目
        /// </summary>
        public int DeactiveObjectCount => GetDeActiveObjectCount();


        public int GetObjectCount()
        {
            return activeObjects.Count + deactiveObjects.Count;
        }

        public int GetMaxCapacity()
        {
            return _maxCapacity;
        }

        public int GetActiveObjectCount()
        {
            return activeObjects.Count;
        }

        public int GetDeActiveObjectCount()
        {
            return deactiveObjects.Count;
        }

        public bool CollectionCheck { get; set; }

        public GameObject Get()
        {
            GameObject instance = null;
            if (!(deactiveObjects.Count > 0))
                instance = CreateObject();
            else
                instance = deactiveObjects.Pop();
            if (instance)
            {
#if UNITY_EDITOR
                poolRepeatCheckCollection.Remove(instance);
#endif

                activeObjects.Add(instance);
                _actionOnGet?.Invoke(instance);

                if (poolables.TryGetValue(instance, out var poolable)) poolable.OnGet();
            }

            return instance;
        }

        public void Recycle(GameObject obj)
        {
            if (!obj)
                return;
#if UNITY_EDITOR
            if (CollectionCheck)
            {
                if (poolRepeatCheckCollection.Contains(obj))
                {
                    Debug.LogError("检查代码，对象池出现重复回收同一实例");
                    return;
                }

                if (ObjectCount < MaxCapacity) poolRepeatCheckCollection.Add(obj);
            }
#endif
            //超出对象池的就丢掉了，嘻嘻
            if (ObjectCount >= MaxCapacity && !activeObjects.Contains(obj))
            {
                GameObject.Destroy(obj);
                return;
            }

            activeObjects.Remove(obj);
            deactiveObjects.Push(obj);
            _actionOnRecycle?.Invoke(obj);

            if (poolables.TryGetValue(obj, out var poolable)) poolable.OnRecycle();
        }

        public void Clear(Predicate<GameObject> match)
        {
            activeObjects.RemoveAll(match);

            var temp = new Stack<GameObject>();

            while (deactiveObjects.Count > 0)
                if (!match(deactiveObjects.Peek()))
                    temp.Push(deactiveObjects.Pop());

            deactiveObjects.Clear();
            while (temp.Count > 0) deactiveObjects.Push(temp.Pop());
#if UNITY_EDITOR
            poolRepeatCheckCollection.RemoveWhere(match);
#endif
        }

        public void Clear()
        {
            activeObjects.Clear();
            deactiveObjects.Clear();
#if UNITY_EDITOR
            poolRepeatCheckCollection.Clear();
#endif
        }

        public void OnDestroy()
        {
            foreach (var activeObject in activeObjects)
            {
                _actionOnDestroy?.Invoke(activeObject);
                if (poolables.TryGetValue(activeObject, out var poolable)) poolable.OnPoolDestroy();
                GameObject.Destroy(activeObject);
            }

            activeObjects.Clear();
            foreach (var deactiveObject in deactiveObjects)
            {
                _actionOnDestroy?.Invoke(deactiveObject);
                if (poolables.TryGetValue(deactiveObject, out var poolable)) poolable.OnPoolDestroy();
                GameObject.Destroy(deactiveObject);
            }

            deactiveObjects.Clear();

#if UNITY_EDITOR
            poolRepeatCheckCollection.Clear();
#endif
        }

        public List<GameObject> GetActiveObjects()
        {
            return activeObjects;
        }

        private GameObject CreateObject()
        {
            var obj = ObjectCount >= MaxCapacity ? null : _createObjectFunc?.Invoke(this);


            if (obj && obj.TryGetComponent<IPoolable>(out var poolable)) poolables.TryAdd(obj, poolable);
            return obj;
        }


        public void SetParent(Transform parent)
        {
            PoolViewer.transform.SetParent(parent);
            GameObjectParent = parent;
        }
    }
}