using System;

namespace ilsFramework
{
    /// <summary>
    /// 通用状态机对应状态
    /// </summary>
    public class State : IState
    {
        protected virtual void onInit()
        {
            
        }
        public event Action OnInitAction;

        public State AddOnInitAction(Action action)
        {
            OnInitAction += action;
            return this;
        }
        protected virtual void onEnter()
        {
            
        }
        public event Action OnEnterAction;

        public State AddOnEnterAction(Action action)
        {
            OnEnterAction += action;
            return this;
        }
        protected virtual void onUpdate()
        {
            
        }
        public event Action OnUpdateAction;

        public State AddOnUpdateAction(Action action)
        {
            OnUpdateAction += action;
            return this;
        }
        protected virtual void onFixedUpdate()
        {
            
        }
        public event Action OnFixedUpdateAction;

        public State AddOnFixedUpdateAction(Action action)
        {
            OnFixedUpdateAction += action;
            return this;
        }
        
        protected virtual void onExit()
        {
            
        }
        public event Action OnExitAction;

        public State AddOnExitAction(Action action)
        {
            OnExitAction += action;
            return this;
        }
        protected virtual void onDestroy()
        {
            
        }
        public event Action OnDestroyAction;

        public State AddOnDestroyAction(Action action)
        {
            OnDestroyAction += action;
            return this;
        }
        
        
        public  void OnInit()
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
    }
}