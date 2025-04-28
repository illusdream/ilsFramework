using System;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     初始化对象池的工厂
    /// </summary>
    public class GameObjectPoolFactory
    {
        /// <summary>
        ///     对象池销毁时该做什么
        /// </summary>
        private Action<GameObject> _actionOnDestroy;

        /// <summary>
        ///     从对象池中取出对象时该做什么
        /// </summary>
        private Action<GameObject> _actionOnGet;

        /// <summary>
        ///     回收对象时该做什么
        /// </summary>
        private Action<GameObject> _actionOnRecycle;


        /// <summary>
        ///     如何创建一个新的对象
        /// </summary>
        private Func<GameObjectPool, GameObject> _createObjectFunc;

        //初始容量
        private int _initialCapacity;

        //最大容量
        private int _maxCapacity;
        
        private Transform gameObjectParent;

        private string name;

        private GameObjectPoolFactory()
        {
            gameObjectParent = PoolManager.ContainerObject.transform;
            name = PoolManager.Instance.GetDefaultGameObjectPoolName();
            _initialCapacity = 8;
            _maxCapacity = 32;
            _createObjectFunc = gPool =>
            {
                var go = new GameObject();
                go.transform.SetParent(gPool.PoolViewer.transform);
                go.SetActive(false);
                return go;
            };
            _actionOnGet = gameObject => { gameObject.SetActive(true); };
            _actionOnRecycle = gameObject => { gameObject.SetActive(false); };
        }

        public static GameObjectPoolFactory Create()
        {
            return new GameObjectPoolFactory();
        }

        public GameObjectPoolFactory SetName(string name)
        {
            this.name = name;
            return this;
        }

        public GameObjectPoolFactory SetGameObjectParent(Transform gameObjectParent)
        {
            this.gameObjectParent = gameObjectParent;
            return this;
        }

        public GameObjectPoolFactory SetMaxCapacity(int maxCapacity)
        {
            _maxCapacity = maxCapacity;
            return this;
        }

        public GameObjectPoolFactory SetInitialCapacity(int initialCapacity)
        {
            _initialCapacity = initialCapacity;
            return this;
        }

        public GameObjectPoolFactory SetCreateObjectFunc(Func<GameObjectPool, GameObject> createObjectFunc)
        {
            _createObjectFunc = createObjectFunc;
            return this;
        }

        public GameObjectPoolFactory SetActionOnGet(Action<GameObject> actionOnGet)
        {
            _actionOnGet = actionOnGet;
            return this;
        }

        public GameObjectPoolFactory SetActionOnRecycle(Action<GameObject> actionOnRecycle)
        {
            _actionOnRecycle = actionOnRecycle;
            return this;
        }

        public GameObjectPoolFactory SetActionOnDestroy(Action<GameObject> actionOnDestroy)
        {
            _actionOnDestroy = actionOnDestroy;
            return this;
        }

        public GameObjectPool Register()
        {
            var instance = new GameObjectPool(_maxCapacity, _initialCapacity, _createObjectFunc, _actionOnGet, _actionOnRecycle, _actionOnDestroy, name, gameObjectParent);
            PoolManager.Instance.RegisterGameObjectPool(instance);
            return instance;
        }
    }
}