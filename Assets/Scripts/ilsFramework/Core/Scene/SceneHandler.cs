using System;
using UnityEngine.SceneManagement;
using static ilsFramework.Core.SceneEvent;

namespace ilsFramework.Core
{
    /// <summary>
    ///     提供对场景变化的事件侦测
    /// </summary>
    public class SceneHandler : ManagerSingleton<SceneHandler>
    {
        private EventCenterCore sceneEventCenterCore;
        
        public override void OnInit()
        {
            sceneEventCenterCore = new EventCenterCore();
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
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
           
        }

        public override void OnDrawGizmos()
        {
            
        }

        public override void OnDrawGizmosSelected()
        {
            
        }

        public static event Action<EventArgs> SceneOnChange
        {
            add => Instance?.SceneChanged_AddListener(value);
            remove => Instance?.SceneChanged_RemoveListener(value);
        }

        public static event Action<EventArgs> SceneOnLoaded
        {
            add => Instance?.SceneLoaded_AddListener(value);
            remove => Instance?.SceneLoaded_RemoveListener(value);
        }

        public static event Action<EventArgs> SceneOnUnloaded
        {
            add => Instance?.SceneUnloaded_AddListener(value);
            remove => Instance?.SceneUnloaded_RemoveListener(value);
        }

        #region SceneChanged

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            var args = new SceneChangedEventArgs(arg0, arg1);
            sceneEventCenterCore.BroadcastMessage(SceneChanged, args);
        }

        /// <summary>
        ///     向场景变化时，添加监听器
        /// </summary>
        /// <param name="action">监听方法，具体的EventArgs为:<see cref="SceneChangedEventArgs" /></param>
        public void SceneChanged_AddListener(Action<EventArgs> action)
        {
            sceneEventCenterCore?.AddListener(SceneChanged, action);
        }

        /// <summary>
        ///     向场景变化时，移除监听器
        /// </summary>
        /// <param name="action">监听方法，具体的EventArgs为:<see cref="SceneChangedEventArgs" /></param>
        public void SceneChanged_RemoveListener(Action<EventArgs> action)
        {
            sceneEventCenterCore?.RemoveListener(SceneChanged, action);
        }

        #endregion

        #region Loaded

        private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            var args = new SceneLoadedEventArgs(arg0, arg1);
            sceneEventCenterCore?.BroadcastMessage(SceneLoaded, args);
        }

        /// <summary>
        ///     向场景加载时，添加监听器
        /// </summary>
        /// <param name="action">监听方法，具体的EventArgs为:<see cref="SceneLoadedEventArgs" /></param>
        public void SceneLoaded_AddListener(Action<EventArgs> action)
        {
            sceneEventCenterCore?.AddListener(SceneLoaded, action);
        }

        /// <summary>
        ///     向场景加载时，移除监听器
        /// </summary>
        /// <param name="action">监听方法，具体的EventArgs为:<see cref="SceneLoadedEventArgs" /></param>
        public void SceneLoaded_RemoveListener(Action<EventArgs> action)
        {
            sceneEventCenterCore?.RemoveListener(SceneLoaded, action);
        }

        #endregion

        #region Unloaded

        private void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            var args = new SceneUnloadedEventArgs(arg0);
            sceneEventCenterCore?.BroadcastMessage(SceneUnloaded, args);
        }

        /// <summary>
        ///     向场景卸载时，添加监听器
        /// </summary>
        /// <param name="action">监听方法，具体的EventArgs为:<see cref="SceneUnloadedEventArgs" /></param>
        public void SceneUnloaded_AddListener(Action<EventArgs> action)
        {
            sceneEventCenterCore?.AddListener(SceneUnloaded, action);
        }

        /// <summary>
        ///     向场景卸载时，添加监听器
        /// </summary>
        /// <param name="action">监听方法，具体的EventArgs为:<see cref="SceneUnloadedEventArgs" /></param>
        public void SceneUnloaded_RemoveListener(Action<EventArgs> action)
        {
            sceneEventCenterCore?.RemoveListener(SceneUnloaded, action);
        }

        #endregion
    }
}