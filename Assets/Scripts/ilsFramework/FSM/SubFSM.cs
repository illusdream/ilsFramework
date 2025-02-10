using System;

namespace ilsFramework
{
    public class SubFSM<T> : FSM<T>,IState where T : IEquatable<T>
    {
        protected virtual void onInit()
        {
            
        }
        public event Action OnInitAction;

        public SubFSM<T> AddOnInitAction(Action action)
        {
            OnInitAction += action;
            return this;
        }
        protected virtual void onEnter()
        {
            
        }
        public event Action OnEnterAction;

        public SubFSM<T> AddOnEnterAction(Action action)
        {
            OnEnterAction += action;
            return this;
        }
        protected virtual void onUpdate()
        {
            
        }
        public event Action OnUpdateAction;

        public SubFSM<T> AddOnUpdateAction(Action action)
        {
            OnUpdateAction += action;
            return this;
        }
        protected virtual void onFixedUpdate()
        {
            
        }
        public event Action OnFixedUpdateAction;

        public SubFSM<T> AddOnFixedUpdateAction(Action action)
        {
            OnFixedUpdateAction += action;
            return this;
        }
        
        protected virtual void onExit()
        {
            
        }
        public event Action OnExitAction;

        public SubFSM<T> AddOnExitAction(Action action)
        {
            OnExitAction += action;
            return this;
        }
        protected virtual void onDestroy()
        {
            
        }
        public event Action OnDestroyAction;

        public SubFSM<T> AddOnDestroyAction(Action action)
        {
            OnDestroyAction += action;
            return this;
        }
        
        
        
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
    }
}