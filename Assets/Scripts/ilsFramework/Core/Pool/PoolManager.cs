using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ilsFramework.Core
{
    public class PoolManager : ManagerSingleton<PoolManager>
    {
        EPoolCollectionCheck _poolCollectionCheck;
        
        PoolConfig _poolConfig;
        public override void OnInit()
        {
            _poolConfig = Config.GetConfig<PoolConfig>();
            _poolCollectionCheck = _poolConfig.PoolCollectionCheck;
        }
        public override void OnUpdate()
        {
            
        }
        public override void OnLateUpdate()
        {
            
        }
        public override void OnLogicUpdate()
        {
            
        }

        public override void OnFixedUpdate()
        {
            
        }
        public override void OnDestroy()
        {
            foreach (var gameObjectPool in gameObjectPools) gameObjectPool.Value.OnDestroy();
            gameObjectPools.Clear();

            foreach (var referencePool in referencePools)
            {
                referencePool.Value.OnDestroy();
            }
            referencePools.Clear();
        }

        public override void OnDrawGizmos()
        {
            
        }

        public override void OnDrawGizmosSelected()
        {
            
        }

        public bool UseCollectionCheck()
        {
            switch (_poolCollectionCheck)
            {
                case EPoolCollectionCheck.None:
                    return false;
                    break;
                case EPoolCollectionCheck.OnlyEditor:
                    return Application.isEditor;
                return false;
                    break;
                case EPoolCollectionCheck.OnlyRunTime:
                    return !Application.isEditor;
                    break;
                case EPoolCollectionCheck.BothEditorAndRunTime:
                    return true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #region ReferencePool

        private Dictionary<Type,ReferencePool> referencePools = new Dictionary<Type, ReferencePool>();

        public ReferencePool<T> CreateReferencePool<T>() where T : class, IPoolable
        {
            Type genericType = typeof(ReferencePool<>);
            var poolType = genericType.MakeGenericType(typeof(T));
            var pool = Activator.CreateInstance(poolType) as ReferencePool<T>;
            referencePools.Add(poolType, pool);
            pool.CollectionCheck = UseCollectionCheck();
            return pool;
        }
        
        public ReferencePool<T> GetReferencePool<T>() where T : class, IPoolable
        {
            if (referencePools.TryGetValue(typeof(T),out var pool))
            {
                return (ReferencePool<T>)pool;
            }
            return CreateReferencePool<T>();
        }
        
        #endregion

        #region GameObjectPool

        /// <summary>
        ///     gameObjectPool对象池管理
        /// </summary>
        private readonly Dictionary<string, GameObjectPool> gameObjectPools = new();

        public string GetDefaultGameObjectPoolName()
        {
            return $"GameObjectPool{gameObjectPools.Count}";
        }

        public bool TryGetGameObjectPool(string name, out GameObjectPool pool)
        {
            return gameObjectPools.TryGetValue(name, out pool);
        }

        public void RegisterGameObjectPool(GameObjectPool pool)
        {
            if (gameObjectPools.ContainsKey(pool.Name))
            {
#if UNITY_EDITOR
                Debug.LogError($"有重复的GameObjectPool Name，检查代码   Name:{pool.Name}");
#endif
            }
            else
            {
                pool.CollectionCheck = UseCollectionCheck();
                gameObjectPools.Add(pool.Name, pool);
            }
        }

        public void RemoveGameObjectPool(string gameObjectPoolName)
        {
            gameObjectPools.Remove(gameObjectPoolName);
        }

        public void RemoveGameObjectPool(GameObjectPool pool)
        {
            gameObjectPools.Remove(pool.Name);
        }

        public void ChangeGameObjectPoolParent(string gameObjectPoolName, Transform parent)
        {
            if (gameObjectPools.TryGetValue(gameObjectPoolName, out var pool)) pool.SetParent(parent);
        }

        #endregion


    }
}