namespace ilsFramework.Core
{
    public interface IState
    {
        public FSM Owner { get; set; }
    
        public bool IsExecuting { get; set; }
        
        void OnInit();

        void OnEnter();

        void OnUpdate();

        void OnLateUpdate();

        void OnLogicUpdate();
        
        void OnFixedUpdate();

        void OnExit();

        void OnDestroy();
    }
}