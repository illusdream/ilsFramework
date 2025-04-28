using System;
using System.Collections.Generic;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     管理类单例，需要配合<see cref="IManager" />一同使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ManagerSingleton<T> : IManagerSingleton,IManager where T : class, IManager, new()
    {
        private static GameObject _containerObject;

        //提供一个快速的访问方式从FrameworkCore获取管理类
        public static T Instance => FrameworkCore.Instance.GetManager<T>();

        public static GameObject ContainerObject
        {
            get
            {
                if (_containerObject == null) _containerObject = FrameworkCore.Instance.GetManagerContainerGameObject<T>();
                return _containerObject;
            }
        }

        public int ManagerUpdateIndex { get; set; }
        
        //统一采用先更新manager自身的，再更新模块的方式
        public List<ManagerModule> ManagerModules { get; set; }
        public Dictionary<Type,ManagerModule> ManagerModulesDictionary { get; set; }
        public void CreateAllNeedModules(Type[] needModules)
        {
            ManagerModules = new List<ManagerModule>();
            ManagerModulesDictionary = new Dictionary<Type, ManagerModule>();
            if (needModules == null)
                return;
            foreach (var type in needModules)
            {
                var typeInstance = Activator.CreateInstance(type);

                if (typeInstance is ManagerModule managerModule)
                {
                    ManagerModules.Add(managerModule);
                    ManagerModulesDictionary.Add(type, managerModule);
                }
            }
            ManagerModules.Sort((a,b)=>a.Priority.CompareTo(b.Priority));
        }

        protected TModule GetModule<TModule>() where TModule : ManagerModule
        {
            if (ManagerModulesDictionary.TryGetValue(typeof(TModule),out var target) && target is TModule module)
            {
                return module;
            }
            return null;
        }

        protected void SetModuleEnabled<TModule>(bool enabled)
        {
            if (ManagerModulesDictionary.TryGetValue(typeof(TModule),out var target) )
            {
                if (enabled ^ target.Enabled)
                {
                    switch (enabled)
                    {
                        case true:
                            target.OnEnable();
                            break;
                        case false:
                            target.OnDisable();
                            break;
                    }
                    target.Enabled = enabled;
                }
            }
        }
        
        public void Init()
        {
            OnInit();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnInit();
            }
        }

        public abstract void OnInit();
        
        public void Update()
        {
            OnUpdate();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnUpdate();
            }
        }
        
        public abstract void OnUpdate();

        public void LateUpdate()
        {
            OnLateUpdate();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnLateUpdate();
            }
        }

        public abstract void OnLateUpdate();
        
        public void LogicUpdate()
        {
            OnLogicUpdate();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnLogicUpdate();
            }
        }
        
        public abstract void OnLogicUpdate();

        public void FixedUpdate()
        {
            OnFixedUpdate();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnFixedUpdate();
            }
        }
        
        public abstract void OnFixedUpdate();

        public void Destroy()
        {
           OnDestroy();
           foreach (var module in ManagerModules)
           {
               if (!module.Enabled)
                   continue;
               module.OnDestroy();
           }
        }
        
        public abstract void OnDestroy();

        public void DrawGizmos()
        {
            OnDrawGizmos();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnDrawGizmos();
            }
        }
        
        public abstract void OnDrawGizmos();

        public void DrawGizmosSelected()
        {
            OnDrawGizmosSelected();
            foreach (var module in ManagerModules)
            {
                if (!module.Enabled)
                    continue;
                module.OnDrawGizmosSelected();
            }
        }
        public abstract void OnDrawGizmosSelected();
    }
}