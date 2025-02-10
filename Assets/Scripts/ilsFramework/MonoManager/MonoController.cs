using System;
using UnityEngine;

namespace ilsFramework
{
    public class MonoController : MonoBehaviour
    {
        #region Update

        private Action _updateAction;
        void Update()
        {
            _updateAction?.Invoke();
        }

        public void SubscribeUpdateListener(Action listener)
        {
            _updateAction += listener;
        }
        public void UnSubscribeUpdateListener(Action listener)
        {
            _updateAction -= listener;
        }
        public void RemoveAllUpdateListener()
        {
            _updateAction = null;
        }
        #endregion
        #region FixedUpdate

        private Action _fixedUpdateAction;
        private void FixedUpdate()
        {
            _fixedUpdateAction?.Invoke();
        }
        public void SubscribeFixedUpdateListener(Action listener)
        {
            _fixedUpdateAction += listener;
        }
        public void UnSubscribeFixedUpdateListener(Action listener)
        {
            _fixedUpdateAction -= listener;
        }
        public void RemoveAllFixedUpdateListener()
        {
            _fixedUpdateAction = null;
        }
        #endregion
        #region LateUpdate

        private Action _lateUpdateAction;
        private void LateUpdate()
        {
            _lateUpdateAction?.Invoke();
        }
        public void SubscribeLateUpdateListener(Action listener)
        {
            _lateUpdateAction += listener;
        }
        public void UnSubscribeLateUpdateListener(Action listener)
        {
            _lateUpdateAction -= listener;
        }
        public void RemoveAllLateUpdateListener()
        {
            _lateUpdateAction = null;
        }
        #endregion
    }
}
