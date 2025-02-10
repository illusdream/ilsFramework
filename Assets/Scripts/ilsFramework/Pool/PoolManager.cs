using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework
{
    public class PoolManager : ManagerSingleton<PoolManager>,IManager
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
            foreach (var gameObjectPool in gameObjectPools)
            {
                gameObjectPool.Value.OnDestroy();
            }
            gameObjectPools.Clear();
        }

        public void OnDrawGizmos()
        {
            
        }

        public void OnDrawGizmosSelected()
        {
            
        }


        #region GameObjectPool
        /// <summary>
        /// gameObjectPool对象池管理
        /// </summary>
        private Dictionary<string,GameObjectPool> gameObjectPools = new Dictionary<string, GameObjectPool>();
        
        public string GetDefaultGameObjectPoolName()
        {
            return $"GameObjectPool{gameObjectPools.Count}";
        }

        public bool TryGetGameObjectPool(string name,out GameObjectPool pool)
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
            if (gameObjectPools.TryGetValue(gameObjectPoolName, out GameObjectPool pool))
            {
                pool.SetParent(parent);
            }
        }
        #endregion


    }
}