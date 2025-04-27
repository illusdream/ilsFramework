namespace ilsFramework.Core
{
    public interface IState
    {
        void OnInit();

        void OnEnter();

        void OnUpdate();

        void OnFixedUpdate();

        void OnExit();

        void OnDestroy();
    }
}