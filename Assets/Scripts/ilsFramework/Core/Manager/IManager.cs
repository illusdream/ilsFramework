namespace ilsFramework.Core
{
    /// <summary>
    ///     管理类的接口，用此接入ilsFrameworkCore
    ///     与<see cref="ManagerSingleton{T}" />一同使用创建Manger单例类
    /// </summary>
    public interface IManager
    {
        /// <summary>
        ///     初始化模块
        /// </summary>
        void Init();

        /// <summary>
        ///     轮询模块
        /// </summary>
        void Update();

        void LateUpdate();

        void LogicUpdate();

        void FixedUpdate();


        /// <summary>
        ///     停止并清理管理类
        /// </summary>
        void Destroy();

        /// <summary>
        /// </summary>
        void DrawGizmos();

        /// <summary>
        /// </summary>
        void DrawGizmosSelected();
    }
}