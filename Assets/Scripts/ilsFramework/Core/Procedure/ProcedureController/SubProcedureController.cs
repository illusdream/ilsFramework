namespace ilsFramework.Core
{
    public class SubProcedureController : ProcedureController,IState
    {
        public FSM Owner { get; set; }
        public bool IsExecuting { get; set; }
        public virtual void OnInit()
        {
            
        }

        public virtual void OnEnter()
        {
            _currentState.IsExecuting = true;
            _currentState.OnEnter();
        }

        public virtual void OnUpdate()
        {
          _currentState.OnUpdate();
        }

        public virtual void OnLateUpdate()
        {
          _currentState.OnLateUpdate();
        }

        public  virtual void OnLogicUpdate()
        {
           _currentState.OnLogicUpdate();
        }

        public virtual  void OnFixedUpdate()
        {
            _currentState.OnFixedUpdate();
        }

        public virtual void OnExit()
        {
            _currentState.IsExecuting = false;
            _currentState.OnExit();
        }
        
        public virtual void ChangeState<T>() where T : IState
        {
            _currentState.OnExit();
            Owner.ChangeState<T>();
        }

        public virtual void ChangeStateByPopStack()
        {
            ChangeProcedureByPopStack();
        }
    

        public virtual void SelfChangeProcedureByPopStack()
        {
            ((ProcedureController)Owner).ChangeProcedureByPopStack();
        }
    }
}