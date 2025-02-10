using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;

namespace ilsFramework
{
    public class MonoManager : ManagerSingleton<MonoManager>, IManager
    {
      private  MonoController controller;
       private List<IEnumerator> _needUseCorotuine;
        public int Priority => (int)ManagerPrioritySet.MonoManager;



        public void Init()
        {
            _needUseCorotuine = new List<IEnumerator>();
            FrameworkCore.Instance.CreateEmptyGameObject("PublicMonoHandler", (go) =>
            {
                controller = go.AddComponent<MonoController>();
            });
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

        void CheckControllerIsNotNull()
        {
            if (controller is null)
            {
                FrameworkCore.Instance.CreateEmptyGameObject("PublicMonoHandler", (go) =>
                {
                    controller = go.AddComponent<MonoController>();
                });
            }
        }
        #endregion

    }
}
