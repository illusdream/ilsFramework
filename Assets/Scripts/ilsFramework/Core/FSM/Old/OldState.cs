using System;

namespace ilsFramework.Core
{
    /// <summary>
    ///     通用状态机对应状态
    /// </summary>
    public class OldState : IState
    {
        public FSM Owner { get; set; }
        public bool IsExecuting { get; set; }

        public void OnInit()
        {
            onInit();
            OnInitAction?.Invoke();
        }

        public void OnEnter()
        {
            onEnter();
            OnEnterAction?.Invoke();
        }

        public void OnUpdate()
        {
            onUpdate();
            OnUpdateAction?.Invoke();
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
        }

        public void OnExit()
        {
            onExit();
            OnExitAction?.Invoke();
        }

        public void OnDestroy()
        {
            onDestroy();
            OnDestroyAction?.Invoke();
        }

        protected virtual void onInit()
        {
        }

        public event Action OnInitAction;

        public OldState AddOnInitAction(Action action)
        {
            OnInitAction += action;
            return this;
        }

        protected virtual void onEnter()
        {
        }

        public event Action OnEnterAction;

        public OldState AddOnEnterAction(Action action)
        {
            OnEnterAction += action;
            return this;
        }

        protected virtual void onUpdate()
        {
        }

        public event Action OnUpdateAction;

        public OldState AddOnUpdateAction(Action action)
        {
            OnUpdateAction += action;
            return this;
        }

        protected virtual void onFixedUpdate()
        {
        }

        public event Action OnFixedUpdateAction;

        public OldState AddOnFixedUpdateAction(Action action)
        {
            OnFixedUpdateAction += action;
            return this;
        }

        protected virtual void onExit()
        {
        }

        public event Action OnExitAction;

        public OldState AddOnExitAction(Action action)
        {
            OnExitAction += action;
            return this;
        }

        protected virtual void onDestroy()
        {
        }

        public event Action OnDestroyAction;

        public OldState AddOnDestroyAction(Action action)
        {
            OnDestroyAction += action;
            return this;
        }
    }
}