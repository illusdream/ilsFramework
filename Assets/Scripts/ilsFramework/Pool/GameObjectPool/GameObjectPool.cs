using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ilsFramework
{
    public class GameObjectPool : IPool<GameObject>
    {
        
        
        private Transform gameObjectParent;
        private GameObject _poolViewer;
        private string name;
        
        public Transform GameObjectParent => gameObjectParent;
        public GameObject PoolViewer => _poolViewer;
        public string Name => name;
        
        
        //最大容量
        private int _maxCapacity;
        //初始容量
        private int _initialCapacity;
        
        //活跃的物体
        private List<GameObject> activeObjects = new List<GameObject>();
        //不活跃的物体
        private Stack<GameObject> deactiveObjects = new Stack<GameObject>();
        //集合检查使用
        private HashSet<GameObject> poolRepeatCheckCollection = new HashSet<GameObject>();
        //将脚本接入对象池的生命周期控制
        private Dictionary<GameObject,IPoolable> poolables = new Dictionary<GameObject, IPoolable>();
        public GameObjectPool(int maxCapacity , int initialCapacity, Func<GameObjectPool,GameObject> createObjectFunc , Action<GameObject> actionOnGet, Action<GameObject> actionOnRecycle , Action<GameObject> actionOnDestroy , bool collectionCheck, string name, Transform gameObjectParent)
        {
            _poolViewer = new GameObject(name);
            _maxCapacity = maxCapacity;
            _initialCapacity = initialCapacity;
            _createObjectFunc = createObjectFunc;
            _actionOnGet = actionOnGet;
            _actionOnRecycle = actionOnRecycle;
            _actionOnDestroy = actionOnDestroy;
            CollectionCheck = collectionCheck;
            this.name = name;
            this.gameObjectParent = gameObjectParent;
            _poolViewer.transform.SetParent(gameObjectParent);
        }

        /// <summary>
        /// 所有对象数目
        /// </summary>
        public int ObjectCount => GetObjectCount();
        
        /// <summary>
        /// 对象池的最大容量
        /// </summary>
        public int MaxCapacity => GetMaxCapacity();
        
        /// <summary>
        /// 活跃的对象数
        /// </summary>
        public int ActiveObjectCount => GetActiveObjectCount();
        
        /// <summary>
        /// 不活跃的对象数目
        /// </summary>
        public int DeactiveObjectCount => GetDeActiveObjectCount();
        
        /// <summary>
        /// 如何创建一个新的对象
        /// </summary>
        private Func<GameObjectPool,GameObject> _createObjectFunc;
        
        /// <summary>
        /// 从对象池中取出对象时该做什么
        /// </summary>
        private Action<GameObject> _actionOnGet;
        
        /// <summary>
        /// 回收对象时该做什么
        /// </summary>
        private Action<GameObject> _actionOnRecycle;
        
        /// <summary>
        /// 对象池销毁时该做什么
        /// </summary>
        private Action<GameObject> _actionOnDestroy;
        
        public List<GameObject> GetActiveObjects() => activeObjects;
        
        
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
        
        private GameObject CreateObject()
        {
            var obj = ObjectCount >= MaxCapacity ? null : _createObjectFunc?.Invoke(this);
            
            
            if (obj && obj.TryGetComponent<IPoolable>(out var poolable))
            {
                poolables.TryAdd(obj, poolable);
            }
            return obj;
        }
        public GameObject Get()
        {
            GameObject instance=null;
            if (!(deactiveObjects.Count > 0))
            {
                instance =  CreateObject();
            }
            else
            {
                instance = deactiveObjects.Pop();
            }
            if (instance)
            {
#if UNITY_EDITOR
                poolRepeatCheckCollection.Remove(instance);
#endif

                activeObjects.Add(instance);    
                _actionOnGet?.Invoke(instance);
                
                if (poolables.TryGetValue(instance, out var poolable))
                {
                    poolable.OnGet();   
                }
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
                if ( poolRepeatCheckCollection.Contains(obj))
                {
                    Debug.LogError("检查代码，对象池出现重复回收同一实例");
                    return;
                }
                else if (ObjectCount < MaxCapacity)
                {
                    poolRepeatCheckCollection.Add(obj);
                }
                else
                {
                    
                }
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
            
            if (poolables.TryGetValue(obj, out var poolable))
            {
                poolable.OnRecycle();   
            }
        }

        public void Clear(Predicate<GameObject> match)
        {
            activeObjects.RemoveAll(match);
            
            Stack<GameObject> temp = new Stack<GameObject>();

            while (deactiveObjects.Count>0)
            {
                if (!match(deactiveObjects.Peek()))
                {
                    temp.Push(deactiveObjects.Pop());
                }
            }
            deactiveObjects.Clear();    
            while (temp.Count > 0)
            {
                deactiveObjects.Push(temp.Pop());
            }
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
                if (poolables.TryGetValue(activeObject, out var poolable))
                {
                    poolable.OnPoolDestroy();   
                }
                GameObject.Destroy(activeObject);
            }
            activeObjects.Clear();
            foreach (var deactiveObject in deactiveObjects)
            {
                _actionOnDestroy?.Invoke(deactiveObject);
                if (poolables.TryGetValue(deactiveObject, out var poolable))
                {
                    poolable.OnPoolDestroy();   
                }
                GameObject.Destroy(deactiveObject);
            }
            deactiveObjects.Clear();

#if UNITY_EDITOR
            poolRepeatCheckCollection.Clear();  
#endif
        }


        public void SetParent(Transform parent)
        {
            _poolViewer.transform.SetParent(parent);
            gameObjectParent = parent;
        }
    }
}