using System;

namespace ilsFramework.Core
{
    public class SubOldFsm<T> : OldFSM<T>, IState where T : IEquatable<T>
    {
        public FSM Owner { get; set; }
        public bool IsExecuting { get; set; }

        public void OnInit()
        {
            onInit();
            OnInitAction?.Invoke();

            Init();
        }

        public void OnEnter()
        {
            Continue();
            onEnter();
            OnEnterAction?.Invoke();
        }

        public void OnUpdate()
        {
            onUpdate();
            OnUpdateAction?.Invoke();

            Update();
        }

        public void OnLateUpdate()
        {
            
        }

        public void OnLogicUpdate()
        {
            
        }

        public void OnFixedUpdate()
        {
            onFixedUpdate();
            OnFixedUpdateAction?.Invoke();

            FixedUpdate();
        }

        public void OnExit()
        {
            onExit();
            OnExitAction?.Invoke();

            Pause();
        }

        public sealed override void OnDestroy()
        {
            onDestroy();
            OnDestroyAction?.Invoke();
        }

        protected virtual void onInit()
        {
        }

        public event Action OnInitAction;

        public SubOldFsm<T> AddOnInitAction(Action action)
        {
            OnInitAction += action;
            return this;
        }

        protected virtual void onEnter()
        {
        }

        public event Action OnEnterAction;

        public SubOldFsm<T> AddOnEnterAction(Action action)
        {
            OnEnterAction += action;
            return this;
        }

        protected virtual void onUpdate()
        {
        }

        public event Action OnUpdateAction;

        public SubOldFsm<T> AddOnUpdateAction(Action action)
        {
            OnUpdateAction += action;
            return this;
        }

        protected virtual void onFixedUpdate()
        {
        }

        public event Action OnFixedUpdateAction;

        public SubOldFsm<T> AddOnFixedUpdateAction(Action action)
        {
            OnFixedUpdateAction += action;
            return this;
        }

        protected virtual void onExit()
        {
        }

        public event Action OnExitAction;

        public SubOldFsm<T> AddOnExitAction(Action action)
        {
            OnExitAction += action;
            return this;
        }

        protected virtual void onDestroy()
        {
        }

        public event Action OnDestroyAction;

        public SubOldFsm<T> AddOnDestroyAction(Action action)
        {
            OnDestroyAction += action;
            return this;
        }
    }
}