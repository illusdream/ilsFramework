using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using static ilsFramework.SceneEvent;
namespace ilsFramework
{
    /// <summary>
    /// 提供对场景变化的事件侦测
    /// </summary>
    public class SceneHandler : ManagerSingleton<SceneHandler>,IManager
    {
        EventCenterCore sceneEventCenterCore;
        
        public static event Action<EventArgs> SceneOnChange
        {
            add => Instance.SceneChanged_AddListener(value);
            remove => Instance.SceneChanged_RemoveListener(value);
        }
        public static  event Action<EventArgs> SceneOnLoaded
        {
            add => Instance.SceneLoaded_AddListener(value);
            remove => Instance.SceneLoaded_RemoveListener(value);
        }
        public static  event Action<EventArgs> SceneOnUnloaded
        {
            add => Instance.SceneUnloaded_AddListener(value);
            remove => Instance.SceneUnloaded_RemoveListener(value);
        }
        public void Init()
        {
            sceneEventCenterCore = new EventCenterCore();
            SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
            SceneManager.sceneUnloaded += SceneManagerOnSceneUnloaded;
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
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
           
        }

        public void OnDrawGizmos()
        {
           
        }

        public void OnDrawGizmosSelected()
        {
           
        }

        #region SceneChanged

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            SceneChangedEventArgs args = new SceneChangedEventArgs(arg0, arg1);
            sceneEventCenterCore.BoradCastMessage(SceneChanged,args);
        }

        public void SceneChanged_AddListener(Action<EventArgs> action)
        {
            sceneEventCenterCore.AddListener(SceneChanged, action);
        }

        public void SceneChanged_RemoveListener(Action<EventArgs> action)
        {
            sceneEventCenterCore.RemoveListener(SceneChanged, action);
        }
        
        #endregion
        
        #region Loaded

        private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneLoadedEventArgs args = new SceneLoadedEventArgs(arg0, arg1);
            sceneEventCenterCore.BoradCastMessage(SceneLoaded,args);
        }

        public void SceneLoaded_AddListener(Action<EventArgs> action)
        {
            sceneEventCenterCore.AddListener(SceneLoaded, action);
        }

        public void SceneLoaded_RemoveListener(Action<EventArgs> action)
        {
            sceneEventCenterCore.RemoveListener(SceneLoaded, action);
        }
        
        
        #endregion
        
        #region Unloaded

        private void SceneManagerOnSceneUnloaded(Scene arg0)
        {
            SceneUnloadedEventArgs args = new SceneUnloadedEventArgs(arg0);
            sceneEventCenterCore.BoradCastMessage(SceneUnloaded,args);
        }

        public void SceneUnloaded_AddListener(Action<EventArgs> action)
        {
            sceneEventCenterCore.AddListener(SceneUnloaded, action);
        }

        public void SceneUnloaded_RemoveListener(Action<EventArgs> action)
        {
            sceneEventCenterCore.RemoveListener(SceneUnloaded, action);
        }
        
        #endregion
    }
}