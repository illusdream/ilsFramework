namespace ilsFramework.Core
{
    public class FSMState : IState
    {
        public FSM Owner { get; set; }
        public bool IsExecuting { get; set; }

        public virtual void OnInit()
        {
            
        }

        public virtual void OnEnter()
        {
           
        }

        public virtual void OnUpdate()
        {
           
        }

        public virtual void OnLateUpdate()
        {
           
        }

        public virtual void OnLogicUpdate()
        {
            
        }

        public virtual void OnFixedUpdate()
        {
           
        }

        public virtual void OnExit()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
        public virtual void ChangeState<T>() where T : IState
        {
            Owner?.ChangeState<T>();
        }
    }
}