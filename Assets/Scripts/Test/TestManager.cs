using System;
using ilsFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ilsFramework.SceneEvent;

namespace Test
{
    public class TestManager : ManagerSingleton<TestManager>,IManager
    {
        public void Init()
        {
            SceneHandler.SceneOnUnloaded += InstanceOnSceneOnUnloaded;
            SceneHandler.SceneOnChange += InstanceOnSceneOnChange;
            SceneHandler.SceneOnLoaded += InstanceOnSceneOnLoaded;
        }

        private void InstanceOnSceneOnLoaded(EventArgs obj)
        {
            SceneLoadedEventArgs args = (SceneLoadedEventArgs)obj;
            Debug.Log($"OnSceneOnLoaded: {args.Scene.name}");
            Debug.Log($"OnSceneOnLoaded: {args.LoadSceneMode.ToString()}");
        }

        private void InstanceOnSceneOnChange(EventArgs obj)
        {
             SceneChangedEventArgs args = (SceneChangedEventArgs)obj;
             Debug.Log($"OnSceneOnChange: {args.BeforeScene.name}");
             Debug.Log($"OnSceneOnChange: {args.AfterScene.name}");
        }

        private void InstanceOnSceneOnUnloaded(EventArgs obj)
        {
             SceneUnloadedEventArgs args = (SceneUnloadedEventArgs)obj;
             Debug.Log($"OnSceneOnUnloaded: {args.Scene.name}");
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
    }
}