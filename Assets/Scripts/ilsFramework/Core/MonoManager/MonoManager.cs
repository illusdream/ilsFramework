using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

namespace ilsFramework.Core
{
    public class MonoManager : ManagerSingleton<MonoManager>
    {
        private List<IEnumerator> _needUseCorotuine;
        private MonoController controller;
        
        public override void OnInit()
        {
            _needUseCorotuine = new List<IEnumerator>();
            FrameworkCore.Instance.CreateEmptyGameObject("PublicMonoHandler", go => { controller = go.AddComponent<MonoController>(); });
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

        #region 更新

        public void SubcirbeUpdateListener(Action listener)
        {
            controller.SubscribeUpdateListener(listener);
        }

        public void UnSubcribeUpdateListener(Action listener)
        {
            controller.UnSubscribeUpdateListener(listener);
        }

        public void SubcirbeFixedUpdateListener(Action listener)
        {
            controller.SubscribeFixedUpdateListener(listener);
        }

        public void UnSubcribeFixedUpdateListener(Action listener)
        {
            controller.UnSubscribeFixedUpdateListener(listener);
        }

        public void SubcirbeLateUpdateListener(Action listener)
        {
            controller.SubscribeLateUpdateListener(listener);
        }

        public void UnSubcribeLateUpdateListener(Action listener)
        {
            controller.UnSubscribeLateUpdateListener(listener);
        }

        #endregion

        #region 协程

        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return controller.StartCoroutine(routine);
        }

        public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value)
        {
            return controller.StartCoroutine(methodName, value);
        }

        public Coroutine StartCoroutine(string methodName)
        {
            return controller.StartCoroutine(methodName);
        }

        public void StopCoroutine(IEnumerator routine)
        {
            controller.StopCoroutine(routine);
        }

        public void StopCoroutine(string methodName)
        {
            controller.StopCoroutine(methodName);
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            controller.StopCoroutine(coroutine);
        }

        public void StopAllCoroutine()
        {
            controller.StopAllCoroutines();
        }

        private void CheckControllerIsNotNull()
        {
            if (controller is null)
                FrameworkCore.Instance.CreateEmptyGameObject("PublicMonoHandler", go => { controller = go.AddComponent<MonoController>(); });
        }

        #endregion
    }
}