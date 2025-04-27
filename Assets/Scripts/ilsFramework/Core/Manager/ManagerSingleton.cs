using System;
using UnityEngine;

namespace ilsFramework.Core
{
    /// <summary>
    ///     管理类单例，需要配合<see cref="IManager" />一同使用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ManagerSingleton<T> : IManagerSingleton where T : class, IManager, new()
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
    }
}