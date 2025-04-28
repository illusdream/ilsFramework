using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework.Core
{
    public class PoolManager : ManagerSingleton<PoolManager>
    {
        public override void OnInit()
        {
            
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
        }

        public override void OnDrawGizmos()
        {
            
        }

        public override void OnDrawGizmosSelected()
        {
            
        }


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